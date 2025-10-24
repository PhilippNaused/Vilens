using System.Diagnostics;
using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

internal static class StackHelper
{
    public static Dictionary<Instruction, int> CalculateStackHeights(CilBody body)
    {
        var exceptionHandlers = body.ExceptionHandlers;
        var stackHeights = new Dictionary<Instruction, int>();
        foreach (var eh in exceptionHandlers)
        {
            Debug.Assert(eh != null, "Exception handler is null.");
            Instruction instr;
            if ((instr = eh!.TryStart) is not null)
                stackHeights[instr] = 0;
            if ((instr = eh.FilterStart) is not null)
                stackHeights[instr] = 1;
            if ((instr = eh.HandlerStart) is not null)
            {
                bool pushed = eh.IsCatch || eh.IsFilter;
                stackHeights[instr] = pushed ? 1 : 0;
            }
        }

        int stack = 0;
        bool resetStack = false;
        var instructions = body.Instructions;
        for (int i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];
            if (instr is null)
                continue;

            if (resetStack)
            {
                _ = stackHeights.TryGetValue(instr, out stack);
                resetStack = false;
            }
            SetStack(instr, stack, stackHeights);
            var opCode = instr.OpCode;
            var code = opCode.Code;
            if (code == Code.Jmp)
            {
                if (stack != 0)
                    throw new InvalidMethodException($"Exited method via {instr} with stack height {stack}.");
            }
            else
            {
                instr.CalculateStackUsage(out int pushes, out int pops);
                if (pops == -1) // PopAll
                    stack = 0;
                else
                {
                    stack -= pops;
                    if (stack < 0)
                        throw new InvalidMethodException($"Stack is negative at {instr}");
                    stack += pushes;
                }
            }
            if (stack < 0)
                throw new InvalidMethodException($"Stack is negative at {instr}");

            switch (opCode.FlowControl)
            {
                case FlowControl.Branch:
                    SetStack((Instruction)instr.Operand, stack, stackHeights);
                    resetStack = true;
                    break;

                case FlowControl.Call:
                    if (code == Code.Jmp)
                        resetStack = true;
                    break;

                case FlowControl.Cond_Branch:
                    if (code == Code.Switch)
                    {
                        if (instr.Operand is IList<Instruction> targets)
                        {
                            for (int j = 0; j < targets.Count; j++)
                                SetStack(targets[j], stack, stackHeights);
                        }
                    }
                    else
                        SetStack((Instruction)instr.Operand, stack, stackHeights);
                    break;

                case FlowControl.Return:
                case FlowControl.Throw:
                    resetStack = true;
                    break;
            }
        }

        Debug.Assert(stackHeights.Count == instructions.Count);
        Debug.Assert(stackHeights.Values.Min() == 0);
        Debug.Assert(stackHeights[instructions[0]] == 0);
        return stackHeights;
    }

    private static void SetStack(Instruction instr, int stack, Dictionary<Instruction, int> stackHeights)
    {
        Debug.Assert(instr != null);
        if (stackHeights.TryGetValue(instr!, out int stack2))
        {
            if (stack != stack2)
                throw new InvalidMethodException($"Inconsistent stack height {stack} != {stack2} for {instr}.");
            return;
        }
        stackHeights[instr!] = stack;
    }
}
