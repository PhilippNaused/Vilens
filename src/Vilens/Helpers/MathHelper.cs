using System.Diagnostics;

namespace Vilens.Helpers;

internal static class MathHelper
{
    /// <summary>
    /// Returns the smallest prime that is greater or equal to <paramref name="min"/>.
    /// </summary>
    /// <param name="min">The minimum value for the prime.</param>
    /// <returns>A prime <c>&gt;=</c> <paramref name="min"/></returns>
    public static int GetPrime(int min)
    {
        if (min < 0)
            throw new ArgumentOutOfRangeException(nameof(min));
        return min switch
        {
            0 => 2,
            1 => 2,
            2 => 2,
            3 => 3,
            4 => 5,
            5 => 5,
            6 => 7,
            7 => 7,
            8 => 11,
            9 => 11,
            10 => 11,
            11 => 11,
            12 => 13,
            13 => 13,
            14 => 17,
            15 => 17,
            16 => 17,
            17 => 17,
            _ => GetPrime2(min)
        };
    }

    private static int GetPrime2(int min)
    {
        for (int i = min; ; i++)
        {
            if (IsPrime(i))
                return i;
        }
    }

    public static bool IsPrime(int p)
    {
        if (p < 0)
            throw new ArgumentOutOfRangeException(nameof(p));
        switch (p)
        {
            case 0:
                return false;
            case 1:
                return false;
            case 2:
                return true;
            default:
                break;
        }
        int psr = (int)Math.Ceiling(Math.Sqrt(p));
        Debug.Assert(psr * psr >= p, "psr * psr >= p");
        Debug.Assert((psr - 1) * (psr - 1) < p, "psr * psr < p");
        for (int i = 2; i <= psr; i++)
        {
            if (p % i == 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <param name="list">A list with <paramref name="p"/> items</param>
    /// <param name="p">A prime</param>
    /// <param name="start">The starting value (<paramref name="start"/> &lt; <see cref="int.MaxValue"/>)</param>
    /// <param name="inc">The increment of each cycle (0 &lt; <paramref name="inc"/> &lt; <paramref name="p"/>)</param>
    public static void ReOrder<T>(IList<T> list, int p, uint start, uint inc)
    {
        if (list is null)
            throw new ArgumentNullException(nameof(list));
        if (!IsPrime(p))
            throw new ArgumentOutOfRangeException(nameof(p), "Value must be a prime.");
        if (list.Count != p)
            throw new ArgumentException($"List must have '{p}' elements", nameof(list));
        if (inc <= 0)
            throw new ArgumentOutOfRangeException(nameof(inc));
        if (inc >= p)
            throw new ArgumentOutOfRangeException(nameof(inc));
        if (start >= int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(start));

        Debug.Assert(inc % (uint)p != 0);
        var copy = new Queue<T>(list);

        uint s2 = start % (uint)p;
        uint i = start;
        do
        {
            i = (i + inc) % (uint)p;
            list[(int)i] = copy.Dequeue();
        } while (i != s2);

        Debug.Assert(list.Count == p);
    }

    /// <summary>
    /// Returns an unsigned integer that is congruent to <paramref name="i"/> modulo <paramref name="p"/>.
    /// </summary>
    /// <returns><c>x</c> such that <c>x % <paramref name="p"/> == <paramref name="i"/> % <paramref name="p"/></c>
    /// and <c>x &lt; <see cref="int.MaxValue"/></c></returns>
    public static uint FindCongruent(uint i, int p, Random random)
    {
        if (p <= 0)
            throw new ArgumentOutOfRangeException(nameof(p));

        uint ip = i % (uint)p;
        Debug.Assert(ip < p);
        uint x = (uint)random.Next(p, checked((int)(int.MaxValue - ip)));
        x -= x % (uint)p;
        Debug.Assert(x % (uint)p == 0u);
        x += ip;
        Debug.Assert(x % (uint)p == ip);
        Debug.Assert(x < int.MaxValue);
        return x;
    }
}
