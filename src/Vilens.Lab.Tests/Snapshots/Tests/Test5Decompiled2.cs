[System.Serializable]
protected internal sealed class 
{
    public static readonly Vilens.Lab.Class5.  = new Vilens.Lab.Class5.();
    public static System.Func<int, bool> ;
    public static System.Func<dnlib.DotNet.MethodDef, bool> ;
    public static System.Func<dnlib.DotNet.FieldDef, bool> ;
    public static System.Func<dnlib.DotNet.Emit.Instruction, bool> ;
    public static System.Func<dnlib.DotNet.Emit.Instruction, bool> ;
    public static System.Func<dnlib.DotNet.Emit.Instruction, bool> ;
    public static System.Func<dnlib.DotNet.Emit.Instruction, bool> ;
    internal bool (int i)
    {
        return i == 7;
    }
    internal bool (dnlib.DotNet.MethodDef m)
    {
        return !m.IsRuntimeSpecialName;
    }
    internal bool (dnlib.DotNet.FieldDef m)
    {
        return !m.IsRuntimeSpecialName;
    }
    internal bool (dnlib.DotNet.Emit.Instruction i)
    {
        return i.OpCode == dnlib.DotNet.Emit.OpCodes.Ldsfld;
    }
    internal bool (dnlib.DotNet.Emit.Instruction i)
    {
        return i.OpCode == dnlib.DotNet.Emit.OpCodes.Ldftn;
    }
    internal bool (dnlib.DotNet.Emit.Instruction i)
    {
        return i.OpCode == dnlib.DotNet.Emit.OpCodes.Newobj;
    }
    internal bool (dnlib.DotNet.Emit.Instruction i)
    {
        return i.OpCode == dnlib.DotNet.Emit.OpCodes.Call;
    }
}