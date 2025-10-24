using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "ControlFlow")]
public class ControlFlowClass
{
    public static void Loop1(IList<int> list)
    {
        for (int i = 0; i < 12; i++)
        {
            list.Add(i);
        }
    }

    public static void TryCatch1(IList<int> list)
    {
        for (int i = 0; i < 12; i++)
        {
            try
            {
                list.Add(i);
            }
            catch (InvalidOperationException e) when (e.InnerException != null)
            {
                list.Add(e.Message.Length);
                return;
            }
            catch (TaskCanceledException)
            {
                list.Add(i + 7);
                return;
            }
        }
    }

    public static void TryCatch2(IList<int> list)
    {
        try
        {
            for (int i = 0; i < 12; i++)
            {
                list.Add(i);
            }
        }
        catch (InvalidOperationException e) when (e.InnerException != null)
        {
            list.Add(e.Message.Length);
            return;
        }
    }

    public static void TryCatch3(IList<int> list)
    {
        for (int i = 0; i < 12; i++)
        {
            try
            {
                list.Add(i);
            }
            catch
            {

            }
        }
    }
}
