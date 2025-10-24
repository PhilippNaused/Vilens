using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "Corruption")]
public static class CorruptionClass
{
    public static unsafe long Test()
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
}
