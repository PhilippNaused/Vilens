using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Vilens.Helpers;

namespace Vilens.Lab;

public class Class5
{
    public static int Test(IList<int> list)
    {
        return list.Single(i => i == 7);
    }

    public static void Update(ModuleDef mod)
    {
        var class5 = mod.FindNormalThrow(typeof(Class5).FullName);

        var lamClass = class5.NestedTypes.Single();
        lamClass.Name = UTF8String.Empty;
        lamClass.Attributes |= TypeAttributes.NestedAssembly;
        lamClass.CustomAttributes.Clear();
        foreach (var meth in lamClass.Methods.Where(m => !m.IsRuntimeSpecialName))
        {
            meth.Name = UTF8String.Empty;
        }
        foreach (var meth in lamClass.Fields.Where(m => !m.IsRuntimeSpecialName))
        {
            meth.Name = UTF8String.Empty;
        }
        var lamCtor = lamClass.FindDefaultConstructor();

        var method = class5.FindMethod(nameof(Test));
        var body = method.Body;
        var inst = body.Instructions;

        var ldstf = inst.Where(i => i.OpCode == OpCodes.Ldsfld).Skip(1).Single();
        var ldftn = inst.Single(i => i.OpCode == OpCodes.Ldftn);
        var newobj = inst.Single(i => i.OpCode == OpCodes.Newobj);
        var call = inst.Single(i => i.OpCode == OpCodes.Call);

        var newMeth = new MethodDefUser(UTF8String.Empty, MethodSig.CreateStatic(mod.CorLibTypes.Object, mod.CorLibTypes.IntPtr), MethodImplAttributes.IL | MethodImplAttributes.Managed | MethodImplAttributes.AggressiveInlining, MethodAttributes.Static | MethodAttributes.Assembly);
        var newBody = new CilBody();
        newMeth.Body = newBody;
        mod.GlobalType.Methods.Add(newMeth);
        var inst2 = newBody.Instructions;
        inst2.Add(Emit.NewObject(lamCtor));
        inst2.Add(Emit.Load(method.Parameters.Single())); // ldarg.0
        inst2.Add(newobj);
        inst2.Add(Emit.Return());

        inst.Clear();
        inst.Add(Emit.Load(method.Parameters.Single())); // ldarg.0
        inst.Add(ldftn);
        inst.Add(Emit.Call(newMeth));
        inst.Add(call);
        inst.Add(Emit.Return());
    }
}
