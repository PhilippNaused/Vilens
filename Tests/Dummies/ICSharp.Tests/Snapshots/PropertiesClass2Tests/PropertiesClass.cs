public class PropertiesClass2
{
    public int Prop1
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
    internal int Prop2
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
    protected int Prop3
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
    protected internal int Prop4
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
    private protected int Prop5
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
    private int Prop6
    {
        get
        {
            return field;
        }
        set
        {
            field = value;
        }
    }
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