using System.Collections.Immutable;
using System.Diagnostics;
using dnlib.DotNet;

namespace Vilens.Helpers;

internal readonly struct NamingHelper
{
    private static readonly ImmutableArray<char> IllegalChars = [.. " .:!?&()`'\"*/\x037E;\e"];
    private static readonly ImmutableArray<char> LegalChars = [.. "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"];
    private readonly IEnumerator<string> m_Enumerator;
    private readonly HashSet<string>? m_UsedTypeNames;
    private readonly ImmutableArray<char> m_Chars;
    private readonly int m_MinLength;

    internal NamingHelper(NamingScheme scheme, HashSet<string>? usedTypeNames = null)
    {
        m_UsedTypeNames = usedTypeNames;
        m_Chars = scheme switch
        {
            NamingScheme.Invalid => IllegalChars,
            NamingScheme.Default => LegalChars,
            _ => throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null)
        };
        m_MinLength = scheme switch
        {
            NamingScheme.Invalid => 0,
            NamingScheme.Default => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(scheme), scheme, null)
        };
        // ReSharper disable once GenericEnumeratorNotDisposed
        m_Enumerator = CombinationsWithRepetition().GetEnumerator();
    }

    public static void Rename(IMemberDef member, NamingScheme scheme, HashSet<string>? usedTypeNames = null)
    {
        var naming = new NamingHelper(scheme, usedTypeNames);
        UTF8String newName;
        do
        {
            newName = naming.GetNextName();
        } while (naming.IsNameIsUse(member, newName));
        member.Name = newName;
    }

    public UTF8String GetNextName()
    {
        var hasNext = m_Enumerator.MoveNext();
        Debug.Assert(hasNext);
        var name = m_Enumerator.Current;
        return new UTF8String(name);
    }

    private IEnumerable<string> CombinationsWithRepetition()
    {
        for (int i = m_MinLength; ; i++)
        {
            foreach (var name in CombinationsWithRepetition(i))
            {
                yield return name;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private IEnumerable<string> CombinationsWithRepetition(int length)
    {
        Debug.Assert(length >= 0);
        if (length <= 0)
        {
            yield return string.Empty;
        }
        else
        {
            foreach (var c in m_Chars)
            {
                foreach (var s in CombinationsWithRepetition(length - 1))
                {
                    yield return s + c;
                }
            }
        }
    }

    private bool IsNameIsUse(IMemberDef member, UTF8String newName)
    {
        TypeDef type = member.DeclaringType;
        return member switch
        {
            MethodDef method => type.Methods.Any(m => m != member && m.Name.Equals(newName)), // TODO: overloads
            FieldDef field => type.Fields.Any(m => m != member && m.Name.Equals(newName)), // TODO: overloads
            PropertyDef property => type.FindProperty(newName, property.PropertySig) != null,
            EventDef @event => type.FindEvent(newName, @event.EventType) != null,
            TypeDef type2 => IsTypeNameInUse(type2, newName),
            GenericParam genericParam => genericParam.Owner.GenericParameters.Any(gp => gp != genericParam && gp.Name.Equals(newName)),
            _ => throw new ArgumentException($"Unknown type: {member.GetType()}", nameof(member)),
        };
    }

    private bool IsTypeNameInUse(TypeDef type, UTF8String newName)
    {
        if (type.IsNested)
        {
            return type.DeclaringType.NestedTypes.Any(t => t.Name.Equals(newName));
        }

        //var fullName = type.Namespace.Length > 0
        //    ? $"{type.Namespace}.{newName}"
        //    : newName.String;

        // non-nested type
        if (m_UsedTypeNames is not null)
        {
            return m_UsedTypeNames.Contains(newName);
        }
        return type.Module.FindNormal(newName) is not null;
    }
}
