using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

internal sealed class StackHelper
{
    private readonly InstructionStackHeight[] data;
    private readonly MethodDef method;

    private StackHelper(MethodDef method)
    {
        this.method = method;
        data = method.Body.Instructions.Select(i => new InstructionStackHeight(i)).ToArray();
    }

    public static int?[] CalculateStackHeights(MethodDef method)
    {
        var helper = new StackHelper(method);
        helper.ExploreAll();

        return helper.data.Select(x => x.StackHeight).ToArray();
    }

    [DebuggerDisplay("{StackHeight}", Name = "{Instruction}")]
    private struct InstructionStackHeight(Instruction instruction)
    {
        public int? StackHeight { get; set; }
        public Instruction Instruction { get; } = instruction;
    }

    private void ExploreAll()
    {
        Explore(0, 0);

        foreach (var handler in method.Body.ExceptionHandlers)
        {
            Debug.Assert(handler != null, "Exception handler is null.");
            if (handler!.FilterStart is not null)
            {
                var idx = IndexOf(handler.FilterStart);
                Explore(idx, 1);
            }
            if (handler!.HandlerStart is not null)
            {
                var idx = IndexOf(handler.HandlerStart);
                bool pushed = handler.IsCatch || handler.IsFilter;
                Explore(idx, pushed ? 1 : 0);
            }
        }
    }

    private int IndexOf(Instruction instr)
    {
        var index = method.Body.Instructions.IndexOf(instr);
        if (index < 0)
            throw new InvalidMethodException($"Instruction {instr} not found in method {method}.");
        return index;
    }

    private void Explore(int index, int stackHeight)
    {
        // recursively iterate instructions, setting stack heights and checking for consistency
    start:
        ref var info = ref data[index];
        if (info.StackHeight is not null)
        {
            if (info.StackHeight != stackHeight)
                throw new InvalidMethodException($"Inconsistent stack height {stackHeight} != {info.StackHeight} for {info.Instruction} in {method}.");
            return; // already visited this instruction with the same stack height, no need to explore further
        }
        info.StackHeight = stackHeight;

        var instr = info.Instruction;
        instr.CalculateStackUsage(method.HasReturnType, out int pushes, out int pops);
        if (pops == -1)
        {
            stackHeight = 0;
        }
        else
        {
            stackHeight -= pops;
            if (stackHeight < 0)
                throw new InvalidMethodException($"Stack is negative at {instr} in {method}.");
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
                    throw new InvalidMethodException($"Returned from method via {instr} with stack height {stackHeight} in {method}.");
                return; // method terminates here
            }
            case FlowControl.Throw:
            {
                return; // method terminates here
            }
            case FlowControl.Branch: // unconditional branch
            {
                var target = (Instruction)instr.Operand;
                index = IndexOf(target);
                goto start; // tail recursion
            }
            case FlowControl.Cond_Branch:
            {
                if (instr.OpCode.Code == Code.Switch)
                {
                    foreach (var target in (IList<Instruction>)instr.Operand)
                    {
                        Explore(IndexOf(target), stackHeight); // explore the branch target
                    }
                    index++;
                    goto start; // explore the next instruction
                }
                else
                {
                    var target = (Instruction)instr.Operand;
                    Explore(IndexOf(target), stackHeight); // explore the branch target
                    index++;
                    goto start; // explore the next instruction
                }
            }
            default:
                throw new InvalidMethodException($"Unsupported flow control {instr.OpCode.FlowControl} for {instr} in {method}.");
        }
    }
}
