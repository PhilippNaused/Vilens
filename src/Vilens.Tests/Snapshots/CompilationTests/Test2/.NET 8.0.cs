// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyVersion("0.0.0.0")]
[module: RefSafetyRules(11)]
public class ControlFlowClass
{
    public static void Loop1(IList<int> list)
    {
        for (int i = 0; i < 12; i++)
        {
            Add_Proxy(list, i);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe static void Add_Proxy(ICollection<int> P_0, int P_1)
    {
        ((delegate*<ICollection<int>, int, void>)__ldvirtftn(ICollection<int>.Add))(P_0, P_1);
    }
}
