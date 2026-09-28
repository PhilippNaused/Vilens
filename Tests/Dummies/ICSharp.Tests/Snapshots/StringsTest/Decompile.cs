public class Strings
{
    public static string Field1 = a.b[1];
    public string Field2 = a.b[2];
    public static string Prop1 { get; } = a.b[4];
    public string Prop2 { get; } = a.b[5];
    public static string Prop3 => a.b[6];
    public string Prop4 => a.b[7];
    public string Invoke1()
    {
        return a.b[0];
    }
    public string Invokeÿǿ()
    {
        return a.b[3];
    }
}