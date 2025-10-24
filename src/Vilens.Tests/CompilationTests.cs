using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using NUnit.Framework;
using Vilens.Data;
using Vilens.Helpers;

namespace Vilens.Tests;

internal class CompilationTests
{
    [Test]
    public void Test1()
    {
        const string sourceCode = """
                            using System.Collections.Generic;

                            public class ControlFlowClass
                            {
                                public static void Loop1(IList<int> list)
                                {
                                    for (int i = 0; i < 12; i++)
                                    {
                                        list.Add(i);
                                    }
                                }
                            }
                            """;
        Utils.Validate(sourceCode, VilensFeature.ControlFlow);
    }

    [Test]
    public void Test2()
    {
        const string sourceCode = """
                            using System.Collections.Generic;

                            public class ControlFlowClass
                            {
                                public static void Loop1(IList<int> list)
                                {
                                    for (int i = 0; i < 12; i++)
                                    {
                                        list.Add(i);
                                    }
                                }
                            }
                            """;
        Utils.Test(sourceCode, scrambler =>
        {
            foreach (var meth in scrambler.Database.Methods)
            {
                var body = meth.Item.Body;
                var list = body.Instructions;
                bool mod = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Operand is IMethodDefOrRef def && (def.Name == ".ctor" || def.Name == ".cctor"))
                    {
                        continue;
                    }
                    if (list[i].OpCode.Code is Code.Call)
                    {
                        var method = (IMethod)list[i].Operand;
                        list.Insert(i++, Emit.LoadMethodAddress(method));
                        list[i].Replace(Emit.CallIndirect(method.MethodSig));
                        mod = true;
                    }
                    else if (list[i].OpCode.Code is Code.Callvirt)
                    {
                        var method = (IMethod)list[i].Operand;
                        var sig2 = method.MethodSig.Clone();
                        var typeSig = method.DeclaringType.ToTypeSig();
                        sig2.Params.Insert(0, typeSig); // Add the object parameter
                        if (typeSig is GenericInstSig genSig)
                        {
                            for (int index = 0; index < sig2.Params.Count; index++)
                            {
                                TypeSig param = sig2.Params[index];
                                if (param is GenericVar genVar)
                                {
                                    sig2.Params[index] = genSig.GenericArguments[(int)genVar.Number];
                                }
                            }
                        }
                        sig2.HasThis = false; // make it "static"
                        var newMeth = new MethodDefUser(method.Name + "_Proxy", sig2, MethodImplAttributes.Managed | MethodImplAttributes.AggressiveInlining, MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig);
                        meth.Item.DeclaringType.Methods.Add(newMeth);
                        newMeth.Body = new CilBody();

                        foreach (var par in newMeth.Parameters)
                        {
                            newMeth.Body.Instructions.Add(Emit.Load(par));
                        }
                        newMeth.Body.Instructions.Add(Emit.Load(newMeth.Parameters.First()));
                        newMeth.Body.Instructions.Add(Emit.LoadVirtualMethodAddress(method));
                        newMeth.Body.Instructions.Add(Emit.CallIndirect(sig2));

                        newMeth.Body.Instructions.Add(Emit.Return());
                        newMeth.Body.Instructions.Optimize();
                        list[i].OpCode = OpCodes.Call;
                        list[i].Operand = newMeth;
                        mod = true;
                    }
                }
                if (mod)
                {
                    list.Optimize();
                    if (MaxStackCalculator.GetMaxStack(list, body.ExceptionHandlers, out var h))
                    {
                        body.MaxStack = (ushort)h;
                    }
                    else
                    {
                        body.MaxStack += 2;
                        body.KeepOldMaxStack = true;
                    }
                }
            }
        }, true);
    }
}
