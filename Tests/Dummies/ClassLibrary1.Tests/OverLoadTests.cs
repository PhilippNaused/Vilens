namespace ClassLibrary1.Tests;

internal sealed class OverLoadTests
{
    private static readonly Type type = typeof(OverLoadClass);

    private static readonly Type type2 = typeof(OverLoadClass);

    [Test]
    public void Test1()
    {
        var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(methods, Has.Length.EqualTo(10));
        Assert.That(methods, Has.None.Property("Name").Empty);
        Assert.That(methods, Has.All.Property("Name").Length.EqualTo(1));
    }

    [Test]
    public void Test2()
    {
        var methods = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(methods, Has.Length.EqualTo(4));
        Assert.That(methods, Has.None.Property("Name").Empty);
    }

    [Test]
    public void Test3()
    {
        var obj = new OverLoadClass();
        Assert.Multiple(() =>
        {
            Assert.That(obj.Public1(), Is.EqualTo(1));
            Assert.That(obj.Public2(), Is.EqualTo(2));
            Assert.That(obj.Public3(), Is.EqualTo(3));
            Assert.That(obj.Public4(), Is.EqualTo(4));
            Assert.That(obj.Public5(), Is.EqualTo(5));
            Assert.That(obj.Public6(), Is.EqualTo(6));
            Assert.That(obj.Public7(), Is.EqualTo(7));
            Assert.That(obj.Public8(), Is.EqualTo(8));
            Assert.That(obj.Public9(), Is.EqualTo(9));
            Assert.That(obj.Public10(), Is.EqualTo(10));
        });
    }

    [Test]
    public void Test4()
    {
        var methods = type2.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(methods, Has.Length.EqualTo(10));
        Assert.That(methods, Has.None.Property("Name").Empty);
        Assert.That(methods, Has.All.Property("Name").Length.EqualTo(1));
    }

    [Test]
    public void Test5()
    {
        var methods = type2.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(methods, Has.Length.EqualTo(4));
        Assert.That(methods, Has.None.Property("Name").Empty);
    }

    [Test]
    public void Test6()
    {
        var obj = new OverLoadClass2<int>();
        Assert.Multiple(() =>
        {
            Assert.That(obj.Public1(), Is.EqualTo(1));
            Assert.That(obj.Public2(), Is.EqualTo(2));
            Assert.That(obj.Public3(), Is.EqualTo(3));
            Assert.That(obj.Public4(), Is.EqualTo(4));
            Assert.That(obj.Public5(), Is.EqualTo(5));
            Assert.That(obj.Public6(), Is.EqualTo(6));
            Assert.That(obj.Public7(), Is.EqualTo(7));
            Assert.That(obj.Public8(), Is.EqualTo(8));
            Assert.That(obj.Public9(), Is.EqualTo(9));
            Assert.That(obj.Public10(), Is.EqualTo(10));
        });
    }
}
