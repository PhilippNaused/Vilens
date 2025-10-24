using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vilens.Helpers;

/// <summary>
/// Xoshiro128**
/// <see href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro128StarStarImpl.cs"/>
/// </summary>
internal sealed class Xoshiro128 : Random
{
    private uint _s0, _s1, _s2, _s3;

    public Xoshiro128(int seed)
    {
        _s0 = (uint)seed;
        _s1 = 1;
        _s2 = 1;
        _s3 = 1;
        _ = NextUInt32();
    }

    public override int Next()
    {
        while (true)
        {
            // Get top 31 bits to get a value in the range [0, int.MaxValue], but try again
            // if the value is actually int.MaxValue, as the method is defined to return a value
            // in the range [0, int.MaxValue).
            uint result = NextUInt32() >> 1;
            if (result != int.MaxValue)
            {
                return (int)result;
            }
        }
    }

    public override int Next(int maxValue)
    {
        Debug.Assert(maxValue >= 0);

        return (int)NextUInt32((uint)maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        Debug.Assert(minValue <= maxValue);

        return (int)NextUInt32((uint)(maxValue - minValue)) + minValue;
    }

    public override void NextBytes(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public override double NextDouble()
    {
        throw new NotImplementedException();
    }

    protected override double Sample()
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint NextUInt32(uint maxValue)
    {
        ulong randomProduct = (ulong)maxValue * NextUInt32();
        uint lowPart = (uint)randomProduct;

        if (lowPart < maxValue)
        {
            uint remainder = unchecked(0u - maxValue) % maxValue;

            while (lowPart < remainder)
            {
                randomProduct = (ulong)maxValue * NextUInt32();
                lowPart = (uint)randomProduct;
            }
        }

        return (uint)(randomProduct >> 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

    /// <summary>Produces a value in the range [0, uint.MaxValue].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint NextUInt32()
    {
        uint s0 = _s0, s1 = _s1, s2 = _s2, s3 = _s3;
        uint result = RotateLeft(s1 * 5, 7) * 9;
        uint t = s1 << 9;

        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;

        s2 ^= t;
        s3 = RotateLeft(s3, 11);

        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;

        return result;
    }
}
