using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Vilens.Helpers;

namespace Vilens.Lab;

public static class Class4
{
    public static string Get()
    {
        return "_Invokÿǿ";
    }
}

public static class Class4b
{
    public static void Update(ModuleDef mod)
    {
        var valueType = new TypeRefUser(mod, nameof(System), nameof(ValueType), mod.CorLibTypes.AssemblyRef);
        var class4 = mod.FindNormalThrow(typeof(Class4).FullName);
        var method = class4.Methods.Single();
        var instr = method.Body.Instructions.Single(i => i.OpCode == OpCodes.Ldstr);

        var str = (string)instr.Operand;
        var bytes = Encoding.Unicode.GetBytes(str);
        var bytesCompressed = Compress(bytes);

        var newClass = new TypeDefUser(string.Empty, valueType)
        {
            PackingSize = 1,
            ClassSize = (uint)bytesCompressed.Length,
            Layout = TypeAttributes.ExplicitLayout,
            Attributes = TypeAttributes.Sealed | TypeAttributes.ExplicitLayout | TypeAttributes.NotPublic
        };
        mod.AddAsNonNestedType(newClass);
        var initField = new FieldDefUser(string.Empty, new FieldSig(newClass.ToTypeSig()))
        {
            DeclaringType = newClass,
            Attributes = FieldAttributes.Static | FieldAttributes.Assembly | FieldAttributes.InitOnly | FieldAttributes.HasFieldRVA,
            InitialValue = bytesCompressed
        };

        var deflate = CreateDecompressor(bytesCompressed.LongLength, bytes.LongLength, mod);
        deflate.DeclaringType = newClass;

        var cctor = newClass.FindOrCreateStaticConstructor();

        var sfield = new FieldDefUser(string.Empty, new FieldSig(mod.CorLibTypes.String))
        {
            Access = FieldAttributes.Assembly,
            DeclaringType = newClass,
            IsStatic = true,
            IsInitOnly = true
        };

        var body = cctor.Body;
        body.Instructions.Clear();

        //  .locals init(
        //      [0] int8*
        //  )

        //  ldsflda valuetype '' ''::''
        //  ldc.i4.0
        //  ldc.i4 26
        //  newobj instance void[System.Runtime] System.String::.ctor(int8*, int32, int32)
        //  ret
        var ptrSig = new PtrSig(mod.CorLibTypes.Void);
        var charPtrSig = new PtrSig(mod.CorLibTypes.Char);
        var ptr = body.Variables.Add(new Local(ptrSig));
        var marshal = mod.AddReference(typeof(Marshal), "netstandard", "System.Runtime.InteropServices", "mscorlib");
        var alloc = new MemberRefUser(mod, nameof(Marshal.AllocHGlobal), MethodSig.CreateStatic(mod.CorLibTypes.IntPtr, mod.CorLibTypes.Int32), marshal); // IntPtr AllocHGlobal (int);
        var free = new MemberRefUser(mod, nameof(Marshal.FreeHGlobal), MethodSig.CreateStatic(mod.CorLibTypes.Void, mod.CorLibTypes.IntPtr), marshal); // IntPtr FreeHGlobal (int);

        // ptr = Marshal.AllocHGlobal(bytes.Length)
        body.Instructions.Add(Emit.Load(bytes.Length));
        body.Instructions.Add(Emit.Call(alloc));
        body.Instructions.Add(Emit.Store(ptr));

        // deflate(&initField, ptr)
        body.Instructions.Add(Emit.LoadAddress(initField));
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Call(deflate));

        // sfield = new string((char*)ptr, 0, bytes.Length / 2)
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Load(0));
        body.Instructions.Add(Emit.Load(bytes.Length / 2));
        var stringRef = mod.CorLibTypes.String.TypeRef;
        Debug.Assert(stringRef != null);
        var intSig = mod.CorLibTypes.Int32;
        var methodRef = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, charPtrSig, intSig, intSig), stringRef);
        body.Instructions.Add(Emit.NewObject(methodRef));
        body.Instructions.Add(Emit.Store(sfield));

        // Marshal.FreeHGlobal(ptr);
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Call(free));
        body.Instructions.Add(Emit.Return());

        instr.Replace(Emit.Load(sfield));

        body.Instructions.Optimize();

    }

    public static unsafe void Decompress(byte* src, long srcLength, byte* dest, long destLength)
    {
        var def = new DeflateStream(new UnmanagedMemoryStream(src, srcLength), CompressionMode.Decompress);
        def.CopyTo(new UnmanagedMemoryStream(dest, 0, destLength, FileAccess.Write));
        def.Dispose();
    }

    public static unsafe void Compress(byte* src, long srcLength, byte* dest, long destLength)
    {
        var def = new DeflateStream(new UnmanagedMemoryStream(dest, 0, destLength, FileAccess.Write), CompressionMode.Compress);
        new UnmanagedMemoryStream(src, srcLength).CopyTo(def);
        def.Dispose();
    }

    private static byte[] Compress(byte[] data)
    {
        var compressStream = new MemoryStream();
        using var compressor = new DeflateStream(compressStream, CompressionMode.Compress);
        new MemoryStream(data).CopyTo(compressor);
        compressor.Close();
        return compressStream.ToArray();
    }

    private static MethodDefUser CreateDecompressor(long inSize, long outSize, ModuleDef mod)
    {
        var ptr = new PtrSig(mod.CorLibTypes.Byte);
        var method = new MethodDefUser(string.Empty)
        {
            IsStatic = true,
            IsManaged = true,
            Access = MethodAttributes.Assembly,
            MethodSig = MethodSig.CreateStatic(mod.CorLibTypes.Void, ptr, ptr),
            Body = new CilBody(),
        };

        method.Parameters.UpdateParameterTypes();
        foreach (var p in method.Parameters)
        {
            p.Name = ",";
        }

        Debug.Assert(method.Parameters.Count == 2, $"{method.Parameters.Count}");

        var defStreamRef = mod.AddReference(typeof(DeflateStream), "netstandard", "System.IO.Compression", "System");
        var memStreamRef = mod.AddCoreRef(typeof(UnmanagedMemoryStream));
        var memCtor1 = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, ptr, mod.CorLibTypes.Int64), memStreamRef);
        var memCtor2 = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, ptr, mod.CorLibTypes.Int64, mod.CorLibTypes.Int64, new ValueTypeSig(mod.AddCoreRef(typeof(FileAccess)))), memStreamRef);

        var streamRef = mod.AddCoreRef(typeof(Stream));
        var compressionModeSig = new ValueTypeSig(mod.AddReference(typeof(CompressionMode), "netstandard", "System.IO.Compression", "System"));
        var defCtor = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, streamRef.ToTypeSig(), compressionModeSig), defStreamRef);

        var copyMethod = new MemberRefUser(mod, nameof(Stream.CopyTo), MethodSig.CreateInstance(mod.CorLibTypes.Void, streamRef.ToTypeSig()), streamRef);
        TypeRef disposableRef = mod.AddCoreRef(typeof(IDisposable));
        var disposeMethod = new MemberRefUser(mod, nameof(IDisposable.Dispose), MethodSig.CreateInstance(mod.CorLibTypes.Void), disposableRef);

        var body = method.Body;
        var local = body.Variables.Add(new Local(defStreamRef.ToTypeSig()));
        var instr = body.Instructions;

        instr.Add(Emit.Load(method.Parameters[0])); // byte* src
        instr.Add(Emit.Load(inSize)); // long inSize
        instr.Add(Emit.NewObject(memCtor1)); // new UnmanagedMemoryStream(byte*, long)

        instr.Add(Emit.Load(0)); // CompressionMode.Decompress
        instr.Add(Emit.NewObject(defCtor)); // new DeflateStream(Stream, CompressionMode)

        instr.Add(Emit.Store(local)); // DeflateStream temp =
        instr.Add(Emit.Load(local)); // DeflateStream temp

        instr.Add(Emit.Load(method.Parameters[1])); // byte* dest
        instr.Add(Emit.Load(0)); // 0
        instr.Add(Emit.Convert_I8()); // (long)0
        instr.Add(Emit.Load(outSize)); // long outSize
        instr.Add(Emit.Load(2)); // FileAccess.Write

        instr.Add(Emit.NewObject(memCtor2)); // new UnmanagedMemoryStream(byte*, long, long, FileAccess)

        instr.Add(Emit.CallVirtual(copyMethod)); // void Stream.CopyTo(Stream)

        instr.Add(Emit.Load(local)); // DeflateStream temp
        instr.Add(Emit.CallVirtual(disposeMethod)); // temp.Dispose()
        instr.Add(Emit.Return()); // return;

        instr.Optimize();
        return method;
    }
}
