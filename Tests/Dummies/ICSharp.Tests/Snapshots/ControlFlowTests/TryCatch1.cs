public static void TryCatch1(System.Collections.Generic.IList<int> list)
{
    uint num = 1617446252u;
    int num2 = 515166423;
    int num3 = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 5u)
        {
        case 2u:
            if (num3 >= 12)
            {
                return;
            }
            num = 931118676u;
            goto default;
        default:
            try
            {
                list.Add(num3);
            }
            catch (System.InvalidOperationException ex) when (ex.InnerException != null)
            {
                list.Add(ex.Message.Length);
                goto IL_0035;
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                list.Add(num3 + 7);
                goto IL_0095;
            }
            num = 1954377649u;
            break;
        case 0u:
            num3 = 0;
            num2 = 1553032828;
            continue;
        case 3u:
            num = 1733975997u;
            goto case 2u;
        case 4u:
            break;
            IL_0095:
            num = 1158543322u;
            return;
            IL_0035:
            num = 1947163727u;
            return;
        }
        num3++;
        num2 = 1369362513;
    }
}