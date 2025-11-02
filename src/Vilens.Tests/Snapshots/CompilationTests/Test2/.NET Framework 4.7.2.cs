// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
[assembly: System.Runtime.CompilerServices.CompilationRelaxations(8)]
[assembly: System.Runtime.CompilerServices.RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: System.Diagnostics.Debuggable(System.Diagnostics.DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
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
        for (int i = 0; i < 12; i++)
        {
            Add_Proxy(list, i);
        }
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private unsafe static void Add_Proxy(System.Collections.Generic.ICollection<int> P_0, int P_1)
    {
        ((delegate*<System.Collections.Generic.ICollection<int>, int, void>)__ldvirtftn(System.Collections.Generic.ICollection<int>.Add))(P_0, P_1);
    }
}
