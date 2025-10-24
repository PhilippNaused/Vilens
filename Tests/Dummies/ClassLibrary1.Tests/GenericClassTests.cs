namespace ClassLibrary1.Tests;

public class GenericClassTests
{
    [Test]
    public void Test1()
    {
        var obj = new GenericClass<int>();
        Assert.That(obj.PublicMethod1(), Is.EqualTo(1));
        Assert.That(obj.PublicMethod2(), Is.EqualTo(2));
        Assert.That(obj.PublicMethod3(), Is.EqualTo(3));
    }

    [Test]
    public void Test2()
    {
        var type = typeof(GenericClass<>);
        Assert.That(type.GetMethod("PublicMethod1", BindingFlags.Public | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetMethod("PublicMethod2", BindingFlags.Public | BindingFlags.Instance), Is.Not.Null);
        Assert.That(type.GetMethod("PublicMethod3", BindingFlags.Public | BindingFlags.Instance), Is.Not.Null);

        Assert.That(type.GetMethod("PrivateMethod1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetMethod("PrivateMethod2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetMethod("PrivateMethod3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);

        Assert.That(type.GetField("PrivateField1", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetField("PrivateField2", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
        Assert.That(type.GetField("PrivateField3", BindingFlags.NonPublic | BindingFlags.Instance), Is.Null);
    }
}
