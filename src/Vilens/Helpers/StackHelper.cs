using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

internal static class StackHelper
{
    [DebuggerDisplay("{StackHeight}", Name = "{Instruction}")]
    private struct InstructionStackHeight(Instruction instruction)
    {
        public int? StackHeight { get; set; }
        public Instruction Instruction { get; } = instruction;
    }

    public static int?[] CalculateStackHeights(MethodDef method)
    {
        var body = method.Body;
        var instructions = body.Instructions;
        var exceptionHandlers = body.ExceptionHandlers;
        InstructionStackHeight[] data = instructions.Select(i => new InstructionStackHeight(i)).ToArray();

        Explore(0, 0);

        foreach (var eh in exceptionHandlers)
        {
            Debug.Assert(eh != null, "Exception handler is null.");
            if (eh!.FilterStart is not null)
            {
                var idx = instructions.IndexOf(eh.FilterStart);
                Explore(idx, 1);
            }
            if (eh!.HandlerStart is not null)
            {
                var idx = instructions.IndexOf(eh.HandlerStart);
                bool pushed = eh.IsCatch || eh.IsFilter;
                Explore(idx, pushed ? 1 : 0);
            }
        }

        void Explore(int index, int stackHeight)
        {
            // recursively iterate instructions, setting stack heights and checking for consistency
        start:
            if (data[index].StackHeight.HasValue)
            {
                if (data[index].StackHeight!.Value != stackHeight)
                    throw new InvalidMethodException($"Inconsistent stack height {stackHeight} != {data[index].StackHeight!.Value} for {data[index].Instruction}.");
                return; // already visited this instruction with the same stack height, no need to explore further
            }
            data[index].StackHeight = stackHeight;
            var instr = data[index].Instruction;
            instr.CalculateStackUsage(method.HasReturnType, out int pushes, out int pops);
            if (pops == -1)
            {
                stackHeight = 0;
            }
            else
            {
                stackHeight -= pops;
                if (stackHeight < 0)
                    throw new InvalidMethodException($"Stack is negative at {instr}");
                stackHeight += pushes;
            }
            switch (instr.OpCode.FlowControl)
            {
                case FlowControl.Break:
                case FlowControl.Call:
                case FlowControl.Meta:
                case FlowControl.Next:
                {
                    if (instr.OpCode.Code == Code.Jmp)
                    {
                        return; // method terminates here, no need to explore further
                    }
                    else
                    {
                        index++;
                        goto start; // just continue to the next instruction
                    }
                }
                case FlowControl.Return:
                {
                    if (stackHeight != 0)
                        throw new InvalidMethodException($"Returned from method via {instr} with stack height {stackHeight}.");
                    return; // method terminates here
                }
                case FlowControl.Throw:
                {
                    return; // method terminates here
                }
                case FlowControl.Branch: // unconditional branch
                {
                    var target = (Instruction)instr.Operand;
                    var targetIndex = instructions.IndexOf(target);
                    if (targetIndex < 0)
                        throw new InvalidMethodException($"Invalid branch target {target} for {instr}.");
                    index = targetIndex;
                    goto start; // tail recursion
                }
                case FlowControl.Cond_Branch:
                {
                    if (instr.OpCode.Code == Code.Switch)
                    {
                        foreach (var target in (IList<Instruction>)instr.Operand)
                        {
                            var targetIndex = instructions.IndexOf(target);
                            if (targetIndex < 0)
                                throw new InvalidMethodException($"Invalid branch target {target} for {instr}.");
                            Explore(targetIndex, stackHeight); // explore the branch target
                        }
                        index++;
                        goto start; // explore the next instruction
                    }
                    else
                    {
                        var target = (Instruction)instr.Operand;
                        var targetIndex = instructions.IndexOf(target);
                        if (targetIndex < 0)
                            throw new InvalidMethodException($"Invalid branch target {target} for {instr}.");
                        Explore(targetIndex, stackHeight); // explore the branch target
                        index++;
                        goto start; // explore the next instruction
                    }
                }
                default:
                    throw new InvalidMethodException($"Unsupported flow control {instr.OpCode.FlowControl} for {instr}.");
            }
        }

        return data.Select(x => x.StackHeight).ToArray();
    }
}
