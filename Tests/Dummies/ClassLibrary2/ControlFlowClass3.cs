using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "ControlFlow")]
public class ControlFlowClass3
{
    public static async Task<int> Test1(IList<int> list)
    {
        await Task.Yield();
        list.Add(0);
        await Task.Delay(1);
        list.Add(1);
        await Task.Delay(2);
        list.Add(2);
        return list.Count;
    }
}
