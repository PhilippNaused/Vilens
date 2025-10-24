#pragma warning disable

using System.Reflection;

namespace ClassLibrary1;

[Obfuscation(Feature = "Renaming")]
public class AutoProperties
{
    public int Property1 { get; set; }
    public static int Property2 { get; set; }
    public int Property3 { get; } = 3;
    public static int Property4 { get; } = 4;
    public int Property5 => 5;
    public static int Property6 => 6;

    public void Test1()
    {
        Property1 = 1;
        Property2 = 2;
    }

    public int[] Test2()
    {
        return
        [
            Property1,
             Property2,
             Property3,
             Property4,
             Property5,
             Property6
        ];
    }
}

[Obfuscation(Feature = "Renaming")]
public class AutoProperties2
{
    private readonly AutoProperties obj;

    public AutoProperties2(AutoProperties obj)
    {
        this.obj = obj;
    }

    public void Test1()
    {
        obj.Property1 = 1;
        AutoProperties.Property2 = 2;
    }

    public int[] Test2()
    {
        return
        [
            obj.Property1,
             AutoProperties.Property2,
             obj.Property3,
             AutoProperties.Property4,
             obj.Property5,
             AutoProperties.Property6
        ];
    }
}
