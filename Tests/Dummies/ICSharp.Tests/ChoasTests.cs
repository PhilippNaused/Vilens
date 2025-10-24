using ClassLibrary2;
using ICSharpCode.Decompiler;
using ICSharpCode.Extension;

namespace ICSharp.Tests;

[TestFixture, Parallelizable(ParallelScope.All)]
internal static class ChaosTests
{
    [Test]
    public static void ChaosClass()
    {
        Assert.That(typeof(ChaosClass).Decompile, Throws.TypeOf<DecompilerException>().With.InnerException.TypeOf<InvalidCastException>());
    }

    [Test]
    public static void ChaosClass2()
    {
        Assert.That(typeof(ChaosClass).Disassemble, Throws.TypeOf<BadImageFormatException>().With.Message.EqualTo("Invalid compressed integer."));
    }
}
