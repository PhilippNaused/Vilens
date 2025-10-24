using NUnit.Framework;
using Vilens.Helpers;

namespace Vilens.Tests;

[TestFixture(TestOf = typeof(MathHelper)), Parallelizable(ParallelScope.All)]
internal static class MathHelperTests
{
    [Test]
    [TestCase(0, false)]
    [TestCase(1, false)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    [TestCase(4, false)]
    [TestCase(5, true)]
    [TestCase(6, false)]
    [TestCase(7, true)]
    [TestCase(8, false)]
    [TestCase(9, false)]
    [TestCase(10, false)]
    [TestCase(11, true)]
    public static void IsPrime(int p, bool isPrime)
    {
        Assert.That(MathHelper.IsPrime(p), Is.EqualTo(isPrime));
    }

    [Test]
    [TestCase(0, 2)]
    [TestCase(1, 2)]
    [TestCase(2, 2)]
    [TestCase(3, 3)]
    [TestCase(4, 5)]
    [TestCase(5, 5)]
    [TestCase(6, 7)]
    [TestCase(7, 7)]
    [TestCase(8, 11)]
    [TestCase(9, 11)]
    [TestCase(10, 11)]
    [TestCase(11, 11)]
    [TestCase(12, 13)]
    public static void GetPrime(int min, int p)
    {
        Assert.That(MathHelper.GetPrime(min), Is.EqualTo(p));
    }

    private static IEnumerable<int> GetPrimeMins()
    {
        return Enumerable.Range(0, 100);
    }

    [Test]
    [TestCaseSource(nameof(GetPrimeMins))]
    public static void GetPrime(int min)
    {
        int p = MathHelper.GetPrime(min);
        Assert.That(MathHelper.IsPrime(p), Is.True);
        Assert.That(p, Is.GreaterThanOrEqualTo(min));
        for (int i = min; i < p; i++)
        {
            Assert.That(MathHelper.IsPrime(i), Is.False);
        }
    }

    [Test]
    [TestCase(5, 0u, 2u, new[] { 4, 2, 0, 3, 1 })]
    [TestCase(5, 5u, 2u, new[] { 4, 2, 0, 3, 1 })]
    [TestCase(5, 1u, 2u, new[] { 1, 4, 2, 0, 3 })]
    public static void ReOrder3(int p, uint start, uint inc, int[] exp)
    {
        var list = Enumerable.Range(0, p).ToList();
        MathHelper.ReOrder(list, p, start, inc);
        Assert.That(list, Is.EqualTo(exp));
    }

    [Test, Pairwise]
    public static void ReOrder([Random(3, 50, 10)] int p, [Random(0u, int.MaxValue, 10)] uint start, [Random(10)] uint inc, [Random(5)] int seed)
    {
        ReOrderInt(p, start, inc, seed);
    }

    [Test]
    [TestCase(3, 1051637943u, 1538663189u, 1)]
    [TestCase(10, 1481044968u, 1211024267u, 1740080403)]
    [TestCase(3, 944156458u, 3873562041u, 2042884762)]
    public static void ReOrder2(int p, uint start, uint inc, int seed)
    {
        ReOrderInt(p, start, inc, seed);
    }

    private static void ReOrderInt(int p, uint start, uint inc, int seed)
    {
        p = MathHelper.GetPrime(p);
        inc %= (uint)p;
        if (inc % p == 0)
        {
            inc++;
        }

        var list = Enumerable.Range(0, p).ToList();
        var list2 = new List<int>(list);
        MathHelper.ReOrder(list, p, start, inc);

        uint state = start;
        var random = new Xoshiro128(seed);
        for (int i = 0; i < p; i++)
        {
            var x = MathHelper.FindCongruent(inc, p, random);
            state = (state + x) % (uint)p;
            Assert.That(list[(int)state], Is.EqualTo(list2[i]));
        }
    }

    [Test, Pairwise]
    public static void FindCongruent([Random(20)] uint i, [Random(1, 1000, 20)] int p, [Random(20)] int seed)
    {
        Assert.That(p, Is.Not.Negative);
        var x = MathHelper.FindCongruent(i, p, new Xoshiro128(seed));
        Assert.That(x, Is.LessThan(int.MaxValue));
        Assert.That(x % (uint)p, Is.EqualTo(i % (uint)p));
    }
}
