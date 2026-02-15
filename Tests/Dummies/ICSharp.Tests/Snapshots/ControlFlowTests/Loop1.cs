public static void Loop1(System.Collections.Generic.IList<int> list)
{
    uint num = 100134548u;
    int num2 = 605824388;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 0u:
            num3++;
            num2 = 1940506093;
            break;
        default:
            if (num3 >= 12)
            {
                return;
            }
            num = 1028374317u;
            goto case 2u;
        case 2u:
            list.Add(num3);
            num2 = 363340603;
            break;
        case 4u:
            num = 856523373u;
            goto default;
        case 1u:
            num3 = 0;
            num2 = 534568438;
            break;
        }
    }
}