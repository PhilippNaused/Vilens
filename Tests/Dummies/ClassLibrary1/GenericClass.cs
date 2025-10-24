#pragma warning disable

namespace ClassLibrary1;

public class GenericClass<T>
{
    private int PrivateField1 = 1;
    private long PrivateField2 = 2;
    private short PrivateField3 = 3;
    public int PublicMethod1() => PrivateMethod1();
    public long PublicMethod2() => PrivateMethod2();
    public short PublicMethod3() => PrivateMethod3();

    private int PrivateMethod1() => PrivateField1;
    private long PrivateMethod2() => PrivateField2;
    private short PrivateMethod3() => PrivateField3;
}
