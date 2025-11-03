using System.Collections.Immutable;
using System.Diagnostics;
using dnlib.DotNet;
using Vilens.Data;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class Trimming : FeatureBase
{
    private readonly HashSet<IMemberRef> _toRemove;

    /// <inheritdoc />
    public Trimming(Scrambler scrambler) : base(scrambler)
    {
        var sw = Stopwatch.StartNew();
        _toRemove =
        [
            .. Database.MemberFinder.MemberRefs.Keys,
            .. Database.MemberFinder.MethodSpecs.Keys,
            .. Database.MemberFinder.TypeSpecs.Keys,
            .. Database.Members.Select(m => m.Item)
        ];

        var toKeep = Database.Members.AsParallel().Where(
            member => member.Visibility > Scrambler.Scope
            || !member.HasFeatures(VilensFeature.Trimming)
            || member.IsVirtual() // fix this
        ).Select(x => x.Item).ToList();
        Log.Debug("Found {0} members that need to be kept", toKeep.Count);
        var entryPoint = Module.EntryPoint;
        if (entryPoint is not null)
        {
            Log.Debug("Excluding entrypoint {0}", entryPoint);
            Exclude(entryPoint);
        }
        Exclude(Module.GlobalType);
        Exclude2(Module);
        foreach (var member in toKeep)
        {
            Exclude(member);
        }
        int c = _toRemove.RemoveWhere(m => m is not IMemberDef);
        Log.Trace("Removed {0} extra elements that are not IMemberDef", c);

        foreach (var member in _toRemove.ToImmutableList()) // create copy to avoid modification during enumeration
        {
            if (Database.UserStrings.Contains(member.Name) || Database.UserStrings.Contains(member.FullName))
            {
                Log.Trace("Skipping [{0}] because it's name was found on the user string heap. It may be referenced via reflection.", member);
                Exclude(member);
            }
        }

        Log.Debug("Found {0} members that can be removed", _toRemove.Count);
        Log.Debug("Filtered data in {0}.", sw.Elapsed);
    }

    private void Exclude3(MemberRef memberRef)
    {
        if (!_toRemove.Remove(memberRef))
        {
            return;
        }
        Log.Trace("Excluding non-module ref {0}", memberRef);
    }

    private void Exclude2(IHasCustomAttribute? attribute)
    {
        if (attribute is null)
        {
            return;
        }

        foreach (var ca in attribute.CustomAttributes)
        {
            Exclude(ca.AttributeType);
            Exclude(ca.Constructor);
        }
    }

    private void ExcludeSig(TypeSig? sig)
    {
        if (sig is null)
        {
            return;
        }
        switch (sig)
        {
            case GenericInstSig sig2:
                ExcludeSig(sig2.GenericType);
                foreach (var argument in sig2.GenericArguments)
                {
                    ExcludeSig(argument);
                }
                break;
            case GenericSig sig2:
                Exclude(sig2.GenericParam);
                break;
            case TypeDefOrRefSig sig2:
                Exclude(sig2.TypeDefOrRef);
                break;
            case ByRefSig sig2:
                ExcludeSig(sig2.Next);
                break;
            case SZArraySig sig2:
                ExcludeSig(sig2.Next);
                break;
            case ArraySig sig2:
                ExcludeSig(sig2.Next);
                break;
            case PtrSig sig2:
                ExcludeSig(sig2.Next);
                break;
            case PinnedSig sig2:
                ExcludeSig(sig2.Next);
                break;
            case CModReqdSig sig2:
                Exclude(sig2.Modifier);
                ExcludeSig(sig2.Next);
                break;
            default:
                throw new InvalidOperationException($"Unexpected type: {sig.GetType()}, [{sig}]");
        }
    }

    private void Exclude(IMemberRef? member)
    {
        if (member is null || !_toRemove.Remove(member))
        {
            // short circuit to prevent infinite recursion.
            return;
        }
        Log.Trace("{0} [{1}] will not be trimmed", member.GetType().Name, member);
        Exclude(member.DeclaringType);
        switch (member)
        {
            case FieldDef field:
                ExcludeSig(field.FieldType);
                break;
            case PropertyDef property:
                foreach (var method in property.GetMethods)
                {
                    Exclude(method);
                }
                foreach (var method in property.SetMethods)
                {
                    Exclude(method);
                }
                foreach (var method in property.OtherMethods)
                {
                    Exclude(method);
                }
                break;
            case EventDef @event:
                Exclude(@event.AddMethod);
                Exclude(@event.RemoveMethod);
                Exclude(@event.InvokeMethod);
                foreach (var method in @event.OtherMethods)
                {
                    Exclude(method);
                }
                break;
            case TypeSpec typeSpec:
                Exclude(typeSpec.ResolveTypeDef());
                Exclude(typeSpec.GetBaseType());
                if (typeSpec.TryGetGenericInstSig() is { } genSig)
                {
                    foreach (var arg in genSig.GenericArguments)
                    {
                        ExcludeSig(arg);
                    }
                }
                break;
            case TypeDef type:
                if (type.IsEnum)
                {
                    foreach (var value in type.Fields)
                    {
                        Exclude(value);
                    }
                }
                if (type.HasCustomAttribute("System.Runtime.CompilerServices.InlineArrayAttribute"))
                {
                    Log.Trace("Type [{0}] has the InlineArrayAttribute, so the fields cannot be trimmed", type);
                    // don't remove the fields because of:
                    // System.TypeLoadException: InlineArrayAttribute requires that the target type has a single instance field.
                    foreach (var field in type.Fields)
                    {
                        Exclude(field);
                    }
                }
                foreach (var gen in type.GenericParameters)
                {
                    Exclude(gen);
                }
                foreach (var inter in type.Interfaces)
                {
                    Exclude(inter.Interface);
                    Exclude2(inter);
                }
                Exclude(type.BaseType);
                Exclude(type.FindStaticConstructor());

                foreach (var gen in type.GenericParameters)
                {
                    Exclude(gen);
                }

                break;
            case MethodSpec methodSpec:
                Exclude(methodSpec.Method);
                foreach (var arg in methodSpec.GenericInstMethodSig.GenericArguments)
                {
                    ExcludeSig(arg);
                }
                break;
            case MethodDef method:

                foreach (var over in method.Overrides) // do we need this?
                {
                    Exclude(over.MethodBody);
                    Exclude(over.MethodDeclaration);
                }

                foreach (var gen in method.GenericParameters)
                {
                    Exclude(gen);
                }

                ExcludeSig(method.ReturnType);

                foreach (var para in method.Parameters)
                {
                    ExcludeSig(para.Type);
                    Exclude2(para.ParamDef);
                }

                foreach (var para in method.ParamDefs)
                {
                    Exclude2(para);
                }

                if (method.Body is not null)
                {
                    foreach (var vars in method.Body.Variables)
                    {
                        ExcludeSig(vars.Type);
                    }

                    foreach (var instr in method.Body.Instructions)
                    {
                        ExcludeSig(instr.Operand as TypeSig);
                        Exclude(instr.Operand as IMemberRef);
                    }
                }

                break;
            case GenericParam param:
                foreach (var constr in param.GenericParamConstraints)
                {
                    Exclude(constr.Constraint);
                    Exclude2(constr);
                }
                break;
            case MemberRef @ref:
                var resolved = @ref.Resolve();
                if (resolved is not null && resolved.Module == Module)
                {
                    Exclude(resolved);
                }
                else
                {
                    Exclude3(@ref);
                }

                break;
            default:
                throw new InvalidOperationException($"Unknown type: {member.GetType()}, [{member}]");
        }

        Exclude2(member as IHasCustomAttribute);
    }

    public override Logger Log { get; } = new(nameof(Trimming));

    /// <inheritdoc />
    public override void Execute()
    {
        foreach (var member in _toRemove)
        {
            Cancellation.ThrowIfCancellationRequested();
            Log.Trace("Removing {0} [{1}]", member.GetType().Name, member);

            switch (member)
            {
                case FieldDef field:
                    field.DeclaringType = null;
                    break;
                case PropertyDef property:
                    property.DeclaringType = null;
                    break;
                case EventDef @event:
                    @event.DeclaringType = null;
                    break;
                case MethodDef method:
                    method.DeclaringType = null;
                    break;
                case TypeDef type:
                    type.DeclaringType = null;
                    _ = Module.Types.Remove(type);
                    break;
                case GenericParam:
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected type: {member.GetType()}, [{member}]");
            }
        }

        Log.Info("Removed {0} members", _toRemove.Count);

        //foreach (var type in Database.Types.Select(x => x.Item).Where(Module.GetTypes().Contains))
        //{
        //    if (!(type.IsAbstract && type.IsSealed) && !type.IsValueType) // is not static
        //    {
        //        if (!type.FindInstanceConstructors().Any())
        //        {
        //            Log.Warn("type {0} has no instance constructors", type);
        //        }
        //    }
        //}
    }
}
