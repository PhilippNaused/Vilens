using System;
using System.Collections.Generic;

public static void TryCatch2(IList<int> list)
{
    try
    {
        for (int i = 0; i < 12; i++)
        {
            list.Add(i);
        }
    }
    catch (InvalidOperationException ex) when (ex.InnerException != null)
    {
        list.Add(ex.Message.Length);
    }
}