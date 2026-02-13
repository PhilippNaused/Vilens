using System.Diagnostics.CodeAnalysis;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Vilens.Helpers;

#pragma warning disable CA1724 // Type names should not match namespaces
[ExcludeFromCodeCoverage] // Trivial wrapper around OpCodes, no need to test (most of them are unused)
internal static class Emit
#pragma warning restore CA1724 // Type names should not match namespaces
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable VSSpell001 // Spell Check

    #region Branching

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Br"/>
    public static Instruction Goto(Instruction target) => OpCodes.Br.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Brtrue"/>
    public static Instruction GotoIfTrue(Instruction target) => OpCodes.Brtrue.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Brfalse"/>
    public static Instruction GotoIfFalse(Instruction target) => OpCodes.Brfalse.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Beq"/>
    public static Instruction GotoIfEqual(Instruction target) => OpCodes.Beq.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Blt"/>
    public static Instruction GotoIfLess(Instruction target) => OpCodes.Blt.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Blt_Un"/>
    public static Instruction GotoIfLess_Un(Instruction target) => OpCodes.Blt_Un.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ble"/>
    public static Instruction GotoIfLessOrEqual(Instruction target) => OpCodes.Ble.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ble_Un"/>
    public static Instruction GotoIfLessOrEqual_Un(Instruction target) => OpCodes.Ble_Un.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Bgt"/>
    public static Instruction GotoIfGreater(Instruction target) => OpCodes.Bgt.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Bgt_Un"/>
    public static Instruction GotoIfGreater_Un(Instruction target) => OpCodes.Bgt_Un.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Bne_Un"/>
    public static Instruction GotoIfNotEqual_Un(Instruction target) => OpCodes.Bne_Un.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Bge"/>
    public static Instruction GotoIfGreaterOrEqual(Instruction target) => OpCodes.Bge.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Bge_Un"/>
    public static Instruction GotoIfGreaterOrEqual_Un(Instruction target) => OpCodes.Bge_Un.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Leave"/>
    public static Instruction Leave(Instruction target) => OpCodes.Leave.ToInstruction(target);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Switch"/>
    public static Instruction Switch(IList<Instruction> list) => OpCodes.Switch.ToInstruction(list);

    #endregion Branching

    #region Method Calls

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldvirtftn"/>
    public static Instruction LoadVirtualMethodAddress(IMethod method) => OpCodes.Ldvirtftn.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldftn"/>
    public static Instruction LoadMethodAddress(IMethod method) => OpCodes.Ldftn.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Newobj"/>
    public static Instruction NewObject(IMethod method) => OpCodes.Newobj.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Jmp"/>
    public static Instruction Jump(IMethod method) => OpCodes.Jmp.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Call"/>
    public static Instruction Call(IMethod method) => OpCodes.Call.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Callvirt"/>
    public static Instruction CallVirtual(IMethod method) => OpCodes.Callvirt.ToInstruction(method);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Calli"/>
    public static Instruction CallIndirect(MethodSig sig) => OpCodes.Calli.ToInstruction(sig);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Tailcall"/>
    public static Instruction TailCall() => OpCodes.Tailcall.ToInstruction();

    #endregion Method Calls

    #region Arrays

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_I"/>
    public static Instruction StoreElement_I() => OpCodes.Stelem_I.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_I1"/>
    public static Instruction StoreElement_I1() => OpCodes.Stelem_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_I2"/>
    public static Instruction StoreElement_I2() => OpCodes.Stelem_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_I4"/>
    public static Instruction StoreElement_I4() => OpCodes.Stelem_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_I8"/>
    public static Instruction StoreElement_I8() => OpCodes.Stelem_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_R4"/>
    public static Instruction StoreElement_R4() => OpCodes.Stelem_R4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_R8"/>
    public static Instruction StoreElement_R8() => OpCodes.Stelem_R8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem_Ref"/>
    public static Instruction StoreElement_Ref() => OpCodes.Stelem_Ref.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stelem"/>
    public static Instruction StoreElement(ITypeDefOrRef type) => OpCodes.Stelem.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_I"/>
    public static Instruction LoadElement_I() => OpCodes.Ldelem_I.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_I1"/>
    public static Instruction LoadElement_I1() => OpCodes.Ldelem_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_I2"/>
    public static Instruction LoadElement_I2() => OpCodes.Ldelem_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_I4"/>
    public static Instruction LoadElement_I4() => OpCodes.Ldelem_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_I8"/>
    public static Instruction LoadElement_I8() => OpCodes.Ldelem_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_U1"/>
    public static Instruction LoadElement_U1() => OpCodes.Ldelem_U1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_U2"/>
    public static Instruction LoadElement_U2() => OpCodes.Ldelem_U2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_U4"/>
    public static Instruction LoadElement_U4() => OpCodes.Ldelem_U4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_R4"/>
    public static Instruction LoadElement_R4() => OpCodes.Ldelem_R4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_R8"/>
    public static Instruction LoadElement_R8() => OpCodes.Ldelem_R8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem_Ref"/>
    public static Instruction LoadElement_Ref() => OpCodes.Ldelem_Ref.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelem"/>
    public static Instruction LoadElement(ITypeDefOrRef type) => OpCodes.Ldelem.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldelema"/>
    public static Instruction LoadElementAddress(ITypeDefOrRef type) => OpCodes.Ldelema.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldlen"/>
    public static Instruction LoadLength() => OpCodes.Ldlen.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Newarr"/>
    public static Instruction NewArray(ITypeDefOrRef type) => OpCodes.Newarr.ToInstruction(type);

    #endregion Arrays

    #region Exception Handling

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Throw"/>
    public static Instruction Throw() => OpCodes.Throw.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Endfilter"/>
    public static Instruction EndFilter() => OpCodes.Endfilter.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Rethrow"/>
    public static Instruction ReThrow() => OpCodes.Rethrow.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Endfinally"/>
    public static Instruction EndFinally() => OpCodes.Endfinally.ToInstruction();

    #endregion

    #region Convert

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I"/>
    public static Instruction Convert_Ovf_I() => OpCodes.Conv_Ovf_I.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I1"/>
    public static Instruction Convert_Ovf_I1() => OpCodes.Conv_Ovf_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I2"/>
    public static Instruction Convert_Ovf_I2() => OpCodes.Conv_Ovf_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I4"/>
    public static Instruction Convert_Ovf_I4() => OpCodes.Conv_Ovf_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I8"/>
    public static Instruction Convert_Ovf_I8() => OpCodes.Conv_Ovf_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I_Un"/>
    public static Instruction Convert_Ovf_I_Un() => OpCodes.Conv_Ovf_I_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I1_Un"/>
    public static Instruction Convert_Ovf_I1_Un() => OpCodes.Conv_Ovf_I1_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I2_Un"/>
    public static Instruction Convert_Ovf_I2_Un() => OpCodes.Conv_Ovf_I2_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I4_Un"/>
    public static Instruction Convert_Ovf_I4_Un() => OpCodes.Conv_Ovf_I4_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_I8_Un"/>
    public static Instruction Convert_Ovf_I8_Un() => OpCodes.Conv_Ovf_I8_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U_Un"/>
    public static Instruction Convert_Ovf_U_Un() => OpCodes.Conv_Ovf_U_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U1_Un"/>
    public static Instruction Convert_Ovf_U1_Un() => OpCodes.Conv_Ovf_U1_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U2_Un"/>
    public static Instruction Convert_Ovf_U2_Un() => OpCodes.Conv_Ovf_U2_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U4_Un"/>
    public static Instruction Convert_Ovf_U4_Un() => OpCodes.Conv_Ovf_U4_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U8_Un"/>
    public static Instruction Convert_Ovf_U8_Un() => OpCodes.Conv_Ovf_U8_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U"/>
    public static Instruction Convert_Ovf_U() => OpCodes.Conv_Ovf_U.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U1"/>
    public static Instruction Convert_Ovf_U1() => OpCodes.Conv_Ovf_U1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U2"/>
    public static Instruction Convert_Ovf_U2() => OpCodes.Conv_Ovf_U2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U4"/>
    public static Instruction Convert_Ovf_U4() => OpCodes.Conv_Ovf_U4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_Ovf_U8"/>
    public static Instruction Convert_Ovf_U8() => OpCodes.Conv_Ovf_U8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_I"/>
    public static Instruction Convert_I() => OpCodes.Conv_I.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_I1"/>
    public static Instruction Convert_I1() => OpCodes.Conv_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_I2"/>
    public static Instruction Convert_I2() => OpCodes.Conv_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_I4"/>
    public static Instruction Convert_I4() => OpCodes.Conv_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_I8"/>
    public static Instruction Convert_I8() => OpCodes.Conv_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_U"/>
    public static Instruction Convert_U() => OpCodes.Conv_U.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_U1"/>
    public static Instruction Convert_U1() => OpCodes.Conv_U1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_U2"/>
    public static Instruction Convert_U2() => OpCodes.Conv_U2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_U4"/>
    public static Instruction Convert_U4() => OpCodes.Conv_U4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_U8"/>
    public static Instruction Convert_U8() => OpCodes.Conv_U8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_R4"/>
    public static Instruction Convert_R4() => OpCodes.Conv_R4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_R8"/>
    public static Instruction Convert_R8() => OpCodes.Conv_R8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Conv_R_Un"/>
    public static Instruction Convert_R4_Un() => OpCodes.Conv_R_Un.ToInstruction();

    #endregion Convert

    #region Compare

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ceq"/>
    public static Instruction CompareEqual() => OpCodes.Ceq.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Clt"/>
    public static Instruction CompareLess() => OpCodes.Clt.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Cgt"/>
    public static Instruction CompareGreater() => OpCodes.Cgt.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Clt_Un"/>
    public static Instruction CompareLess_Un() => OpCodes.Clt_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Cgt_Un"/>
    public static Instruction CompareGreater_Un() => OpCodes.Cgt_Un.ToInstruction();

    #endregion Compare

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Prefixref"/>
    public static Instruction PrefixRef() => OpCodes.Prefixref.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Arglist"/>
    public static Instruction ArgList() => OpCodes.Arglist.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Localloc"/>
    public static Instruction LocAlloc() => OpCodes.Localloc.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Volatile"/>
    public static Instruction Volatile() => OpCodes.Volatile.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Cpblk"/>
    public static Instruction CopyBlock() => OpCodes.Cpblk.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Initblk"/>
    public static Instruction InitBlock() => OpCodes.Initblk.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Readonly"/>
    public static Instruction Readonly() => OpCodes.Readonly.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Dup"/>
    public static Instruction Duplicate() => OpCodes.Dup.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Pop"/>
    public static Instruction Pop() => OpCodes.Pop.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ret"/>
    public static Instruction Return() => OpCodes.Ret.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Break"/>
    public static Instruction Break() => OpCodes.Break.ToInstruction();

    #region Load/Store Indirect

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_Ref"/>
    public static Instruction LoadIndirect_Ref() => OpCodes.Ldind_Ref.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_R8"/>
    public static Instruction LoadIndirect_R8() => OpCodes.Ldind_R8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_R4"/>
    public static Instruction LoadIndirect_R4() => OpCodes.Ldind_R4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_I"/>
    public static Instruction LoadIndirect_I() => OpCodes.Ldind_I.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_I8"/>
    public static Instruction LoadIndirect_I8() => OpCodes.Ldind_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_U4"/>
    public static Instruction LoadIndirect_U4() => OpCodes.Ldind_U4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_I4"/>
    public static Instruction LoadIndirect_I4() => OpCodes.Ldind_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_U2"/>
    public static Instruction LoadIndirect_U2() => OpCodes.Ldind_U2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_I2"/>
    public static Instruction LoadIndirect_I2() => OpCodes.Ldind_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_U1"/>
    public static Instruction LoadIndirect_U1() => OpCodes.Ldind_U1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldind_I1"/>
    public static Instruction LoadIndirect_I1() => OpCodes.Ldind_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_I4"/>
    public static Instruction StoreIndirect_I4() => OpCodes.Stind_I4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_I8"/>
    public static Instruction StoreIndirect_I8() => OpCodes.Stind_I8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_R4"/>
    public static Instruction StoreIndirect_R4() => OpCodes.Stind_R4.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_R8"/>
    public static Instruction StoreIndirect_R8() => OpCodes.Stind_R8.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_Ref"/>
    public static Instruction StoreIndirect_Ref() => OpCodes.Stind_Ref.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_I1"/>
    public static Instruction StoreIndirect_I1() => OpCodes.Stind_I1.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_I2"/>
    public static Instruction StoreIndirect_I2() => OpCodes.Stind_I2.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stind_I"/>
    public static Instruction StoreIndirect_I() => OpCodes.Stind_I.ToInstruction();

    #endregion

    #region Primitives

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Not"/>
    public static Instruction Not() => OpCodes.Not.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Neg"/>
    public static Instruction Negate() => OpCodes.Neg.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Xor"/>
    public static Instruction Xor() => OpCodes.Xor.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Add_Ovf"/>
    public static Instruction Add_Ovf() => OpCodes.Add_Ovf.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Add_Ovf_Un"/>
    public static Instruction Add_Ovf_Un() => OpCodes.Add_Ovf_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Mul_Ovf"/>
    public static Instruction Mul_Ovf() => OpCodes.Mul_Ovf.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Mul_Ovf_Un"/>
    public static Instruction Mul_Ovf_Un() => OpCodes.Mul_Ovf_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Sub_Ovf"/>
    public static Instruction Sub_Ovf() => OpCodes.Sub_Ovf.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Sub_Ovf_Un"/>
    public static Instruction Sub_Ovf_Un() => OpCodes.Sub_Ovf_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Or"/>
    public static Instruction Or() => OpCodes.Or.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Add"/>
    public static Instruction Add() => OpCodes.Add.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Mul"/>
    public static Instruction Mul() => OpCodes.Mul.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Div"/>
    public static Instruction Div() => OpCodes.Div.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Div_Un"/>
    public static Instruction Div_Un() => OpCodes.Div_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Rem"/>
    public static Instruction Rem() => OpCodes.Rem.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Rem_Un"/>
    public static Instruction Rem_Un() => OpCodes.Rem_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.And"/>
    public static Instruction And() => OpCodes.And.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Sub"/>
    public static Instruction Sub() => OpCodes.Sub.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ckfinite"/>
    public static Instruction CheckFinite() => OpCodes.Ckfinite.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Shr_Un"/>
    public static Instruction ShiftRight_Un() => OpCodes.Shr_Un.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Shr"/>
    public static Instruction ShiftRight() => OpCodes.Shr.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Shl"/>
    public static Instruction ShiftLeft() => OpCodes.Shl.ToInstruction();

    #endregion Primitives

    #region Load

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_R8"/>
    public static Instruction Load(double d) => OpCodes.Ldc_R8.ToInstruction(d);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_R4"/>
    public static Instruction Load(float f) => OpCodes.Ldc_R4.ToInstruction(f);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldstr"/>
    public static Instruction Load(string s) => OpCodes.Ldstr.ToInstruction(s);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_I4"/>
    public static Instruction Load(int i) => OpCodes.Ldc_I4.ToInstruction(i);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_I4"/>
    public static Instruction Load(uint i) => OpCodes.Ldc_I4.ToInstruction((int)i);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_I8"/>
    public static Instruction Load(long l) => OpCodes.Ldc_I8.ToInstruction(l);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldc_I8"/>
    public static Instruction Load(ulong l) => OpCodes.Ldc_I8.ToInstruction((long)l);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldloc"/>
    public static Instruction Load(Local l) => OpCodes.Ldloc.ToInstruction(l);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldarg"/>
    public static Instruction Load(Parameter p) => OpCodes.Ldarg.ToInstruction(p);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldsfld"/> // TODO: Fix
    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldfld"/>
    public static Instruction Load(FieldDef field) => (field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld).ToInstruction(field);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldtoken"/>
    public static Instruction LoadToken(ITokenOperand token) => OpCodes.Ldtoken.ToInstruction(token);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldloca"/>
    public static Instruction LoadAddress(Local l) => OpCodes.Ldloca.ToInstruction(l);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldarga"/>
    public static Instruction LoadAddress(Parameter p) => OpCodes.Ldarga.ToInstruction(p);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldsflda"/> // TODO: Fix
    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldflda"/>
    public static Instruction LoadAddress(FieldDef field) => (field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda).ToInstruction(field);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldnull"/>
    public static Instruction LoadNull() => OpCodes.Ldnull.ToInstruction();

    #endregion Load

    #region Store

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Starg"/>
    public static Instruction Store(Parameter p) => OpCodes.Starg.ToInstruction(p);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stloc"/>
    public static Instruction Store(Local l) => OpCodes.Stloc.ToInstruction(l);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stsfld"/> // TODO: Fix
    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stfld"/>
    public static Instruction Store(FieldDef field) => (field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld).ToInstruction(field);

    #endregion Store

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Sizeof"/>
    public static Instruction SizeOf(ITypeDefOrRef type) => OpCodes.Sizeof.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Isinst"/>
    public static Instruction IsInstanceOf(ITypeDefOrRef type) => OpCodes.Isinst.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Constrained"/>
    public static Instruction Constrained(ITypeDefOrRef type) => OpCodes.Constrained.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Castclass"/>
    public static Instruction CastTo(ITypeDefOrRef type) => OpCodes.Castclass.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Nop"/>
    public static Instruction NoOp() => OpCodes.Nop.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Unaligned"/>
    public static Instruction Unaligned() => OpCodes.Unaligned.ToInstruction();

    #region Dynamically Typed References

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Refanytype"/>
    public static Instruction RefAnyType() => OpCodes.Refanytype.ToInstruction();

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Mkrefany"/>
    public static Instruction MakeRefAny(ITypeDefOrRef type) => OpCodes.Mkrefany.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Refanyval"/>
    public static Instruction RefAnyVal(ITypeDefOrRef type) => OpCodes.Refanyval.ToInstruction(type);

    #endregion Dynamically Typed References

    #region Value Types

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Box"/>
    public static Instruction Box(ITypeDefOrRef type) => OpCodes.Box.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Unbox"/>
    public static Instruction Unbox(ITypeDefOrRef type) => OpCodes.Unbox.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Unbox_Any"/>
    public static Instruction Unbox_Any(ITypeDefOrRef type) => OpCodes.Unbox_Any.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Initobj"/>
    public static Instruction InitObject(ITypeDefOrRef type) => OpCodes.Initobj.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Ldobj"/>
    public static Instruction LoadObject(ITypeDefOrRef type) => OpCodes.Ldobj.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Cpobj"/>
    public static Instruction CopyObject(ITypeDefOrRef type) => OpCodes.Cpobj.ToInstruction(type);

    /// <inheritdoc cref="System.Reflection.Emit.OpCodes.Stobj"/>
    public static Instruction StoreObject(ITypeDefOrRef type) => OpCodes.Stobj.ToInstruction(type);

    #endregion
}
