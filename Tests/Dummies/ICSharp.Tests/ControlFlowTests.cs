using ClassLibrary2;
using ICSharpCode.Extension;
using VeriGit;

namespace ICSharp.Tests;
internal sealed class ControlFlowTests
{
    [Test]
    public void Loop1()
    {
        var str = typeof(ControlFlowClass).GetMethod(nameof(ControlFlowClass.Loop1))!.Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public void Yield1()
    {
        var str = typeof(ControlFlowClass2).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public void Await1()
    {
        var str = typeof(ControlFlowClass3).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public void TryCatch1()
    {
        var str = typeof(ControlFlowClass).GetMethod(nameof(ControlFlowClass.TryCatch1))!.Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public void TryCatch2()
    {
        var str = typeof(ControlFlowClass).GetMethod(nameof(ControlFlowClass.TryCatch2))!.Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public void TryCatch3()
    {
        var str = typeof(ControlFlowClass).GetMethod(nameof(ControlFlowClass.TryCatch3))!.Decompile();
        Validation.Validate(str, "cs");
    }
}
