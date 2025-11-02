public class ControlFlowClass2
{
    public static System.Collections.Generic.IEnumerable<int> Test1(System.Collections.Generic.IList<int> list)
    {
        uint num = 100192212u;
        int num2 = 375760892;
        int i = default(int);
        int num3 = default(int);
        int num4 = default(int);
        int num5 = default(int);
        while (true)
        {
            switch (num = (uint)(num2 + (int)num) % 23u)
            {
            case 0u:
                yield return list[i];
                /*Error: Unable to find new state assignment for yield return*/;
            case 3u:
            case 5u:
                num3 = num4;
                num2 = 292008071;
                break;
            case 18u:
                /*Error near IL_00b6: Unexpected return in MoveNext()*/;
            case 12u:
                if (i >= list.Count)
                {
                    num2 = 872674006;
                    break;
                }
                goto case 0u;
            case 4u:
                /*Error near IL_00d5: Unexpected return in MoveNext()*/;
            case 22u:
                yield break;
            case 21u:
                i = 0;
                num = 1079264846u;
                goto case 12u;
            case 11u:
                num2 = 481525288;
                break;
            default:
                try
                {
                    num2 = 2079632632;
                }
                finally
                {
                    /*Error: Could not find finallyMethod for state=2.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                }
            case 17u:
                /*Error near IL_010e: Unexpected return in MoveNext()*/;
            case 13u:
                yield return 0;
                /*Error: Unable to find new state assignment for yield return*/;
            case 15u:
                try
                {
                    num2 = 1416034010;
                }
                finally
                {
                    /*Error: Could not find finallyMethod for state=1.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                }
            case 8u:
                num5 = i;
                num2 = 139756696;
                break;
            case 14u:
                yield return int.MaxValue;
                /*Error: Unable to find new state assignment for yield return*/;
            case 16u:
                try
                {
                    num2 = 585965160;
                }
                finally
                {
                    /*Error: Could not find finallyMethod for state=3.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                }
            case 19u:
                num2 = 1463793004;
                break;
            case 20u:
                num2 = 407400980;
                break;
            case 6u:
                num2 = 1674538784;
                break;
            case 9u:
                yield break;
            case 10u:
                i = num5 + 1;
                num2 = 2124247457;
                break;
            case 7u:
                {
                    switch (num3)
                    {
                    case 2:
                        break;
                    case 3:
                        goto IL_00f1;
                    case 0:
                        goto IL_018d;
                    default:
                        goto IL_01e5;
                    case 1:
                        goto IL_01ef;
                    }
                    num = 897954759u;
                    goto case 6u;
                }
                IL_01ef:
                num = 2145820531u;
                goto case 19u;
                IL_01e5:
                num2 = 395232071;
                break;
                IL_018d:
                num = 1272545276u;
                goto case 11u;
                IL_00f1:
                num = 552593696u;
                goto case 20u;
            }
        }
    }
}