namespace ClassLibrary1.Tests;
internal sealed class Enum1Tests
{
    [Test]
    public static void Test1()
    {
        Assert.That((int)Enum1.Value1, Is.EqualTo(1));
        Assert.That((int)Enum1.Value2, Is.EqualTo(2));
        Assert.That((int)Enum1.Value3, Is.EqualTo(3));
        Assert.That(typeof(Enum1).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public), Has.Length.EqualTo(3));
    }
}
