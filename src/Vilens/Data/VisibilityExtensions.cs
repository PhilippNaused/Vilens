using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Vilens.Data;

internal static class VisibilityExtensions
{
    private static readonly ConcurrentDictionary<IMemberDef, Visibility> s_VisibilityCache = new();

    private static readonly string[] s_GeneratedCodeAttributeNames =
    [
        typeof(CompilerGeneratedAttribute).FullName!,
        typeof(System.CodeDom.Compiler.GeneratedCodeAttribute).FullName!,
    ];

    private static readonly ConcurrentDictionary<TypeDef, bool> s_IsHiddenTypeCache = new();

    public static bool IsInScope(this IMemberDef member, Visibility visibility)
    {
        Debug.Assert(visibility != Visibility.Auto);
        if (visibility == Visibility.Public)
        {
            return true;
        }
        return GetVisibility(member) <= visibility;
    }

    public static Visibility GetVisibility(this IMemberDef member)
    {
        return s_VisibilityCache.GetOrAdd(member, GetVisibilityInternal);
    }

    private static Visibility Max(params ReadOnlySpan<IList<MethodDef>> methodLists)
    {
        var visibility = Visibility.Private;
        foreach (var list in methodLists)
        {
            foreach (var method in list)
            {
                var vis = GetVisibility(method);
                if (vis > visibility)
                {
                    visibility = vis;
                    if (visibility == Visibility.Public)
                    {
                        return visibility;
                    }
                }
            }
        }
        return visibility;
    }

    private static Visibility Max(params ReadOnlySpan<MethodDef?> methods)
    {
        var visibility = Visibility.Private;

        foreach (var method in methods)
        {
            if (method is null)
            {
                continue;
            }
            var vis = GetVisibility(method);
            if (vis > visibility)
            {
                visibility = vis;
                if (visibility == Visibility.Public)
                {
                    return visibility;
                }
            }
        }
        return visibility;
    }

    private static Visibility GetVisibilityInternal(IMemberDef member)
    {
        if (IsHidden(member))
        {
            return Visibility.Private;
        }

        var visibility = member switch
        {
            TypeDef type => ToVisibility(type.Attributes),
            FieldDef field => ToVisibility(field.Access),
            MethodDef method => ToVisibility(method.Access),
            PropertyDef property => Max(property.GetMethods, property.SetMethods, property.OtherMethods),
            EventDef @event => Max(@event.AddMethod, @event.RemoveMethod, @event.InvokeMethod),
            _ => Visibility.Public,
        };

        if (visibility > Visibility.Private && member.DeclaringType is { } parent)
        {
            var parentVis = GetVisibility(parent);
            if (parentVis < visibility)
            {
                // public members in an internal type are treated as internal
                return parentVis;
            }
        }

        return visibility;
    }

    private static Visibility ToVisibility(FieldAttributes att)
    {
        return (att & FieldAttributes.FieldAccessMask) switch
        {
            <= FieldAttributes.Private => Visibility.Private,
            <= FieldAttributes.Assembly => Visibility.Internal,
            _ => Visibility.Public
        };
    }

    private static Visibility ToVisibility(TypeAttributes att)
    {
        return (att & TypeAttributes.VisibilityMask) switch
        {
            TypeAttributes.NotPublic => Visibility.Internal,
            TypeAttributes.Public => Visibility.Public,
            TypeAttributes.NestedPublic => Visibility.Public,
            TypeAttributes.NestedPrivate => Visibility.Private,
            TypeAttributes.NestedFamily => Visibility.Public,
            TypeAttributes.NestedAssembly => Visibility.Internal,
            TypeAttributes.NestedFamANDAssem => Visibility.Internal,
            TypeAttributes.NestedFamORAssem => Visibility.Public,
            _ => throw new ArgumentOutOfRangeException(nameof(att))
        };
    }

    private static Visibility ToVisibility(MethodAttributes att)
    {
        return (att & MethodAttributes.MemberAccessMask) switch
        {
            <= MethodAttributes.Private => Visibility.Private,
            <= MethodAttributes.Assembly => Visibility.Internal,
            _ => Visibility.Public
        };
    }

    private static bool IsHidden(IMemberDef member)
    {
        return member switch
        {
            TypeDef type => IsHiddenType(type),
            _ => IsHiddenType(member.DeclaringType),
        };
    }

    private static bool IsHiddenType(TypeDef type)
    {
        return s_IsHiddenTypeCache.GetOrAdd(type, IsHiddenTypeInternal);
    }

    private static bool IsHiddenTypeInternal(TypeDef type)
    {
        if (type.IsNotPublic && type.Name.Contains("<") && type.CustomAttributes.Any(ca => s_GeneratedCodeAttributeNames.Contains(ca.TypeFullName)))
        {
            // We can be sure the type is not referenced outside the assembly if it has a '<' in the name.
            return true;
        }

        return type.IsNestedPrivate;
    }
}
