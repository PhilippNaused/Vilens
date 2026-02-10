using NUnit.Framework;
using System.IO.Compression;
using System.Text;

namespace Vilens.Lab.Tests;

public static class StringHide
{
    public static string[] strings = null!;

    private static readonly byte[] data = [0, 1, .. "a"u8, 3, .. "Hi!"u8, 13, .. "Hello, World!"u8];

    public static unsafe void Initialize()
    {
        strings = new string[4];
        int index = 0;

        //var data2 = DecompressionMethods();

        fixed (byte* ptr = data)
        {
            byte* p = ptr;
            while (index < strings.Length)
            {
                int l = *p;
                p++;
                strings[index] = Encoding.UTF8.GetString(p, l);
                index++;
                p += l;
            }
        }
    }

    public static unsafe void DecompressionMethods(byte[] data2)
    {
        fixed (byte* ptr = data)
        {
            var uStream = new UnmanagedMemoryStream(ptr, data.Length);
            var dStream = new DeflateStream(uStream, CompressionMode.Decompress);
            var mStream = new MemoryStream(data2);
            dStream.CopyTo(mStream);
            dStream.Dispose();
        }
    }
}

public class StringHideTests
{
    [Test]
    public void Test()
    {
        StringHide.Initialize();
        Assert.That(StringHide.strings, Has.Length.EqualTo(4));
        Assert.That(StringHide.strings[0], Is.EqualTo(""));
        Assert.That(StringHide.strings[1], Is.EqualTo("a"));
        Assert.That(StringHide.strings[2], Is.EqualTo("Hi!"));
        Assert.That(StringHide.strings[3], Is.EqualTo("Hello, World!"));
    }
}
