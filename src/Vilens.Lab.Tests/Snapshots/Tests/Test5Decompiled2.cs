using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

[Serializable]
protected internal sealed class 
{
    public static readonly   = new ();
    public static Func<int, bool> ;
    public static Func<MethodDef, bool> ;
    public static Func<FieldDef, bool> ;
    public static Func<Instruction, bool> ;
    public static Func<Instruction, bool> ;
    public static Func<Instruction, bool> ;
    public static Func<Instruction, bool> ;
    internal bool (int i)
    {
        return i == 7;
    }
    internal bool (MethodDef m)
    {
        return !m.IsRuntimeSpecialName;
    }
    internal bool (FieldDef m)
    {
        return !m.IsRuntimeSpecialName;
    }
    internal bool (Instruction i)
    {
        return i.OpCode == OpCodes.Ldsfld;
    }
    internal bool (Instruction i)
    {
        return i.OpCode == OpCodes.Ldftn;
    }
    internal bool (Instruction i)
    {
        return i.OpCode == OpCodes.Newobj;
    }
    internal bool (Instruction i)
    {
        return i.OpCode == OpCodes.Call;
    }
}