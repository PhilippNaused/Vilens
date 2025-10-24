using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "Trimming;AttributeCleaning")]
public class PropertiesClass2
{
    public int Prop1 { get; set; }
    internal int Prop2 { get; set; }
    protected int Prop3 { get; set; }
    protected internal int Prop4 { get; set; }
    private protected int Prop5 { get; set; }
    private int Prop6 { get; set; }

    public void Use()
    {
        Prop1++;
        Prop2++;
        Prop3++;
        Prop4++;
        Prop5++;
        Prop6++;
    }
}
