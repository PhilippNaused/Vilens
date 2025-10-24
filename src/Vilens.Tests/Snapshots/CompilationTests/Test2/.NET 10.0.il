.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit ControlFlowClass
	extends [System.Private.CoreLib]System.Object
{
	// Methods
	.method public hidebysig static 
		void Loop1 (
			class [System.Private.CoreLib]System.Collections.Generic.IList`1<int32> list
		) cil managed 
	{
		// Method begins at RVA 0x2068
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
			IL_0006: call void ControlFlowClass::Add_Proxy(class [System.Private.CoreLib]System.Collections.Generic.ICollection`1<int32>, int32)
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
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [System.Private.CoreLib]System.Object::.ctor()
		IL_0006: ret
	} // end of method ControlFlowClass::.ctor

	.method private hidebysig static 
		void Add_Proxy (
			class [System.Private.CoreLib]System.Collections.Generic.ICollection`1<int32> '',
			int32 ''
		) cil managed aggressiveinlining 
	{
		// Method begins at RVA 0x2058
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.0
		IL_0003: ldvirtftn instance void class [System.Private.CoreLib]System.Collections.Generic.ICollection`1<int32>::Add(!0)
		IL_0009: calli void(class [System.Private.CoreLib]System.Collections.Generic.ICollection`1<int32>, int32)
		IL_000e: ret
	} // end of method ControlFlowClass::Add_Proxy

} // end of class ControlFlowClass