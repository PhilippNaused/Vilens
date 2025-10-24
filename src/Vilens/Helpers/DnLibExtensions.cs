using System.Diagnostics;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using Vilens.Data;
using Vilens.Logging;

namespace Vilens.Helpers;

internal static class DnLibExtensions
{
    private static Logger Log { get; } = new Logger(nameof(DnLibExtensions));

    public static Instruction Append(this IList<Instruction> list, Instruction inst)
    {
        Debug.Assert(list is not null);
        Debug.Assert(inst is not null);

        list!.Add(inst!);
        return inst!;
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> list2)
    {
        Debug.Assert(list is not null);
        Debug.Assert(list2 is not null);

        foreach (var inst in list2!)
        {
            list!.Add(inst);
        }
    }

    public static bool IsUnconditional(this FlowControl flow)
    {
        return flow is FlowControl.Branch or FlowControl.Return or FlowControl.Throw;
    }

    public static bool IsBranch(this Instruction instr)
    {
        return instr.Operand is Instruction or IList<Instruction>;
    }

    public static void Replace(this Instruction instr, Instruction other)
    {
        instr.OpCode = other.OpCode;
        instr.Operand = other.Operand;
        instr.Offset = other.Offset;
        instr.SequencePoint = other.SequencePoint;
    }

    public static TypeRef AddCoreRef(this ModuleDef module, Type type)
    {
        Log.Debug("Getting reference to {type}", type);
        return module.CorLibTypes.GetTypeRef(type.Namespace, type.Name);
    }

    public static TypeRef AddRef(this ModuleDef module, Type type, IResolutionScope scope)
    {
        Log.Debug("Getting reference to {type} in {scope}", type, scope);
        var typeRef = module.GetTypeRefs().FirstOrDefault(t => t.FullName == type.FullName && t.ResolutionScope == scope);
        if (typeRef is not null)
        {
            Log.Trace("Using existing reference to {type} in {scope}", type, scope);
            return typeRef;
        }
        Log.Debug("Adding reference to {type} from {scope}", type, scope);
        return new TypeRefUser(module, type.Namespace, type.Name, scope);
    }

    public static AssemblyRef AddAssemblyRef(this ModuleDef module, AssemblyName asmName)
    {
        if (module.GetAssemblyRef(asmName.Name) is { } asmRef)
            return asmRef;
        var pkt = asmName.GetPublicKeyToken();
        if (pkt is null || pkt.Length == 0)
            pkt = null;
        Log.Debug("Adding reference to {assemblyName}", asmName);
        return module.UpdateRowId(new AssemblyRefUser(asmName.Name, asmName.Version, PublicKeyBase.CreatePublicKeyToken(pkt), asmName.CultureInfo?.Name ?? string.Empty));
    }

    public static TypeRef AddReference(this ModuleDef module, Type type, string netStandard, string netCore, string netFramework)
    {
        Log.Debug("Getting reference to {type}", type);
        var systemRef = module.CorLibTypes.AssemblyRef;

        var shortName = systemRef.Name.String switch
        {
            "netstandard" => netStandard,
            "System.Runtime" => netCore,
            "mscorlib" => netFramework,
            _ => throw new NotSupportedException($"Unknown core library: '{systemRef}'"),
        };

        if (shortName == systemRef.Name)
        {
            Log.Trace("Using core reference for {type}", type);
            return module.AddCoreRef(type);
        }

        var assemblyName = new AssemblyName(systemRef.FullNameToken)
        {
            Name = shortName,
        };
        Log.Debug("Adding reference to {type} from {assemblyName}", type, assemblyName);
        var asmRef = module.AddAssemblyRef(assemblyName);
        return module.AddRef(type, asmRef);
    }

    public static void Optimize(this IList<Instruction> list)
    {
        list.SimplifyBranches();
        list.OptimizeMacros();
        list.OptimizeBranches();
    }

    public static IEnumerable<IMemberDef> GetMembers(this TypeDef type)
    {
        foreach (var method in type.Methods)
        {
            yield return method;
        }
        foreach (var field in type.Fields)
        {
            yield return field;
        }
        foreach (var property in type.Properties)
        {
            yield return property;
        }
        foreach (var @event in type.Events)
        {
            yield return @event;
        }
    }

    public static bool IsRuntimeSpecialName(this IMemberDef member)
    {
        return member switch
        {
            MethodDef method => method.IsRuntimeSpecialName,
            FieldDef field => field.IsRuntimeSpecialName,
            PropertyDef property => property.IsRuntimeSpecialName,
            EventDef @event => @event.IsRuntimeSpecialName,
            TypeDef type => type.IsRuntimeSpecialName,
            _ => false,
        };
    }

    public static bool IsVirtual(this IMemberDef member)
    {
        return member switch
        {
            MethodDef method => method.IsVirtual,
            _ => false,
        };
    }

    public static HashSet<string> ToHashSet(this USStream stream) => [.. ReadAll(stream)];

    private static IEnumerable<string> ReadAll(USStream stream)
    {
        var reader = stream.CreateReader();
        while (reader.BytesLeft > 0)
        {
            if (!reader.TryReadCompressedUInt32(out uint length))
                break;
            if (!reader.CanRead(length))
                break;
            string str = null!;
            try
            {
                str = reader.ReadUtf16String((int)(length / 2));
            }
            catch (ArgumentException e)
            {
                // It's possible that an exception is thrown when converting a char* to a string.
                Log.Error(e, "Error while parsing '#US' table");
            }
            if (str != null)
            {
                yield return str;
            }
        }
    }

    public static bool HasCustomAttribute<T>(this IHasCustomAttribute member) where T : Attribute
    {
        return member.HasCustomAttribute(typeof(T).FullName!);
    }

    public static bool HasCustomAttribute(this IHasCustomAttribute member, string fullName)
    {
        return member.CustomAttributes.Any(a => a.TypeFullName == fullName);
    }

    public static bool WasTrimmed(this IMemberData<IMemberDef> member)
    {
        return member.Item.WasTrimmed();
    }

    public static bool WasTrimmed(this IMemberDef member)
    {
        if (member.Module is null)
            return true;
        if (member.DeclaringType is not null)
            return false;
        if (member is not TypeDef type)
            return true;
        // member is type and not nested => check if it's in the module
        return !member.Module.Types.Contains(type);
    }
}
