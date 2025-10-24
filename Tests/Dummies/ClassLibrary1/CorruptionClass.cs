using System.Reflection;

namespace ClassLibrary1;

[Obfuscation(Exclude = false, Feature = "Corruption")]
public static class CorruptionClass
{
    public static string GetReversePublic(string input) => GetReversePrivate(input);

    private static string GetReversePrivate(string input)
    {
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static unsafe long Test2()
    {
        int i = 9;
        uint u = 7;
        short s = 10;
        sbyte b = 11;
        ulong U = 12;
        nint n = 13;
        void* v = (void*)14;
        char c = 'x';
        string str = "123";
        ushort[] arr = [1, 2, 3];
        return i + u + s + b + c + (long)U + str[1] + arr[2] + n + (long)v;
    }

    public static Type Test3()
    {
        char c = 'a';
        return c.GetType();
    }

    public static Type Test4()
    {
        string s = "123";
        return s.GetType();
    }

    public static sbyte Test5()
    {
        sbyte s = 123;
        return s;
    }
}
