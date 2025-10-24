using System.Reflection;

#pragma warning disable

namespace ClassLibrary1;

public class ReflectionTest
{
    private const string name2 = nameof(ReflectedMethod2);
    private static readonly string name1 = nameof(ReflectedMethod1);

    public int PublicMethod1()
    {
        return (int)GetType().GetMethod(name1, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, []);
    }

    private int ReflectedMethod1()
    {
        return 7;
    }

    public int PublicMethod2()
    {
        return (int)GetType().GetMethod(name2, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, []);
    }

    private int ReflectedMethod2()
    {
        return 8;
    }
}
