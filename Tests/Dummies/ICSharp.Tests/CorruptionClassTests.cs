using ICSharpCode.Decompiler;
using ICSharpCode.Extension;

namespace ClassLibrary2.Tests;

[TestFixture]
public class CorruptionClassTests
{
    [Test]
    public static void Decompile()
    {
        Assert.That(typeof(CorruptionClass).Decompile, Throws.TypeOf<DecompilerException>().With.InnerException.TypeOf<InvalidCastException>());
    }
}
