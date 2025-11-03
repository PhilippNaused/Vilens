using NUnit.Framework;
using Vilens.Data;

namespace Vilens.Tests;

public class VilensFeatureTests
{
    [Test]
    [TestCase("", VilensFeature.None)] // empty string
    [TestCase("  ,;\n \r", VilensFeature.None)] // whitespace only
    [TestCase("none", VilensFeature.None)]
    [TestCase("All", VilensFeature.All)]
    [TestCase("renaming", VilensFeature.Renaming)]
    [TestCase("Renaming;controlFlow", VilensFeature.Renaming | VilensFeature.ControlFlow)]
    public void TestParser(string text, VilensFeature result)
    {
        var parsed = FeatureExtensions.Parse(text);
        Assert.That(parsed, Is.EqualTo(result));
    }
}
