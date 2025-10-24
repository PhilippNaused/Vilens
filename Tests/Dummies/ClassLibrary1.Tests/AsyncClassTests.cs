namespace ClassLibrary1.Tests;

internal sealed class AsyncClassTests
{
    [Test]
    public async Task Test1()
    {
        var obj = new AsyncClass();
        Assert.That(await obj.GetNumbersPublic(), Is.EqualTo(7));
    }
}
