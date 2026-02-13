using BenchmarkDotNet.Attributes;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Vilens.Helpers;

#pragma warning disable CA1515 // Consider making public types internal

namespace Vilens.Benchmark;

[MediumRunJob, MemoryDiagnoser]
public class MaxStackBench
{
    private readonly MethodDef method;

    public MaxStackBench()
    {
        var module = ModuleDefMD.Load(typeof(Scrambler).Module);
        module.LoadEverything();
        method = module.FindNormalThrow("Vilens.Features.ControlFlow").FindMethod("Obfuscate");
        if (method.Body.Instructions.Count != 310 || method.Body.ExceptionHandlers.Count != 3)
        {
            throw new InvalidOperationException($"{method.Body.Instructions.Count} {method.Body.ExceptionHandlers.Count}");
        }
    }

    [Benchmark(Baseline = true, Description = "dnlib")]
    public uint Old()
    {
        return MaxStackCalculator.GetMaxStack(method.Body.Instructions, method.Body.ExceptionHandlers);
    }

    [Benchmark(Description = "Vilens")]
    public ushort New()
    {
        return StackHelper.GetMaxStack(method.Body);
    }
}
