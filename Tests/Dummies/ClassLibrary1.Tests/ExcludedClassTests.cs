namespace ClassLibrary1.Tests;

internal sealed class ExcludedClassTests
{
    private static readonly Type type = typeof(ExcludedClass);

    [Test]
    public void Test1()
    {
        Assert.That(type.GetMethod("ExcludedMethod1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetMethod("ExcludedMethod2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetMethod("ExcludedMethod3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetMethod("ExcludedMethod4", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
    }

    [Test]
    public void Test2()
    {
        Assert.That(type.GetField("ExcludedField1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetField("ExcludedField2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetField("ExcludedField3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetField("ExcludedField4", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
    }
}

internal sealed class ExcludedClass2Tests
{
    private static readonly Type type = typeof(ExcludedClass2);

    [Test]
    public void Test1()
    {
        Assert.That(type.GetMethod("ExcludedMethod1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetMethod("ExcludedMethod2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetMethod("ExcludedMethod3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetMethod("ExcludedMethod4", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
    }

    [Test]
    public void Test2()
    {
        Assert.That(type.GetField("ExcludedField1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetField("ExcludedField2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetField("ExcludedField3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetField("ExcludedField4", BindingFlags.NonPublic | BindingFlags.Instance), Is.Not.Null);
    }
}
