using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "ControlFlow")]
public class ControlFlowClass2
{
    public static IEnumerable<int> Test1(IList<int> list)
    {
        yield return 0;
        for (int i = 0; i < list.Count; i++)
        {
            yield return list[i];
        }
        yield return int.MaxValue;
    }
}
