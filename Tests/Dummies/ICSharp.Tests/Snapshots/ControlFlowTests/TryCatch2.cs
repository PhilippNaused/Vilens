public static void TryCatch2(System.Collections.Generic.IList<int> list)
{
    try
    {
        for (int i = 0; i < 12; i++)
        {
            list.Add(i);
        }
    }
    catch (System.InvalidOperationException ex) when (ex.InnerException != null)
    {
        list.Add(ex.Message.Length);
    }
}