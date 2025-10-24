using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public class FieldsClass
{
    public int Field1;
    internal int Field2;
    protected int Field3;
    protected internal int Field4;
    private protected int Field5;
    private int Field6;
}
