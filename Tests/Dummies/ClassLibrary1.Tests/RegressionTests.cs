namespace ClassLibrary1.Tests;

internal sealed class RegressionTests
{
    [Test]
    public void Test1()
    {
        var obj = new RegressionClass();
        Assert.That(obj.Test1(), Is.EqualTo(8));
    }
}
