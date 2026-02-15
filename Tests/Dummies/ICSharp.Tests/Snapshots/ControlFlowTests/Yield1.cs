public class ControlFlowClass2
{
    public static System.Collections.Generic.IEnumerable<int> Test1(System.Collections.Generic.IList<int> list)
    {
        uint num = 1290929669u;
        int num2 = 339567531;
        int num5 = default(int);
        int i = default(int);
        int num3 = default(int);
        int num4 = default(int);
        while (true)
        {
            switch (num = (uint)(num2 + (int)num) % 23u)
            {
            case 15u:
                num5 = i;
                num2 = 348100071;
                break;
            case 18u:
                /*Error near IL_0087: Unexpected return in MoveNext()*/;
            case 22u:
                num2 = 2092502066;
                break;
            case 12u:
                yield break;
            case 9u:
                yield return 0;
                /*Error: Unable to find new state assignment for yield return*/;
            case 8u:
                i = num5 + 1;
                num2 = 21620407;
                break;
            case 11u:
                num2 = 2059005165;
                break;
            case 10u:
                try
                {
                    num2 = 683737599;
                }
                finally
                {
                    /*Error: Could not find finallyMethod for state=3.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                }
            case 3u:
                /*Error near IL_00e2: Unexpected return in MoveNext()*/;
            case 7u:
                switch (num3)
                {
                case 2:
                    goto IL_0103;
                case 0:
                    goto IL_0134;
                case 1:
                    goto IL_0180;
                case 3:
                    goto IL_018b;
                }
                num2 = 1695325669;
                break;
            case 19u:
                num2 = 1825481726;
                break;
            case 17u:
                yield return int.MaxValue;
                /*Error: Unable to find new state assignment for yield return*/;
            case 4u:
                i = 0;
                num = 1438839981u;
                goto case 1u;
            case 1u:
                if (i >= list.Count)
                {
                    num2 = 748755908;
                    break;
                }
                goto case 20u;
            case 5u:
            case 6u:
                /*Error near IL_0166: Unexpected return in MoveNext()*/;
            case 14u:
                num3 = num4;
                num2 = 1832319764;
                break;
            case 16u:
                num2 = 883426014;
                break;
            case 13u:
                try
                {
                    num2 = 1151743968;
                }
                finally
                {
                    /*Error: Could not find finallyMethod for state=2.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                }
            case 20u:
                yield return list[i];
                /*Error: Unable to find new state assignment for yield return*/;
            case 0u:
            case 21u:
                yield break;
            default:
                {
                    try
                    {
                        num2 = 747950195;
                    }
                    finally
                    {
                        /*Error: Could not find finallyMethod for state=1.
Possibly this method is affected by a C# compiler bug that causes the finally body
not to run in case of an exception or early 'break;' out of a loop consuming this iterable.*/;
                    }
                }
                IL_018b:
                num = 337821719u;
                goto case 19u;
                IL_0180:
                num = 900905434u;
                goto case 11u;
                IL_0134:
                num = 1913829119u;
                goto case 16u;
                IL_0103:
                num = 2090411257u;
                goto case 22u;
            }
        }
    }
}