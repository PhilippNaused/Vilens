using dnlib.DotNet;
using Vilens.Helpers;

namespace Vilens.Lab;

public static unsafe class Class6
{
    public static void Get(byte* dest, int size, __arglist)
    {
        throw null!;
    }
}

public static class Class6b
{
    public static void Update(ModuleDef mod)
    {
        var class6 = mod.FindNormalThrow(typeof(Class6).FullName);
        var method = class6.Methods.Single(m => m.Name == nameof(Class6.Get));
        var insts = method.Body.Instructions;
        insts.Clear();
        insts.Add(Emit.Load(method.Parameters[0]));
        insts.Add(Emit.ArgList());
        insts.Add(Emit.Load(method.Parameters[1]));
        insts.Add(Emit.CopyBlock());
        insts.Add(Emit.Return());
        insts.Optimize();
    }
}
