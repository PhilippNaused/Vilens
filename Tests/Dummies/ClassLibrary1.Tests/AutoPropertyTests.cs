namespace ClassLibrary1.Tests;

internal sealed class AutoPropertyTests
{
    [Test]
    public void Test1()
    {
        var obj = new AutoProperties();
        Assert.That(obj.Property1, Is.Zero);
        obj.Test1();
        Assert.That(obj.Property1, Is.EqualTo(1));
        Assert.That(AutoProperties.Property2, Is.EqualTo(2));
        Assert.That(obj.Test2(), Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }));
    }

    [Test]
    public void Test2()
    {
        var obj = new AutoProperties();
        Assert.That(obj.Property1, Is.Zero);
        var obj2 = new AutoProperties2(obj);
        obj2.Test1();
        Assert.That(obj.Property1, Is.EqualTo(1));
        Assert.That(AutoProperties.Property2, Is.EqualTo(2));
        Assert.That(obj2.Test2(), Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }));
    }
}
