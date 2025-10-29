using ClassLibrary2;
using ICSharpCode.Extension;
using VeriGit;

namespace ICSharp.Tests;

[TestFixture, Parallelizable(ParallelScope.All)]
internal static class StringsTest
{
    [Test]
    public static void Decompile()
    {
        var str = typeof(Strings).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void Values()
    {
        Assert.That(Strings.Field1, Is.EqualTo("_Field1"));
        Assert.That(Strings.Prop1, Is.EqualTo("_Prop1"));
        Assert.That(Strings.Prop3, Is.EqualTo("_Prop3"));
        var val = new Strings();
        Assert.That(val.Field2, Is.EqualTo("_Field2"));
        Assert.That(val.Prop2, Is.EqualTo("_Prop2"));
        Assert.That(val.Prop4, Is.EqualTo("_Pr\0p4"));
        Assert.That(val.Invoke1(), Is.EqualTo("_Invoke1"));
        // cspell:ignore Invokeÿǿ
        Assert.That(val.Invokeÿǿ().ToCharArray(), Has.Some.GreaterThan((char)128));
        Assert.That(val.Invokeÿǿ(), Is.EqualTo("_Invokeÿǿ"));
    }
}
