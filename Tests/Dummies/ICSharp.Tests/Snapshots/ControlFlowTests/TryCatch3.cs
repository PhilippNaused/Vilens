using System.Collections.Generic;

public static void TryCatch3(IList<int> list)
{
    uint num = 100282826u;
    int num2 = 1380354022;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 2u:
            try
            {
                list.Add(num3);
            }
            catch
            {
                goto IL_002c;
            }
            num = 920394319u;
            goto case 4u;
        case 4u:
            num3++;
            num2 = 986371727;
            break;
        case 1u:
            if (num3 >= 12)
            {
                return;
            }
            num = 1604418702u;
            goto case 2u;
        default:
            num = 1501102081u;
            goto case 1u;
        case 3u:
            {
                num3 = 0;
                num2 = 1667420382;
                break;
            }
            IL_002c:
            num = 675159404u;
            goto case 4u;
        }
    }
}