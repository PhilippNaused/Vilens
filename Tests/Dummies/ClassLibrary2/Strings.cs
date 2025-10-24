#pragma warning disable

using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "StringHiding")]
public class Strings
{
    public static string Field1 = "_Field1";
    public string Field2 = "_Field2";
    public static string Prop1 { get; } = "_Prop1";
    public string Prop2 { get; } = "_Prop2";
    public static string Prop3 => "_Prop3";
    public string Prop4 => "_Pr\0p4";

    public string Invoke1()
    {
        return "_Invoke1";
    }

    public string Invokeÿǿ()
    {
        return "_Invokeÿǿ";
    }
}
