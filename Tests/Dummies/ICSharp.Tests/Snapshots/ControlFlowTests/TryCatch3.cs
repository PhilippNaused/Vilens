public static void TryCatch3(System.Collections.Generic.IList<int> list)
{
    uint num = 1617446252u;
    int num2 = 856138073;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 2u:
            if (num3 < 12)
            {
                num = 197267041u;
                goto default;
            }
            return;
        case 4u:
            num3++;
            num2 = 567863008;
            break;
        default:
            try
            {
                list.Add(num3);
            }
            catch
            {
                goto IL_002e;
            }
            num = 1951428529u;
            goto case 4u;
        case 3u:
            num = 406038482u;
            goto case 2u;
        case 0u:
            {
                num3 = 0;
                num2 = 1769356613;
                break;
            }
            IL_002e:
            num = 1935361484u;
            goto case 4u;
        }
    }
}