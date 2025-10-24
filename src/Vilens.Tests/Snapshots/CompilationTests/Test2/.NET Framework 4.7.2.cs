// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.CodeAnalysis;
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyVersion("0.0.0.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
    [CompilerGenerated]
    [Embedded]
    internal sealed class EmbeddedAttribute : Attribute
    {
    }
}
namespace System.Runtime.CompilerServices
{
    [CompilerGenerated]
    [Embedded]
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    internal sealed class RefSafetyRulesAttribute : Attribute
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
