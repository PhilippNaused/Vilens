// Ignore Spelling: Vilens

using System.Collections.Immutable;
using System.Diagnostics;
using dnlib.DotNet;

namespace Vilens.Data;
internal sealed partial class Database
{
    private abstract class MemberData<T> : IMemberData<T> where T : IMemberDef
    {
        protected MemberData(T member, Database database)
        {
            Item = member;
            Name = member.Name;
            FullName = member.FullName;
            Features = database.FeatureMap.GetFeatures(member);
            Visibility = member.GetVisibility();
            Kind = member switch
            {
                MethodDef => MemberKind.Method,
                FieldDef => MemberKind.Field,
                PropertyDef => MemberKind.Property,
                EventDef => MemberKind.Event,
                TypeDef => MemberKind.Type,
                GenericParam => MemberKind.GenericParameter,
                _ => MemberKind.Unknown,
            };
            Debug.Assert(Kind is not MemberKind.Unknown);
            Log.Trace("{0} [{1}] has visibility {2} and features: {3}", Kind, Item, Visibility, Features);
        }

        public T Item { get; }
        public UTF8String Name { get; }
        public string FullName { get; }
        public Visibility Visibility { get; }
        public VilensFeature Features { get; }
        public MemberKind Kind { get; }

        public override bool Equals(object? obj) => obj is MemberData<T> data && Item.Equals(data.Item);
        public override int GetHashCode() => Item.GetHashCode();
        public override string ToString() => FullName;
    }

    private sealed class TypeData(TypeDef member, Database database) : MemberData<TypeDef>(member, database), ITypeData
    {
        public ImmutableArray<TypeRef> Refs { get; } = [.. database._typeRefMap[member]];
    }

    private sealed class MethodData(MethodDef member, Database database) : MemberData<MethodDef>(member, database), IMethodData
    {
        public ImmutableArray<MemberRef> Refs { get; } = [.. database._memberRefMap[member]];
    }

    private sealed class FieldData(FieldDef member, Database database) : MemberData<FieldDef>(member, database), IFieldData
    {
        public ImmutableArray<MemberRef> Refs { get; } = [.. database._memberRefMap[member]];
    }

    private sealed class PropertyData(PropertyDef member, Database database) : MemberData<PropertyDef>(member, database), IPropertyData
    { }

    private sealed class EventData(EventDef member, Database database) : MemberData<EventDef>(member, database), IEventData
    { }
}
