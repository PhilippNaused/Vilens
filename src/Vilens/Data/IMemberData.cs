using System.Collections.Immutable;
using dnlib.DotNet;
using Vilens.Helpers;

namespace Vilens.Data;
internal interface IMemberData<out T> where T : IMemberDef
{
    T Item { get; }
    UTF8String Name { get; }
    string FullName { get; }
    Visibility Visibility { get; }
    VilensFeature Features { get; }
    MemberKind Kind { get; }
}

internal static class MemberDataExtensions
{
    public static bool IsRuntimeSpecialName<T>(this IMemberData<T> data) where T : IMemberDef
    {
        return data.Item.IsRuntimeSpecialName();
    }

    public static bool IsVirtual<T>(this IMemberData<T> data) where T : IMemberDef
    {
        return data.Item.IsVirtual();
    }

    public static bool HasFeatures<T>(this IMemberData<T> data, VilensFeature feature) where T : IMemberDef
    {
        return (data.Features & feature) == feature;
    }
}

internal interface ITypeData : IMemberData<TypeDef>
{
    ImmutableArray<TypeRef> Refs { get; }
}

internal interface IHasMemberRefData
{
    ImmutableArray<MemberRef> Refs { get; }
}

internal interface IMethodData : IMemberData<MethodDef>, IHasMemberRefData
{ }

internal interface IFieldData : IMemberData<FieldDef>, IHasMemberRefData
{ }

internal interface IPropertyData : IMemberData<PropertyDef>
{ }

internal interface IEventData : IMemberData<EventDef>
{ }
