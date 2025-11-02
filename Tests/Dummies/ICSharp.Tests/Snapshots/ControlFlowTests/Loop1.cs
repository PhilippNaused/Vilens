public static void Loop1(System.Collections.Generic.IList<int> list)
{
    uint num = 100271843u;
    int num2 = 1466146002;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 1u:
            num3++;
            num2 = 1600664192;
            continue;
        case 3u:
            if (num3 >= 12)
            {
                return;
            }
            num = 1408978844u;
            break;
        case 2u:
            num = 1483407358u;
            goto case 3u;
        default:
            num3 = 0;
            num2 = 1763295307;
            continue;
        case 4u:
            break;
        }
        list.Add(num3);
        num2 = 1448691052;
    }
}