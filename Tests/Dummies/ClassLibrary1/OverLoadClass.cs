#pragma warning disable

namespace ClassLibrary1;

public class OverLoadClass
{
    private int Field1 = default;
    private long Field2 = default;
    private short Field3 = default;
    private object Field4 = default;

    private int OverLoad1() => 1;
    private int OverLoad2(int i) => 2;
    private long OverLoad3() => 3;
    private static int OverLoad4() => 4;
    private int OverLoad5<T>() => 5;

    private int OverLoad6() => 6;
    private int OverLoad7(int i) => 7;
    private long OverLoad8() => 8;
    private static int OverLoad9() => 9;
    private int OverLoad10<T>() => 10;

    public int Public1() => OverLoad1();
    public int Public2() => OverLoad2(default);
    public int Public3() => (int)OverLoad3();
    public int Public4() => OverLoad4();
    public int Public5() => OverLoad5<int>();

    public int Public6() => OverLoad6();
    public int Public7() => OverLoad7(default);
    public int Public8() => (int)OverLoad8();
    public int Public9() => OverLoad9();
    public int Public10() => OverLoad10<int>();
}

public class OverLoadClass2<T>
{
    private int Field1 = default;
    private long Field2 = default;
    private short Field3 = default;
    private object Field4 = default;

    private int OverLoad1() => 1;
    private int OverLoad2(int i) => 2;
    private long OverLoad3() => 3;
    private static int OverLoad4() => 4;
    private int OverLoad5<T2>() => 5;

    private int OverLoad6() => 6;
    private int OverLoad7(int i) => 7;
    private long OverLoad8() => 8;
    private static int OverLoad9() => 9;
    private int OverLoad10<T2>() where T2 : notnull => 10;

    public int Public1() => OverLoad1();
    public int Public2() => OverLoad2(default);
    public int Public3() => (int)OverLoad3();
    public int Public4() => OverLoad4();
    public int Public5() => OverLoad5<int>();

    public int Public6() => OverLoad6();
    public int Public7() => OverLoad7(default);
    public int Public8() => (int)OverLoad8();
    public int Public9() => OverLoad9();
    public int Public10() => OverLoad10<int>();
}
