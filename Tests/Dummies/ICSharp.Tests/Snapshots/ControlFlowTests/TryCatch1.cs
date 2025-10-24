using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static void TryCatch1(IList<int> list)
{
    uint num = 100282826u;
    int num2 = 1965290142;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 1u:
            if (num3 >= 12)
            {
                return;
            }
            num = 1604418702u;
            goto case 2u;
        case 4u:
            num3++;
            num2 = 1380354022;
            break;
        case 2u:
            try
            {
                list.Add(num3);
            }
            catch (InvalidOperationException ex) when (ex.InnerException != null)
            {
                list.Add(ex.Message.Length);
                goto IL_00b2;
            }
            catch (TaskCanceledException)
            {
                list.Add(num3 + 7);
                goto IL_003f;
            }
            num = 920394319u;
            goto case 4u;
        default:
            num = 1501102081u;
            goto case 1u;
        case 3u:
            {
                num3 = 0;
                num2 = 986371727;
                break;
            }
            IL_003f:
            num = 404428996u;
            return;
            IL_00b2:
            num = 675159401u;
            return;
        }
    }
}