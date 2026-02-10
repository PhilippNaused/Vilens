using System.Reflection;
using ICSharpCode.Extension;
using NUnit.Framework;
using VeriGit;
using Vilens.Lab;

namespace TestProject1;

public class UnitTest1
{
    [Test]
    public void Test1()
    {
        Assert.That(Enum.Parse<Class1>("Test123").ToString(), Is.Not.Null);
        Assert.That(Class1.Test2, Is.EqualTo(7));
    }

    [Test]
    public void Test2()
    {
        var x = new Class3()
        {
            Value = 7
        };
        Assert.That(x.Value, Is.EqualTo(7));
    }

    [Test]
    public unsafe void Compression()
    {
        var x = "Hello, World!, Hello, World!, Hello, World!"u8;
        Span<byte> y = stackalloc byte[100];
        Span<byte> z = stackalloc byte[100];
        fixed (byte* px = x)
        fixed (byte* py = y)
        fixed (byte* pz = z)
        {
            Class4b.Compress(px, x.Length, py, 100);
            Class4b.Decompress(py, 100, pz, 100);
            var s = new string((sbyte*)pz, 0, x.Length);
            Assert.That(s, Is.EqualTo("Hello, World!, Hello, World!, Hello, World!"));
        }
        var hex = Convert.ToHexString(y);
        Assert.That(hex, Is.EqualTo("F348CDC9C9D75108CF2FCA4951D451F0C0C305" + new string('0', 162)));
    }

    [Test]
    public void Test4()
    {
        string str = Class4.Get();
        // cspell:ignore Invokÿǿ
        Assert.That(str, Is.EqualTo("_Invokÿǿ"));
    }

    [Test]
    public void Test5()
    {
        List<int> list = [1, 3, 5, 7, 9];
        var i = Class5.Test(list);
        Assert.That(i, Is.EqualTo(7));

        list = [1, 3, 5, 9];
        Assert.That(() => Class5.Test(list), Throws.InvalidOperationException);
    }

    [Test]
    public void Test5Decompiled()
    {
        var method = typeof(Class5).GetAllMethods().Single(m => m.Name == nameof(Class5.Test));
        var code = method.Decompile();
        Validation.Validate(code, "cs");
    }

    [Test]
    public void Test5Decompiled2()
    {
        var type = typeof(Class5).GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Single();
        var code = type.Decompile();
        Validation.Validate(code, "cs");
    }
}
