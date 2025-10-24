using System.Runtime.CompilerServices;

public int Prop1
{
    [CompilerGenerated]
    get
    {
        return this.a;
    }
    [CompilerGenerated]
    set
    {
        this.a = value;
    }
}