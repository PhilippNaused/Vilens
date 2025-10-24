// Test, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Reference: System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]
[assembly: AssemblyVersion("0.0.0.0")]
[module: RefSafetyRules(11)]
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
