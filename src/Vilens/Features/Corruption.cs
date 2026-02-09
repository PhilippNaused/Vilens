using System.Collections.Immutable;
using System.Diagnostics;
using dnlib.DotNet;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class Corruption : FeatureBase
{
    private readonly TypeSig? _enum16;
    private readonly TypeSig? _enum32;
    private readonly TypeSig? _enum64;
    private readonly TypeSig? _enum8;
    private readonly ITypeDefOrRef _enumType;

    private readonly FieldSig _invalidSignature;
    private readonly ImmutableList<IMethodData> _methods;
    private readonly TypeSig _voidPtr;
    private int _variablesProcessed;

    public Corruption(Scrambler scrambler) : base(scrambler)
    {
        var sw = Stopwatch.StartNew();
        _enumType = Module.CorLibTypes.GetTypeRef(nameof(System), nameof(Enum));
        _methods = Database.Methods.Where(data => data.HasFeatures(VilensFeature.Corruption) && data is { Item.Body.HasVariables: true }).ToImmutableList();
        _voidPtr = new FnPtrSig(MethodSig.CreateStatic(Module.CorLibTypes.Void));
        Log.Debug("Found {0} methods in {1}.", _methods.Count, sw.Elapsed);
        if (Scrambler.Settings.AotSafeMode)
        {
            _invalidSignature = new FieldSig(_voidPtr);
        }
        else
        {
            // A 0-dim array can cause "System.BadImageFormatException: Invalid compressed integer".
            // This is useful when it happens in the decompiler, but it can also occur during AOT compilation.
            _invalidSignature = new FieldSig(new ArraySig(Module.CorLibTypes.Void, 0u));
        }
        if (_methods.Count > 0)
        {
            var e = CreateEnum(Module.CorLibTypes.SByte);
            _enum8 = new ValueTypeSig(e);
            e = CreateEnum(Module.CorLibTypes.UInt16);
            _enum16 = new ValueTypeSig(e);
            e = CreateEnum(Module.CorLibTypes.UInt32);
            _enum32 = new ValueTypeSig(e);
            e = CreateEnum(Module.CorLibTypes.UInt64);
            _enum64 = new ValueTypeSig(e);
        }
    }

    public override Logger Log { get; } = new Logger(nameof(Corruption));

    public override void Execute()
    {
        var types = Module.GetTypes().ToList();
        int count = types.AsParallel().Where(t => t.HasFeatures(VilensFeature.Corruption, Database.FeatureMap)).Count(Infect);
        Log.Info("Invalid code added to {0} types.", count);

        foreach (var method in _methods)
        {
            Cancellation.ThrowIfCancellationRequested();
            foreach (var arg in method.Item.Body.Variables)
            {
                var replacement = GetReplacementType(arg.Type);
                if (replacement != null)
                {
                    Log.Trace("Changing type of variable '{0}' from [{1}] to [{2}] in [{3}]", arg, arg.Type, replacement, method);
                    arg.Type = replacement;
                    _variablesProcessed++;
                }
            }
        }
        Log.Info("Replaced {0} local variable types", _variablesProcessed);
    }

    private bool Infect(TypeDef type)
    {
        if (type.IsEnum)
        {
            return false;
        }
        if (type.IsGlobalModuleType)
        {
            return false; // Infecting global module type results in a crash in .NET 9.0: System.BadImageFormatException: Enclosing type(s) not found for type '' in assembly 'XYZ'
        }
        type.NestedTypes.Add(CreateEnum2());
        return true;
    }

    private TypeDefUser CreateEnum2()
    {
        var type = new TypeDefUser(UTF8String.Empty, _enumType)
        {
            Attributes = TypeAttributes.Sealed | TypeAttributes.NestedFamily
        };
        type.Fields.Add(new FieldDefUser("value__", new FieldSig(Module.CorLibTypes.Int32), FieldAttributes.Public | FieldAttributes.RTSpecialName | FieldAttributes.SpecialName));
        type.Fields.Add(CreateInvalidField());
        return type;
    }

    private TypeDefUser CreateEnum(TypeSig typeSig)
    {
        var type = new TypeDefUser(UTF8String.Empty, _enumType)
        {
            Attributes = TypeAttributes.Sealed | TypeAttributes.NotPublic,
        };
        type.Fields.Add(new FieldDefUser("value__", new FieldSig(typeSig), FieldAttributes.Public | FieldAttributes.RTSpecialName | FieldAttributes.SpecialName));
        type.Fields.Add(CreateInvalidField());
        Module.AddAsNonNestedType(type);
        NamingHelper.Rename(type, Scrambler.Settings.NamingScheme);
        return type;
    }

    private TypeSig? GetReplacementType(TypeSig sig)
    {
        var type = sig.ElementType;

        TypeSig? newType = type switch
        {
            ElementType.I1 => _enum8,
            ElementType.U1 => _enum8,
            ElementType.Boolean => _enum8,

            ElementType.I2 => _enum16,
            ElementType.U2 => _enum16,
            ElementType.Char => _enum16,

            ElementType.I4 => _enum32,
            ElementType.U4 => _enum32,

            ElementType.I8 => _enum64,
            ElementType.U8 => _enum64,

            ElementType.I => _voidPtr,
            ElementType.U => _voidPtr,
            ElementType.Ptr => _voidPtr,
            ElementType.FnPtr => _voidPtr,

            ElementType.Class => Module.CorLibTypes.Object,
            ElementType.String => Module.CorLibTypes.Object,
            ElementType.Array => Module.CorLibTypes.Object,
            ElementType.SZArray => Module.CorLibTypes.Object,

            ElementType.GenericInst => IsReferenceType(sig.ToGenericInstSig()) ? Module.CorLibTypes.Object : null,
            _ => null
        };

        return newType;
    }

    private static bool IsReferenceType(GenericInstSig? type)
    {
        return type?.GenericType.IsClassSig == true;
    }

    private FieldDefUser CreateInvalidField()
    {
        var field = new FieldDefUser(UTF8String.Empty, _invalidSignature, FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal)
        {
            Constant = new ConstantUser(string.Empty)
        };
        return field;
    }
}
