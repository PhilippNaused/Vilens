using ClassLibrary2;
using ICSharpCode.Extension;
using VeriGit;

namespace ICSharp.Tests;

[TestFixture, Parallelizable(ParallelScope.All)]
internal static class PropertiesClass2Tests
{
    [Test]
    public static void PropertiesClass()
    {
        var str = typeof(PropertiesClass2).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void PropertiesClass2()
    {
        Assert.That(typeof(PropertiesClass2).Disassemble, Throws.Nothing);
    }

    [Test, Ignore("Not implemented")]
    public static void PropertiesClass3()
    {
        var names = typeof(PropertiesClass2).GetAllProperties().Select(p => p.Name).ToList();
        Assert.Multiple(delegate
        {
            Assert.That(names, Has.Count.EqualTo(3));
            Assert.That(names[0], Is.EqualTo("Prop1"));
            Assert.That(names[1], Is.EqualTo("Prop3"));
            Assert.That(names[2], Is.EqualTo("Prop4"));
        });
    }
}
