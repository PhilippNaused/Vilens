using System.Diagnostics;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class ControlFlow : FeatureBase
{
    private readonly List<IMethodData> _methods;
    private int _counter;

    /// <inheritdoc />
    public ControlFlow(Scrambler scrambler) : base(scrambler)
    {
        _methods = Database.Methods.Where(m => m.HasFeatures(VilensFeature.ControlFlow)
                                            && m.Item.HasBody
                                            ).ToList();
    }

    public override Logger Log { get; } = new Logger(nameof(ControlFlow));

    /// <inheritdoc />
    public override void Execute()
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = Scrambler.Cancellation,
            MaxDegreeOfParallelism = Environment.ProcessorCount, // use all available cores
        };
#if DEBUG
        if (Debugger.IsAttached)
        {
            // Don't use parallelization when debugging to make it easier to step through the code.
            parallelOptions.MaxDegreeOfParallelism = 1;
        }
#endif
        var result = Parallel.ForEach(_methods, parallelOptions, method =>
        {
            Cancellation.ThrowIfCancellationRequested();
            if (method.WasTrimmed())
            {
                Log.Trace("Skipping [{0}] because it was trimmed", method);
                return;
            }
            CilBody body = method.Item.Body;

            if (body.ExceptionHandlers.Any(eh => eh.HandlerEnd is null))
            {
                // TODO: remove
                Log.Warn("Skipping [{0}] because of unknown exception handler end", method);
                return;
            }

            AddPadding(method); // TODO: revert if not needed

            Log.Trace("Calculating [{0}]", method);
            List<List<Instruction>> blocks;

            try
            {
                blocks = GetBlocks(method.Item);
            }
            catch (dnlib.DotNet.Emit.InvalidMethodException)
            {
                Log.Fatal("Failed to get stack height for {0}", method);
                throw;
            }

            Debug.Assert(blocks.Sum(b => b.Count) == body.Instructions.Count);
            Log.Trace("[{0}] has {1} instructions in {2} block(s)", method, body.Instructions.Count, blocks.Count);
            if (blocks.Count < 5)
            {
                // not worth it
                return;
            }

            Obfuscate(method, blocks);

            // update max stack height
            body.MaxStack = StackHelper.GetMaxStack(body);
            // The max stack is always at least 2 because of the add and modulo operations.
            Debug.Assert(body.MaxStack >= 2);
            if (!MaxStackCalculator.GetMaxStack(body.Instructions, body.ExceptionHandlers, out _))
            {
                body.KeepOldMaxStack = true;
            }

            _ = Interlocked.Increment(ref _counter);
            Log.Trace("[{0}] has been obfuscated", method);
        });

        Debug.Assert(result.IsCompleted);
        Log.Info("Obfuscated {0} methods", _counter);
    }

    private static List<List<Instruction>> GetBlocks(dnlib.DotNet.MethodDef method)
    {
        var body = method.Body;
        var instr = body.Instructions.ToList();
        var stackHeights = StackHelper.GetStackHeights(method.Body);
        // Split instructions into blocks on every instruction with stack height 0.
        List<List<Instruction>> blocks = [];
        int j = instr.Count;
        for (int i = instr.Count - 1; i >= 0; i--)
        {
            if (stackHeights[i] != 0)
                continue;
            var range = instr.GetRange(i, j - i);
            blocks.Add(range);
            j = i;
        }
        blocks.Reverse();

        Debug.Assert(blocks.Count > 0, "No blocks"); // shouldn't happen since method bodies cannot be empty
        Debug.Assert(j == 0, $"Didn't finish ({j} != 0)");
        int l = blocks.Sum(b => b.Count);
        Debug.Assert(l == instr.Count, $"Length mismatch ({l}/{instr.Count})");

        // merge blocks that are part of the same try-catch block. This is necessary to avoid breaking the exception handling.
        CombineExceptionBlocks(blocks, body);
        // merge small blocks to avoid having too many blocks
        Compress(blocks);
        _ = blocks.RemoveAll(b => b.Count == 0);
        return blocks;
    }

    private void AddPadding(IMethodData method)
    {
        var body = method.Item.Body;
        foreach (var eh in body.ExceptionHandlers)
        {
            int iStart = body.Instructions.IndexOf(eh.TryStart);
            if (iStart <= 0 || body.Instructions[iStart - 1].OpCode != OpCodes.Nop)// Don't add padding if the block is already a no-op
            {
                Log.Trace("Adding padding to ExceptionHandler TryStart ({0}) in [{1}]", eh.TryStart, method);
                body.Instructions.Insert(iStart, Emit.NoOp());
            }

            int iEnd = body.Instructions.IndexOf(eh.HandlerEnd);
            if (eh.HandlerEnd is { OpCode.Code: Code.Nop })
            {
                continue;
            }
            if (iEnd != -1)
            {
                if (body.ExceptionHandlers.Where(e2 => e2 != eh).Any(e2 => eh.HandlerEnd == e2.HandlerStart || eh.HandlerEnd == e2.FilterStart))
                {
                    // multiple handlers in a row => no padding
                    continue;
                }
                Log.Trace("Adding padding to ExceptionHandler HandlerEnd ({0}) in [{1}]", eh.HandlerEnd, method);
                var end = Emit.NoOp();
                body.Instructions.Insert(iEnd, end);
                eh.HandlerEnd = end;
            }
            else
            {
                throw new NotSupportedException(method.FullName);
            }
        }
        body.Instructions.Optimize();
    }

    private static void CombineExceptionBlocks(List<List<Instruction>> blocks, CilBody body)
    {
        var instr = body.Instructions;

        foreach (var eh in body.ExceptionHandlers)
        {
            var start = instr[instr.IndexOf(eh.TryStart) - 1];
            //var end = (eh.HandlerEnd is null) ? instr[^1] : instr[instr.IndexOf(eh.HandlerEnd) - 1];
            Debug.Assert(start.OpCode == OpCodes.Nop);
            var end = eh.HandlerEnd;
            // Debug.Assert(end.OpCode == OpCodes.Nop);
            var startIndex = blocks.FindIndex(b => b.Contains(start));
            var endIndex = blocks.FindIndex(b => b.Contains(end));
            Debug.Assert(startIndex <= endIndex, $"Exception handler block misaligned? {startIndex} <= {endIndex}");
            if (startIndex == endIndex)
            {
                continue;
            }
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                blocks[startIndex].AddRange(blocks[i]);
                blocks[i].Clear();
            }
        }
    }

    /// <summary>
    /// Merges blocks that are too small e.g. a single NOP instruction (happens a lot in debug builds)
    /// </summary>
    private static void Compress(List<List<Instruction>> blocks)
    {
        for (int i = blocks.Count - 1; i > 1; i--)
        {
            if (IsSmallBlock(blocks[i]))
            {
                blocks[i - 1].AddRange(blocks[i]);
                blocks[i].Clear();
            }
        }

        if (blocks.Count > 1 && IsSmallBlock(blocks[0]))
        {
            blocks[1].InsertRange(0, blocks[0]);
            blocks[0].Clear();
        }
    }

    private static bool IsSmallBlock(List<Instruction> block)
    {
        // A block is considered small if it contains only a single instruction that is not a NOP.
        return block.Count(i => i.OpCode.Code != Code.Nop) < 2;
    }

    /// <summary>
    /// This is where the magic happens.
    /// </summary>
    private static void Obfuscate(IMethodData method, List<List<Instruction>> blocks)
    {
        var random = new Xoshiro128(method.Name.Data); // use name as seed to be deterministic.
        var body = method.Item.Body;

        int p = MathHelper.GetPrime(blocks.Count);
        Debug.Assert(MathHelper.IsPrime(p));
        Debug.Assert(p >= blocks.Count);

        // Pad with dead code blocks until we reach blocks.Count == p.
        // We need to work with a prime number since the quotient group modulo an integer is a simple (cyclic) group. Then, we can use any positive integer as a generator for the cyclic group.
        for (int i = blocks.Count; i < p; i++)
        {
            // This code is dead because the original code bocks must already contain a return statement.
            // So we could add literally any code here (unless it crashes the JIT compiler).
            blocks.Add([]);
        }

        Debug.Assert(blocks.Count == p, "blocks.Count == p");

        // This variable will hold the current index of the code block we are executing.
        var state = body.Variables.Add(new Local(method.Item.Module.CorLibTypes.UInt32)); // uint state;

        // Use a random start value and increment for each iteration.
        uint startState = (uint)random.Next(100_000_000, int.MaxValue); // The start value can be larger than p since we use a modulo operation to trim the values. Use a large value for extra confusion.
        // The actual start index will be this value modulo p.
        Debug.Assert(startState < uint.MaxValue / 2, "startState < uint.MaxValue / 2"); // must be at most half the max value for an uint to avoid overflow.

        // inc must be a relative prime to p. That way, "inc" is a generator of the cyclic group ℤ/pℤ. Then we can use it to transition to all states in [0, p] by repeatedly adding values congruent to "inc".
        // Since p is a prime, any integer that is not a multiple of p is a relative prime to p (because p is prime).
        // The base increment should be in [2, p-2] to make the transition between states less obvious.
        Debug.Assert(p > 3, "p > 3"); // this will not work for p = 2 or p = 3.
        uint inc = (uint)random.Next(2, p - 1);

        // adjust the order of the blocks to match the transition between values that the state variable will go through.
        MathHelper.ReOrder(blocks, p, startState, inc);

        // generate the jump table to the first instruction of each block.
        var r_table = blocks.Where(b => b.Count > 0).Select(b => b[0]).ToList();
        var table = blocks.Select((b, i) => b.Count > 0 ? b[0] : r_table[i % r_table.Count]).ToList();

        var swBlock = new[]
        {
            // this block expects there to be an Int32 on the stack that in congruent to "inc" modulo p.
            Emit.Load(state),
            Emit.Add(), // Add the increment to the current state.
            Emit.Load(p),
            Emit.Rem_Un(), // unsigned modulo with p.
            Emit.Duplicate(),
            Emit.Store(state), // Store the value for next time
            Emit.Switch(table), // Perform the jump to the next block

            // equivalent C# code:
            // switch (state = (inc + state) % p)

            // This is dead code. The state value will never be larger that the jump table.
            // But the JIT compiler doesn't know that, so we need to put an instruction here that will transition to an instruction with stack height 0.
            Emit.Goto(table[random.Next(table.Count)]) // jump to random block
        };

        // now we need to deal with the existing branch instructions in the original code.
        // If the code transitions to a different block without going over the switch block, the index variable would be out of sync with the current block, changing the behavior of the code.
        List<Instruction[]> extraBlocks = CreateJumpRedirects(blocks, random, p, state);

        // now add the unconditional jumps to the end of each block to transition to the switch block.
        foreach (var block in blocks)
        {
            if (block.Count > 0 && !block[^1].OpCode.FlowControl.IsUnconditional()) // If the block ends in an unconditional jump (e.g. BR, RETURN, THROW) there is no point in adding (dead) code here. Decompilers would just ignore it.
            {
                // All values congruent to "inc" are effectively equivalent. Use a random one for each block for extra confusion.
                // put the value on the stack
                var loadInstr = Emit.Load(MathHelper.FindCongruent(inc, p, random));

                if (block[^1].OpCode == OpCodes.Nop)
                {
                    // If the block ends in a NOP, replace it with the jump to the switch block.
                    block[^1].Replace(loadInstr);
                }
                else
                {
                    block.Add(loadInstr);
                }
                // then jump back to the switch block
                block.Add(Emit.Goto(swBlock[0]));
            }
        }

        var inst = body.Instructions;
        // Now replace the instructions list of the method body.
        inst.Clear();
        // initialize the index state
        inst.Add(Emit.Load(startState));
        inst.Add(Emit.Store(state));
        // load the first increment value and add the switch
        inst.Add(Emit.Load(MathHelper.FindCongruent(inc, p, random)));
        inst.AddRange(swBlock);

        // Since all blocks end with unconditional jumps, we can randomize their order for extra confusion.
        IList<Instruction>[] allBlocks = [.. blocks, .. extraBlocks];
        random.Shuffle(allBlocks.AsSpan());

        foreach (var block in allBlocks)
        {
            inst.AddRange(block);
        }

        // Update the opcodes e.g. replace br with br.s and vice verse depending on the instruction offset.
        inst.Optimize();
    }

    private static List<Instruction[]> CreateJumpRedirects(List<List<Instruction>> blocks, Random random, int p, Local state)
    {
        List<Instruction[]> extraBlocks = [];
        foreach (var block in blocks)
        {
            foreach (var instr in block.Where(i => i.IsBranch()))
            {
                if (instr.OpCode == OpCodes.Switch)
                {
                    var table = (IList<Instruction>)instr.Operand;
                    Debug.Assert(table is not null);

                    for (int i = 0; i < table!.Count; i++)
                    {
                        Instruction target = table[i]; // target instruction to jump to
                        var newBlock = CreateJumpBlock(target, block, blocks, random, p, state);
                        if (newBlock != null)
                        {
                            table[i] = newBlock[0];
                            extraBlocks.Add(newBlock);
                        }
                    }
                }
                else
                {
                    var target = (Instruction)instr.Operand; // target instruction to jump to
                    Debug.Assert(target is not null);

                    // instead of jumping directly to the target, jump to a new block that will update the state index, and then jump to the intended target.
                    var newBlock = CreateJumpBlock(target!, block, blocks, random, p, state);
                    if (newBlock != null)
                    {
                        instr.Operand = newBlock[0]; // Do not replace the instruction. Only update its operand in case a different instruction or exception handler has a reference to it.
                        extraBlocks.Add(newBlock);
                    }
                }
            }
        }

        return extraBlocks;
    }

    private static Instruction[]? CreateJumpBlock(Instruction target, List<Instruction> currentBlock, List<List<Instruction>> blocks, Random random, int p, Local state)
    {
        var targetBlock = blocks.Single(bl => bl.Contains(target)); // block that contains the target
        if (targetBlock == currentBlock)
        {
            return null; // no cross-block jump => Ignore
        }
        var blockIndex = blocks.IndexOf(targetBlock);
        // The new value for the index state must be the index of the target block.
        // Use a value that is congruent to that index for extra confusion.
        var newState = MathHelper.FindCongruent((uint)blockIndex, p, random);

        // instead of jumping directly to the target, jump to a new block that will update the state index, and then jump to the intended target.
        return
        [
            Emit.Load(newState),
            Emit.Store(state),
            Emit.Goto(target),
        ];
    }
}
