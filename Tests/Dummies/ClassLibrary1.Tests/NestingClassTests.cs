namespace ClassLibrary1.Tests;

public static class NestingClassTests
{
    [Test]
    public static void Test1()
    {
        Assert.That(NestingClass.Get1(), Is.EqualTo(23));
    }

    [Test]
    public static void Test2()
    {
        Assert.That(NestingClass.Get2(), Is.EqualTo(78));
    }
}
