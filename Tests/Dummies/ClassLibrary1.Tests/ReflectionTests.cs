namespace ClassLibrary1.Tests;

internal sealed class ReflectionTests
{
    private static readonly Type type = typeof(ReflectionTest);

    [Test]
    public void Test1()
    {
        Assert.That(type.GetMethod("ReflectedMethod1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(new ReflectionTest().PublicMethod1(), Is.EqualTo(7));
    }

    [Test]
    public void Test2()
    {
        Assert.That(type.GetMethod("ReflectedMethod2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(new ReflectionTest().PublicMethod2(), Is.EqualTo(8));
    }
}
