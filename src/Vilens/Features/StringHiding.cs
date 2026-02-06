using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class StringHiding : FeatureBase
{
    private readonly List<IMethodData> _methods;

    /// <summary>
    /// new string(sbyte*, int, int)
    /// </summary>
    private readonly MemberRef ASCIICtor;

    /// <summary>
    /// new string(sbyte*)
    /// </summary>
    private readonly MemberRef ASCIICtorNull;

    /// <summary>
    /// new string(char*, int, int)
    /// </summary>
    private readonly MemberRef UTF16Ctor;

    /// <summary>
    /// new string(char*)
    /// </summary>
    private readonly MemberRef UTF16CtorNull;

    private readonly TypeSig voidPtr;

    public StringHiding(Scrambler scrambler) : base(scrambler)
    {
        var stringRef = Module.CorLibTypes.String.TypeRef;

        var sBytePtr = new PtrSig(Module.CorLibTypes.SByte);
        voidPtr = new FnPtrSig(MethodSig.CreateStatic(Module.CorLibTypes.Void));
        var charPtr = new PtrSig(Module.CorLibTypes.Char);
        var intSig = Module.CorLibTypes.Int32;

        // TODO: Add UTF8 support (Encoding.UTF8.GetString(byte*, int))
        ASCIICtor = new MemberRefUser(Module, ".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void, sBytePtr, intSig, intSig), stringRef); // new string(sbyte*, int, int)
        UTF16Ctor = new MemberRefUser(Module, ".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void, charPtr, intSig, intSig), stringRef); // new string(char*, int, int)
        ASCIICtorNull = new MemberRefUser(Module, ".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void, sBytePtr), stringRef); // new string(sbyte*)
        UTF16CtorNull = new MemberRefUser(Module, ".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void, charPtr), stringRef); // new string(char*)

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
        var encoded = dict.Select(p => p.Value).ToImmutableList();
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
        var initField = new FieldDefUser(string.Empty, new FieldSig(newClass.ToTypeSig()))
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

        // add an init-only string field for each string
        foreach (var eString in encoded)
        {
            eString.Field = new FieldDefUser(string.Empty, new FieldSig(Module.CorLibTypes.String))
            {
                Name = naming.GetNextName(),
                Access = FieldAttributes.Assembly,
                DeclaringType = newClass,
                IsStatic = true,
                IsInitOnly = true
            };
        }

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
        var localArray = body.Variables.Add(new Local(new PinnedSig(new SZArraySig(uint8))));

        // fixed (byte[] data = new byte[outSize])
        body.Instructions.Add(Emit.Load(data.Length));
        body.Instructions.Add(Emit.NewArray(uint8.ToTypeDefOrRef()));
        body.Instructions.Add(Emit.Store(localArray));

        Decompress(cctor, compressedData.LongLength, initField, localArray);

        // ptr = &data[0]
        body.Instructions.Add(Emit.Load(localArray));
        body.Instructions.Add(Emit.Load(0));
        body.Instructions.Add(Emit.LoadElementAddress(uint8.ToTypeDefOrRef()));
        body.Instructions.Add(Emit.Convert_U());
        body.Instructions.Add(Emit.Store(ptr));

        int current = 0;
        foreach (var eString in encoded)
        {
            Cancellation.ThrowIfCancellationRequested();
            // ptr += offset - current
            body.Instructions.Add(Emit.Load(ptr));
            if (eString.Offset != current)
            {
                body.Instructions.Add(Emit.Load(eString.Offset - current));
                body.Instructions.Add(Emit.Add());
                body.Instructions.Add(Emit.Duplicate());
                body.Instructions.Add(Emit.Store(ptr));
                current = eString.Offset;
            }

            if (eString.IsAscii)
            {
                if (eString.HasNull)
                {
                    // field = new string((sbyte*)ptr, 0, length)
                    body.Instructions.Add(Emit.Load(0));
                    body.Instructions.Add(Emit.Load(eString.Length));
                    body.Instructions.Add(Emit.NewObject(ASCIICtor));
                }
                else
                {
                    // field = new string((sbyte*)ptr)
                    body.Instructions.Add(Emit.NewObject(ASCIICtorNull));
                }
            }
            else
            {
                Debug.Assert(eString.Length % 2 == 0);
                if (eString.HasNull)
                {
                    // field = new string((char*)ptr, 0, length / 2)
                    body.Instructions.Add(Emit.Load(0));
                    body.Instructions.Add(Emit.Load(eString.Length / 2));
                    body.Instructions.Add(Emit.NewObject(UTF16Ctor));
                }
                else
                {
                    // field = new string((char*)ptr)
                    body.Instructions.Add(Emit.NewObject(UTF16CtorNull));
                }
            }
            body.Instructions.Add(Emit.Store(eString.Field!));
        }

        body.Instructions.Add(Emit.Return());

        body.Instructions.Optimize();
        int count = 0;
        foreach (var method in _methods)
        {
            foreach (var instr in method.Item.Body.Instructions)
            {
                if (instr.Operand is string str && dict.TryGetValue(str, out var eString))
                {
                    Log.Trace("Replacing {0} in {1} using {2} encoding", instr, method, eString.IsAscii ? "ASCII" : "UTF16");
                    instr.Replace(Emit.Load(eString.Field!));
                    count++;
                }
            }
        }
        Log.Info("Encoded {0} instructions with {1} unique strings into {2} bytes of data.", count, encoded.Count, compressedData.Length);
    }

    private static void Encode(List<string> strings, out byte[] data, out Dictionary<string, EncodedString> values)
    {
        values = [];
        using var stream = new MemoryStream();
        foreach (var str in strings)
        {
            bool ascii = IsAscii(str);
            bool hasNull = str.Contains('\0');
            var str2 = str;
            if (!hasNull)
            {
                str2 += '\0';
            }
            byte[] buffer = ascii ? Encoding.ASCII.GetBytes(str2) : Encoding.Unicode.GetBytes(str2);
            int offset = checked((int)stream.Position);
            stream.Write(buffer, 0, buffer.Length);

            var encoded = new EncodedString
            {
                IsAscii = ascii,
                Offset = offset,
                Length = buffer.Length,
                HasNull = hasNull,
            };
            values.Add(str, encoded);
        }

        data = stream.ToArray();
    }

    private static bool IsAscii(string str)
    {
        ReadOnlySpan<char> span = str.AsSpan();
        foreach (var c in span)
        {
            if (c > 127)
            {
                return false;
            }
        }
        return true;
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

    private sealed class EncodedString
    {
        public FieldDefUser? Field;
        public required bool HasNull;
        public required bool IsAscii;
        public required int Length;
        public required int Offset;
    }
}
