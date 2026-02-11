.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
	extends [mscorlib]System.Attribute
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	.custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = (
		01 00 00 00
	)
	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Attribute::.ctor()
		IL_0006: ret
	} // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
	extends [mscorlib]System.Attribute
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	.custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = (
		01 00 00 00
	)
	.custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = (
		01 00 02 00 00 00 02 00 54 02 0d 41 6c 6c 6f 77
		4d 75 6c 74 69 70 6c 65 00 54 02 09 49 6e 68 65
		72 69 74 65 64 00
	)
	// Fields
	.field public initonly int32 Version

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			int32 ''
		) cil managed 
	{
		// Method begins at RVA 0x2058
		// Header size: 1
		// Code size: 14 (0xe)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Attribute::.ctor()
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: stfld int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
		IL_000d: ret
	} // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

.class public auto ansi beforefieldinit ControlFlowClass
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig static 
		void Loop1 (
			class [mscorlib]System.Collections.Generic.IList`1<int32> list
		) cil managed 
	{
		// Method begins at RVA 0x2080
		// Header size: 12
		// Code size: 21 (0x15)
		.maxstack 2
		.locals init (
			[0] int32
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		IL_0002: br.s IL_000f
		// loop start (head: IL_000f)
			IL_0004: ldarg.0
			IL_0005: ldloc.0
			IL_0006: call void ControlFlowClass::Add_Proxy(class [mscorlib]System.Collections.Generic.ICollection`1<int32>, int32)
			IL_000b: ldloc.0
			IL_000c: ldc.i4.1
			IL_000d: add
			IL_000e: stloc.0

			IL_000f: ldloc.0
			IL_0010: ldc.i4.s 12
			IL_0012: blt.s IL_0004
		// end loop

		IL_0014: ret
	} // end of method ControlFlowClass::Loop1

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2067
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method ControlFlowClass::.ctor

	.method private hidebysig static 
		void Add_Proxy (
			class [mscorlib]System.Collections.Generic.ICollection`1<int32> '',
			int32 ''
		) cil managed aggressiveinlining 
	{
		// Method begins at RVA 0x206f
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.0
		IL_0003: ldvirtftn instance void class [mscorlib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
		IL_0009: calli void(class [mscorlib]System.Collections.Generic.ICollection`1<int32>, int32)
		IL_000e: ret
	} // end of method ControlFlowClass::Add_Proxy

} // end of class ControlFlowClass