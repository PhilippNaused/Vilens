namespace ClassLibrary1;

public static class NestingClass
{
    public static int Get1() => Class1.Class2.Get();
    public static int Get2() => Class3.Class4.Get();

    private static class Class1
    {
        internal static class Class2
        {
            public static int Get() => 23;
        }
    }

    private static class Class3
    {
        internal static class Class4
        {
            public static int Get() => 78;
        }
    }
}
