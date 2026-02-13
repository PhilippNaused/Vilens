using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

//TODO: use [DebuggerTypeProxy]
internal readonly struct StackHelper
{
    private readonly IList<Instruction> instructions;
    private readonly IList<ExceptionHandler> exceptionHandlers;
    private readonly uint?[] stackHeights;

    private StackHelper(CilBody body)
    {
        instructions = body.Instructions;
        exceptionHandlers = body.ExceptionHandlers;
        stackHeights = new uint?[instructions.Count];
    }

    public static uint GetMaxStack(CilBody body)
    {
        var helper = new StackHelper(body);
        helper.ExploreAll();
        return helper.stackHeights.Max() ?? 0;
    }

    public static uint?[] GetStackHeights(CilBody body)
    {
        var helper = new StackHelper(body);
        helper.ExploreAll();
        return helper.stackHeights;
    }

    private void ExploreAll()
    {
        Explore(0, 0);

        foreach (var handler in exceptionHandlers)
        {
            if (handler!.FilterStart is not null)
            {
                var idx = IndexOf(handler.FilterStart);
                Explore(idx, 1);
            }
            if (handler!.HandlerStart is not null)
            {
                var idx = IndexOf(handler.HandlerStart);
                bool pushed = handler.IsCatch || handler.IsFilter;
                Explore(idx, pushed ? 1u : 0u);
            }
        }
    }

    private readonly int IndexOf(Instruction instr)
    {
        var index = instructions.IndexOf(instr);
        if (index < 0)
            throw new InvalidMethodException($"Instruction {instr} not found.");
        return index;
    }

    private void Explore(int index, uint stackHeight)
    {
    start:
        var previous = stackHeights[index];
        var instr = instructions[index];
        if (previous is not null)
        {
            if (previous != stackHeight)
                throw new InvalidMethodException($"Inconsistent stack height {stackHeight} != {previous} for {instr}.");
            return; // already visited this instruction with the same stack height, no need to explore further
        }
        stackHeights[index] = stackHeight;

        instr.CalculateStackUsage(out int pushes, out int pops);
        if (pops == -1)
        {
            stackHeight = 0;
        }
        else
        {
            if (stackHeight < pops)
                throw new InvalidMethodException($"Stack is negative at {instr}.");
            stackHeight -= (uint)pops;
            stackHeight += (uint)pushes;
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
                if (stackHeight > 1)
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
                throw new InvalidMethodException($"Unsupported flow control {instr.OpCode.FlowControl} for {instr}.");
        }
    }
}
