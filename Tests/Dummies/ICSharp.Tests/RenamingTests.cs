using ClassLibrary2.Renaming;
using ICSharpCode.Extension;
using VeriGit;

namespace ICSharp.Tests;

[TestFixture, Parallelizable(ParallelScope.All)]
internal static class RenamingTests
{
    [Test]
    public static void MethodsClass()
    {
        var str = typeof(MethodsClass).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void PropertiesClass()
    {
        var str = typeof(PropertiesClass).Decompile();
        Validation.Validate(str, "cs");
        Assert.That(typeof(PropertiesClass).Disassemble, Throws.Nothing);
    }

    [Test]
    public static void PropertiesClass3()
    {
        var names = typeof(PropertiesClass).GetAllProperties().Select(p => p.Name).ToList();
        Assert.Multiple(delegate
        {
            Assert.That(names[0], Is.EqualTo("Prop1"));
            Assert.That(names[1], Is.EqualTo("a"));
            Assert.That(names[2], Is.EqualTo("Prop3"));
            Assert.That(names[3], Is.EqualTo("Prop4"));
            Assert.That(names[4], Is.EqualTo("b"));
            Assert.That(names[5], Is.EqualTo("c"));
        });
    }

    [Test]
    public static void PropertiesClass_Prop1()
    {
        var str = typeof(PropertiesClass).GetAllProperties()[0].Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void PropertiesClass_Prop6()
    {
        var str = typeof(PropertiesClass).GetAllProperties()[5].Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void EventClass()
    {
        var str = typeof(EventClass).Decompile();
        Validation.Validate(str, "cs");
        Assert.That(typeof(EventClass).Disassemble, Throws.Nothing);
    }

    [Test]
    public static void EventClass3()
    {
        var names = typeof(EventClass).GetAllEvents().Select(p => p.Name).ToList();
        Assert.Multiple(delegate
        {
            Assert.That(names[0], Is.EqualTo("Ev1"));
            Assert.That(names[1], Is.EqualTo("a"));
            Assert.That(names[2], Is.EqualTo("Ev3"));
            Assert.That(names[3], Is.EqualTo("Ev4"));
            Assert.That(names[4], Is.EqualTo("b"));
            Assert.That(names[5], Is.EqualTo("c"));
        });
    }

    [Test]
    public static void EventClass_Ev1()
    {
        var str = typeof(EventClass).GetAllEvents()[0].Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void EventClass_Ev6()
    {
        var str = typeof(EventClass).GetAllEvents()[5].Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void FieldsClass()
    {
        var str = typeof(FieldsClass).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void NestingClass()
    {
        var str = typeof(NestingClass).Decompile();
        Validation.Validate(str, "cs");
    }

    [Test]
    public static void EnumClass()
    {
        var str = typeof(EnumClass).Decompile();
        Validation.Validate(str, "cs");
    }
}
