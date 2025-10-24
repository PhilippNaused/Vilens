#pragma warning disable

namespace ClassLibrary1;

public class SwitchClass
{
    public string Method1(ulong i)
    {
        switch (i % 1333UL)
        {
            case 1:
                return "1";
            case 2:
                return "2";
            case 3:
                return "3";
            default:
                return "error";
        }
    }

    public string Method2(int i)
    {
        return i switch
        {
            1 => "1",
            2 => "2",
            3 => "3",
            _ => "error"
        };
    }
}
