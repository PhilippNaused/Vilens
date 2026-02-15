using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

[DebuggerTypeProxy(typeof(StackHelperDebugView))]
internal struct StackHelper
{
    [ExcludeFromCodeCoverage]
    private sealed class StackHelperDebugView(StackHelper helper)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public StackInfo[] Instructions => helper.instructions
            .Select((instr, index) => new StackInfo(instr, helper.stackHeights[index]))
            .ToArray();

        [DebuggerDisplay("{StackHeight}", Name = "{Instruction}")]
        public readonly record struct StackInfo(Instruction Instruction, uint? StackHeight);
    }

    private readonly IList<Instruction> instructions;
    private readonly IList<ExceptionHandler> exceptionHandlers;
    private readonly ushort?[] stackHeights;
    private ushort maxStack;

    private StackHelper(CilBody body)
    {
        instructions = body.Instructions;
        exceptionHandlers = body.ExceptionHandlers;
        stackHeights = new ushort?[instructions.Count];
        maxStack = 0;
    }

    public static ushort GetMaxStack(CilBody body)
    {
        var helper = new StackHelper(body);
        helper.ExploreAll();
        return helper.maxStack;
    }

    public static ushort?[] GetStackHeights(CilBody body)
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
            if (handler.FilterStart is not null)
            {
                Explore(handler.FilterStart, 1);
            }
            if (handler.HandlerStart is not null)
            {
                bool pushed = handler.IsCatch || handler.IsFilter;
                Explore(handler.HandlerStart, pushed ? (ushort)1 : (ushort)0);
            }
        }

        Debug.Assert((stackHeights.Max() ?? 0) == maxStack);
    }

    private readonly int IndexOf(Instruction instr)
    {
        var index = instructions.IndexOf(instr);
        if (index < 0)
            ThrowInstructionNotFound(instr);
        return index;
    }

    private void Explore(Instruction instr, ushort stackHeight)
    {
        Explore(IndexOf(instr), stackHeight);
    }

    private void Explore(int index, ushort stackHeight)
    {
    start:
        var previous = stackHeights[index];
        if (previous is ushort prev)
        {
            if (prev != stackHeight)
                ThrowInconsistentStackHeight(index, stackHeight, prev);
            return; // already visited this instruction
        }
        var instr = instructions[index];
        stackHeights[index] = stackHeight;
        if (stackHeight > maxStack)
            maxStack = stackHeight;

        instr.CalculateStackUsage(out int pushes, out int pops);
        if (pops == -1)
        {
            stackHeight = 0;
        }
        else
        {
            if (stackHeight < pops)
                throw new InvalidMethodException($"Stack is negative at {instr}.");
            stackHeight -= (ushort)pops;
            stackHeight += (ushort)pushes;
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
                    return; // method terminates here
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
                    ThrowBadStackAtReturn(stackHeight, instr);
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
                        Explore(target, stackHeight); // explore the branch target
                    }
                    index++;
                    goto start; // explore the next instruction (fall-through)
                }
                else
                {
                    var target = (Instruction)instr.Operand;
                    Explore(target, stackHeight); // explore the branch target
                    index++;
                    goto start; // explore the next instruction
                }
            }
            default:
                ThrowUnknownFlowControl(instr);
                return;
        }
    }

    // These are the cold paths. They should never be called on a valid method.

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining), ExcludeFromCodeCoverage]
    private static void ThrowUnknownFlowControl(Instruction instr)
    {
        throw new InvalidMethodException($"Unsupported flow control {instr.OpCode.FlowControl} for {instr}.");
    }

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining), ExcludeFromCodeCoverage]
    private static void ThrowBadStackAtReturn(ushort stackHeight, Instruction instr)
    {
        throw new InvalidMethodException($"Returned from method via {instr} with stack height {stackHeight}.");
    }

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining), ExcludeFromCodeCoverage]
    private readonly void ThrowInconsistentStackHeight(int index, ushort stackHeight, ushort previous)
    {
        throw new InvalidMethodException($"Inconsistent stack height {stackHeight} != {previous} for {instructions[index]}.");
    }

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining), ExcludeFromCodeCoverage]
    private static void ThrowInstructionNotFound(Instruction instr)
    {
        throw new InvalidMethodException($"Instruction {instr} not found.");
    }
}
