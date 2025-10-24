using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public class PropertiesClass
{
    public int Prop1 { get; set; }
    internal int Prop2 { get; set; }
    protected int Prop3 { get; set; }
    protected internal int Prop4 { get; set; }
    private protected int Prop5 { get; set; }
    private int Prop6 { get; set; }
}
