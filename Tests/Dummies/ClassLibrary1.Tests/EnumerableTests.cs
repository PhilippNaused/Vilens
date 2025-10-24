namespace ClassLibrary1.Tests;

internal sealed class EnumerableTests
{
    [Test]
    public void Test1()
    {
        var obj = new EnumerableClass();
        Assert.That(obj.GetNumbersPublic(), Is.EqualTo(Enumerable.Range(1, 14)));
    }
}
