using System.Runtime.CompilerServices;

public class PropertiesClass
{
    [CompilerGenerated]
    private int m_a;
    [CompilerGenerated]
    private int m_b;
    [CompilerGenerated]
    private int m_c;
    [CompilerGenerated]
    private int d;
    [CompilerGenerated]
    private int e;
    [CompilerGenerated]
    private int f;
    public int Prop1
    {
        [CompilerGenerated]
        get
        {
            return this.m_a;
        }
        [CompilerGenerated]
        set
        {
            this.m_a = value;
        }
    }
    internal int a
    {
        [CompilerGenerated]
        get
        {
            return this.m_b;
        }
        [CompilerGenerated]
        set
        {
            this.m_b = value;
        }
    }
    protected int Prop3
    {
        [CompilerGenerated]
        get
        {
            return this.m_c;
        }
        [CompilerGenerated]
        set
        {
            this.m_c = value;
        }
    }
    protected internal int Prop4
    {
        [CompilerGenerated]
        get
        {
            return d;
        }
        [CompilerGenerated]
        set
        {
            d = value;
        }
    }
    private protected int b
    {
        [CompilerGenerated]
        get
        {
            return e;
        }
        [CompilerGenerated]
        set
        {
            e = value;
        }
    }
    private int c
    {
        [CompilerGenerated]
        get
        {
            return f;
        }
        [CompilerGenerated]
        set
        {
            f = value;
        }
    }
}