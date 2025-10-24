// Ignore Spelling: Vilens

using System.Collections.Immutable;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens.Data;

internal sealed partial class Database
{
    private readonly ILookup<IMemberDef?, MemberRef> _memberRefMap;
    private readonly ILookup<TypeDef?, TypeRef> _typeRefMap;

    internal Database(ModuleDefMD module, VilensFeature defaultFeatures, CancellationToken cancellation)
    {
        Log.Trace("Filling Database");

        MemberFinder = new MemberFinder().FindAll(module);
        FeatureMap = new FeatureMap(module, defaultFeatures);

        _memberRefMap = MemberFinder.MemberRefs.Keys.ToLookup(memberRef => memberRef.Resolve() as IMemberDef);
        _typeRefMap = MemberFinder.TypeRefs.Keys.ToLookup(typeRef => typeRef.Resolve())!;
        UserStrings = module.USStream.ToHashSet();
        Log.Debug("Found {c} strings in the #US stream", UserStrings.Count);
        cancellation.ThrowIfCancellationRequested();
        Fields = MemberFinder.FieldDefs.Keys.Select(d => new FieldData(d, this)).ToImmutableList<IFieldData>();
        Methods = MemberFinder.MethodDefs.Keys.Select(d => new MethodData(d, this)).ToImmutableList<IMethodData>();
        Properties = MemberFinder.PropertyDefs.Keys.Select(d => new PropertyData(d, this)).ToImmutableList<IPropertyData>();
        Events = MemberFinder.EventDefs.Keys.Select(d => new EventData(d, this)).ToImmutableList<IEventData>();
        Types = MemberFinder.TypeDefs.Keys.Select(d => new TypeData(d, this)).ToImmutableList<ITypeData>();
        cancellation.ThrowIfCancellationRequested();
        Log.Debug("Found {c} fields", Fields.Count);
        Log.Debug("Found {c} methods", Methods.Count);
        Log.Debug("Found {c} properties", Properties.Count);
        Log.Debug("Found {c} events", Events.Count);
        Log.Debug("Found {c} types", Types.Count);

        Members = [.. Types, .. Fields, .. Methods, .. Properties, .. Events];

        Log.Debug("Found {c} members", Members.Count);

        foreach (var data in Methods)
        {
            cancellation.ThrowIfCancellationRequested();
            var method = data.Item;
            var constants = method.Body?.PdbMethod?.Scope?.Constants;
            if (constants is not null)
            {
                for (int i = constants.Count - 1; i >= 0; i--)
                {
                    // workaround for https://github.com/0xd4d/dnlib/issues/550
                    // remove any constant with a reference type where the value is not a byte[] or null.
                    var constant = constants[i];
                    if (constant.Type?.ElementType is ElementType.Class &&
                        constant.Value is not null &&
                        constant.Value is not byte[])
                    {
                        Log.Debug("Removing PDB constant {constant} from {method}", constant, method);
                        constants.RemoveAt(i);
                    }
                }
            }
        }
    }

    private static Logger Log { get; } = new Logger(nameof(Database));

    #region Properties

    public ImmutableList<ITypeData> Types { get; }
    public ImmutableList<IFieldData> Fields { get; }
    public ImmutableList<IMethodData> Methods { get; }
    public ImmutableList<IPropertyData> Properties { get; }
    public ImmutableList<IEventData> Events { get; }
    public ImmutableList<IMemberData<IMemberDef>> Members { get; }

    /// <summary>
    /// The content of the <see cref="USStream"/>
    /// </summary>
    public HashSet<string> UserStrings { get; }

    public FeatureMap FeatureMap { get; }
    public MemberFinder MemberFinder { get; }

    #endregion Properties
}
