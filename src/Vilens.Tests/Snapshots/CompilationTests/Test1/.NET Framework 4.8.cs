// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
[assembly: System.Runtime.CompilerServices.CompilationRelaxations(8)]
[assembly: System.Runtime.CompilerServices.RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: System.Diagnostics.Debuggable(System.Diagnostics.DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETFramework,Version=v4.8", FrameworkDisplayName = ".NET Framework 4.8")]
[assembly: System.Reflection.AssemblyVersion("0.0.0.0")]
[module: System.Runtime.CompilerServices.RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
    [Microsoft.CodeAnalysis.Embedded]
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal sealed class EmbeddedAttribute : System.Attribute
    {
    }
}
namespace System.Runtime.CompilerServices
{
    [Microsoft.CodeAnalysis.Embedded]
    [System.AttributeUsage(System.AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal sealed class RefSafetyRulesAttribute : System.Attribute
    {
        public readonly int Version;
        public RefSafetyRulesAttribute(int P_0)
        {
            Version = P_0;
        }
    }
}
public class ControlFlowClass
{
    public static void Loop1(System.Collections.Generic.IList<int> list)
    {
        uint num = 100233400u;
        int num2 = 1499553607;
        int num3 = default(int);
        while (true)
        {
            switch (num = (uint)(num2 + (int)num) % 5u)
            {
            case 1u:
                list.Add(num3);
                num2 = 1457620292;
                break;
            case 3u:
                num3++;
                num2 = 1676122952;
                break;
            case 2u:
                num3 = 0;
                num2 = 1442500362;
                break;
            case 4u:
                num = 873181665u;
                goto default;
            default:
                if (num3 < 12)
                {
                    num = 1132462081u;
                    goto case 1u;
                }
                return;
            }
        }
    }
}
