// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
[assembly: System.Runtime.CompilerServices.CompilationRelaxations(8)]
[assembly: System.Runtime.CompilerServices.RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: System.Diagnostics.Debuggable(System.Diagnostics.DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]
[assembly: System.Reflection.AssemblyVersion("0.0.0.0")]
[module: System.Runtime.CompilerServices.RefSafetyRules(11)]
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
