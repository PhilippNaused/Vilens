using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class StringHiding : FeatureBase
{
    private readonly List<IMethodData> _methods;

    private readonly TypeSig voidPtr;

    public StringHiding(Scrambler scrambler) : base(scrambler)
    {
        voidPtr = new FnPtrSig(MethodSig.CreateStatic(Module.CorLibTypes.Void));

        _methods = Database.Methods.AsParallel().Where(m => m.Item.HasBody && m.HasFeatures(VilensFeature.StringHiding)).ToList();
        Log.Debug("Filtered {0} methods", _methods.Count);
    }

    public override Logger Log { get; } = new Logger(nameof(StringHiding));

    public override void Execute()
    {
        var removed = _methods.RemoveAll(m => m.WasTrimmed());
        Log.Debug("Removed {0} methods that were trimmed", removed);

        Log.Debug("Searching for strings");
        var strings = _methods.SelectMany(m => m.Item.Body.Instructions.Where(i => i.Operand is string).Select(i => (string)i.Operand)).Distinct(StringComparer.Ordinal).ToList();
        Log.Debug("Found {0} unique strings", strings.Count);
        Cancellation.ThrowIfCancellationRequested();
        if (strings.Count == 0)
        {
            Log.Debug("Nothing to do");
            return;
        }

        Encode(strings, out var data, out var dict);
        var valueType = Module.AddCoreRef(typeof(ValueType));
        var compressedData = Compress(data);

        // create new struct for decoding the strings
        var newClass = new TypeDefUser(string.Empty, valueType)
        {
            PackingSize = 1,
            ClassSize = (uint)compressedData.Length,
            Layout = TypeAttributes.ExplicitLayout,
            Visibility = TypeAttributes.NotPublic,
            IsAbstract = true
        };
        Module.AddAsNonNestedType(newClass);
        NamingHelper.Rename(newClass, Scrambler.Settings.NamingScheme);
        var naming = new NamingHelper(Scrambler.Settings.NamingScheme);

        // init-only field that holds the compressed data
        var compressedDataField = new FieldDefUser(string.Empty, new FieldSig(newClass.ToTypeSig()))
        {
            Name = naming.GetNextName(),
            DeclaringType = newClass,
            Access = FieldAttributes.Assembly,
            InitialValue = compressedData,
            HasFieldRVA = true,
            IsStatic = true,
            IsInitOnly = true
        };
        Cancellation.ThrowIfCancellationRequested();

        // internal static readonly string[] strings;
        var stringsField = new FieldDefUser(string.Empty, new FieldSig(new SZArraySig(Module.CorLibTypes.String)))
        {
            Name = naming.GetNextName(),
            Access = FieldAttributes.Assembly,
            DeclaringType = newClass,
            IsStatic = true,
            IsInitOnly = true
        };

        // use the static constructor to decode the data at runtime
        var cctor = newClass.FindOrCreateStaticConstructor();
        var body = cctor.Body;
        body.Instructions.Clear();

        var uint8 = Module.CorLibTypes.Byte;
        //  .locals init(
        //      [0] void* ptr,
        //      [1] byte[] data (pinned)
        //  )
        var ptr = body.Variables.Add(new Local(voidPtr));
        var decompressedDataArray = body.Variables.Add(new Local(new PinnedSig(new SZArraySig(uint8))));

        // fixed (byte[] data = new byte[outSize])
        body.Instructions.Add(Emit.Load(data.Length));
        body.Instructions.Add(Emit.NewArray(uint8.ToTypeDefOrRef()));
        body.Instructions.Add(Emit.Store(decompressedDataArray));

        Decompress(cctor, compressedData.LongLength, compressedDataField, decompressedDataArray);

        // void* ptr = &data[0]
        body.Instructions.Add(Emit.Load(decompressedDataArray));
        body.Instructions.Add(Emit.Load(0));
        body.Instructions.Add(Emit.LoadElementAddress(uint8.ToTypeDefOrRef()));
        body.Instructions.Add(Emit.Convert_U());
        body.Instructions.Add(Emit.Store(ptr));

        var encodingRef = Module.AddCoreRef(typeof(Encoding));
        // get Property Encoding.UTF8
        var get_UTF8Method = new MemberRefUser(Module, "get_UTF8", MethodSig.CreateStatic(encodingRef.ToTypeSig()), encodingRef);
        // string Encoding.UTF8.GetString(ptr, length)
        var GetStringMethod = new MemberRefUser(Module, "GetString", MethodSig.CreateInstance(Module.CorLibTypes.String, new PtrSig(uint8), Module.CorLibTypes.Int32), encodingRef);

        // initialize the string array field
        // strings = new string[encodedCount]
        body.Instructions.Add(Emit.Load(dict.Count));
        body.Instructions.Add(Emit.NewArray(Module.CorLibTypes.String.ToTypeDefOrRef()));
        body.Instructions.Add(Emit.Store(stringsField));

        // int index = 0
        var index = body.Variables.Add(new Local(Module.CorLibTypes.Int32));
        // TODO: do we need to initialize this?

        // int length = *ptr;
        var length = body.Variables.Add(new Local(Module.CorLibTypes.Int32));
        var loopStart = body.Instructions.Append(Emit.Load(ptr));
        body.Instructions.Add(Emit.LoadIndirect_U1());
        body.Instructions.Add(Emit.Store(length));

        // ptr++;
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Load(1));
        body.Instructions.Add(Emit.Add());
        body.Instructions.Add(Emit.Store(ptr));

        // strings[index] = encoding.GetString(ptr, length);
        body.Instructions.Add(Emit.Load(stringsField));
        body.Instructions.Add(Emit.Load(index));
        body.Instructions.Add(Emit.Call(get_UTF8Method));
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Load(length));
        body.Instructions.Add(Emit.Call(GetStringMethod));
        body.Instructions.Add(Emit.StoreElement_Ref());

        // index++;
        body.Instructions.Add(Emit.Load(index));
        body.Instructions.Add(Emit.Load(1));
        body.Instructions.Add(Emit.Add());
        body.Instructions.Add(Emit.Store(index));

        // ptr += length;
        body.Instructions.Add(Emit.Load(ptr));
        body.Instructions.Add(Emit.Load(length));
        body.Instructions.Add(Emit.Add());
        body.Instructions.Add(Emit.Store(ptr));

        // if (index < encodedCount) goto loopStart;
        body.Instructions.Add(Emit.Load(index));
        body.Instructions.Add(Emit.Load(dict.Count));
        body.Instructions.Add(Emit.GotoIfLess(loopStart));

        body.Instructions.Add(Emit.Return());

        body.Instructions.Optimize();
        int count = 0;
        foreach (var method in _methods)
        {
            bool updated = false;
            var instructions = method.Item.Body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction? instr = instructions[i];
                if (instr.Operand is string str && dict.TryGetValue(str, out var sIndex))
                {
                    Log.Trace("Replacing {0} in {1}", instr, method);
                    // newClass.string[i];
                    instr.Replace(Emit.Load(stringsField));
                    instructions.Insert(i + 1, Emit.Load(sIndex));
                    instructions.Insert(i + 2, Emit.LoadElement_Ref());
                    count++;
                    updated = true;
                }
            }
            if (updated)
            {
                instructions.SimplifyBranches();
                instructions.OptimizeBranches();
            }
        }
        Log.Info("Encoded {0} instructions with {1} unique strings into {2} bytes of data.", count, dict.Count, compressedData.Length);
    }

    private static void Encode(List<string> strings, out byte[] data, out Dictionary<string, int> values)
    {
        values = [];
        using var stream = new MemoryStream();
        int index = 0;
        foreach (var str in strings)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            if (buffer.Length > byte.MaxValue)
                continue; // TODO: remove this limitation
            byte l = (byte)buffer.Length;
            stream.WriteByte(l);
            stream.Write(buffer, 0, l);
            values.Add(str, index++);
        }

        data = stream.ToArray();
    }

    private static byte[] Compress(byte[] data)
    {
        using var compressStream = new MemoryStream();
        using var compressor = new DeflateStream(compressStream, CompressionLevel.Optimal);
        new MemoryStream(data).CopyTo(compressor);
        compressor.Close();
        return compressStream.ToArray();
    }

    private void Decompress(MethodDef method, long inSize, FieldDef field, Local dataArrayLocal)
    {
        var mod = Module;
        var uint8Ptr = new PtrSig(mod.CorLibTypes.Byte);

        var defStreamRef = mod.AddReference(typeof(DeflateStream), "netstandard", "System.IO.Compression", "System");
        var uMemStreamRef = mod.AddCoreRef(typeof(UnmanagedMemoryStream));
        var memStreamRef = mod.AddCoreRef(typeof(MemoryStream));
        // void System.IO.UnmanagedMemoryStream::.ctor(uint8*, int64)
        var newUnmanagedMemoryStream = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, uint8Ptr, mod.CorLibTypes.Int64), uMemStreamRef);
        // void System.IO.MemoryStream::.ctor(uint8[])
        var newMemoryStream = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, new SZArraySig(mod.CorLibTypes.Byte)), memStreamRef);

        var streamRef = mod.AddCoreRef(typeof(Stream));
        // Need to explicitly use ValueTypeSig because dnlib thinks that CompressionMode is a class IF we run on Linux.
        // TODO: report this to dnlib
        var compressionModeSig = new ValueTypeSig(mod.AddReference(typeof(CompressionMode), "netstandard", "System.IO.Compression", "System"));
        var defCtor = new MemberRefUser(mod, ".ctor", MethodSig.CreateInstance(mod.CorLibTypes.Void, streamRef.ToTypeSig(), compressionModeSig), defStreamRef);

        var copyMethod = new MemberRefUser(mod, nameof(Stream.CopyTo), MethodSig.CreateInstance(mod.CorLibTypes.Void, streamRef.ToTypeSig()), streamRef);
        TypeRef disposableRef = mod.AddCoreRef(typeof(IDisposable));
        var disposeMethod = new MemberRefUser(mod, nameof(IDisposable.Dispose), MethodSig.CreateInstance(mod.CorLibTypes.Void), disposableRef);

        var body = method.Body;
        var deflateStream = body.Variables.Add(new Local(mod.CorLibTypes.Object));
        var instr = body.Instructions;

        // new UnmanagedMemoryStream(&field, inSize)
        instr.Add(Emit.LoadAddress(field));
        instr.Add(Emit.Load(inSize));
        instr.Add(Emit.NewObject(newUnmanagedMemoryStream));

        // DeflateStream temp = new DeflateStream(..., CompressionMode.Decompress)
        instr.Add(Emit.Load((int)CompressionMode.Decompress));
        instr.Add(Emit.NewObject(defCtor));
        instr.Add(Emit.Store(deflateStream));

        // temp.CopyTo(new MemoryStream(data))
        instr.Add(Emit.Load(deflateStream)); // DeflateStream temp

        instr.Add(Emit.Load(dataArrayLocal)); // byte[] data
        instr.Add(Emit.NewObject(newMemoryStream)); // new MemoryStream(data)
        instr.Add(Emit.CallVirtual(copyMethod)); // void Stream.CopyTo(Stream)

        // temp.Dispose()
        instr.Add(Emit.Load(deflateStream)); // DeflateStream temp
        instr.Add(Emit.CallVirtual(disposeMethod)); // temp.Dispose()
    }
}
