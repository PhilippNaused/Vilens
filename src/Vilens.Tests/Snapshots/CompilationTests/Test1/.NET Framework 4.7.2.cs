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
