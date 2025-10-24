using NUnit.Framework;
using Vilens.Helpers;

namespace Vilens.Tests;

public class NamingTests
{
    [Test]
    public void Test1([Values] NamingScheme scheme)
    {
        var naming = new NamingHelper(scheme);
        string[] names = Enumerable.Range(0, 1000).Select(_ => naming.GetNextName().String).ToArray();
        Assert.Multiple(() =>
        {
            if (scheme is NamingScheme.Invalid)
            {
                Assert.That(names[0], Is.Empty);
                Assert.That(names, Has.One.Empty);
                Assert.That(names.Skip(1), Has.None.Empty);
            }
            else
            {
                Assert.That(names, Has.None.Empty);
            }
            Assert.That(names, Is.Ordered.By(nameof(string.Length)));
            Assert.That(names, Is.Unique);
        });
    }
}
