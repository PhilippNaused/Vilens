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
        method = module.FindNormalThrow(typeof(Features.ControlFlow).FullName).FindMethod("Obfuscate");
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
