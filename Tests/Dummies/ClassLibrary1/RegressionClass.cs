using System.Reflection;

namespace ClassLibrary1;

public class RegressionClass
{
    /// <summary>
    /// Regression for workaround of <see href="https://github.com/0xd4d/dnlib/issues/550"/>
    /// </summary>
    public int Test1()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var members = GetType().GetMembers(flags);
        return members.Length;
    }
}
