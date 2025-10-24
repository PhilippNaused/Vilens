using ICSharpCode.Extension;
using NUnit.Framework;
using VeriGit;
using Vilens.Data;

namespace Vilens.Tests;

internal static class Utils
{
    public static string TestObfuscate(string sourceCode, out string framework, VilensFeature features)
    {
        var data = CompilerUtils.Compile(sourceCode, out var pdbData, out framework);
        var settings = new VilensSettings
        {
            Features = features,
            Scope = Visibility.Public
        };
        Scrambler.Scramble(data: data,
            pdbData: pdbData,
            settings,
            newData: out var newData,
            newPdbData: out _,
            TestContext.CurrentContext.CancellationToken);
        var decomp = ICSharpCodeExtensions.GetDecompiler(newData, "Test.dll").GetCode();
        return decomp;
    }

    public static void Test(string sourceCode, Action<Scrambler> action, bool cil = false)
    {
        var data = CompilerUtils.Compile(sourceCode, out var pdbData, out var framework);
        var settings = new VilensSettings
        {
            Features = VilensFeature.All,
            Scope = Visibility.Public
        };
        var scrambler = new Scrambler(data, pdbData, settings, TestContext.CurrentContext.CancellationToken);
        action(scrambler);
        scrambler.Save(out var newData, out _, false, null);
        var decompiler = ICSharpCodeExtensions.GetDecompiler(newData, "Test.dll");
        using var x = Assert.EnterMultipleScope();
        if (cil)
        {
            Validation.Validate(decompiler.Disassemble(), "il", framework);
        }
        Validation.Validate(decompiler.GetCode(), "cs", framework);
    }

    public static void Validate(string sourceCode, VilensFeature features)
    {
        string decomp = TestObfuscate(sourceCode, out var framework, features);
        Validation.Validate(decomp, "cs", framework);
    }
}
