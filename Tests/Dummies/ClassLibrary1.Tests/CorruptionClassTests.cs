using ICSharpCode.Decompiler;
using ICSharpCode.Extension;

namespace ClassLibrary1.Tests;

[TestFixture]
public class CorruptionClassTests
{
    [Test]
    public void Test1()
    {
        Assert.That(CorruptionClass.GetReversePublic("abc"), Is.EqualTo("cba"));
    }

    [Test]
    public static void Test2()
    {
        var x = CorruptionClass.Test2();
        Assert.That(x, Is.EqualTo(249));
    }

    [Test]
    public static void Test3()
    {
        var t = CorruptionClass.Test3();
        Assert.That(t, Is.EqualTo(typeof(char)));
    }

    [Test]
    public static void Test4()
    {
        var t = CorruptionClass.Test4();
        Assert.That(t, Is.EqualTo(typeof(string)));
    }

    [Test]
    public static void Test5()
    {
        var t = CorruptionClass.Test5();
        Assert.That(t, Is.EqualTo(123));
    }

    [Test]
    public static void Decompile()
    {
        Assert.That(typeof(CorruptionClass).Decompile, Throws.TypeOf<DecompilerException>().With.InnerException.TypeOf<InvalidCastException>());
    }
}
