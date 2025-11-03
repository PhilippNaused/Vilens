using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using dnlib.DotNet;
using Vilens.Logging;

namespace Vilens.Data;

internal class FeatureMap
{
    private static readonly string s_AttributeName = typeof(ObfuscationAttribute).FullName!;
    private readonly ConcurrentDictionary<IMemberDef, VilensFeature> _memberCache = new();

    private readonly ConcurrentDictionary<ModuleDef, VilensFeature> _moduleCache = new();
    private readonly ConcurrentDictionary<CustomAttribute, IHasCustomAttribute> _toRemove = new();
    private readonly ConcurrentDictionary<TypeDef, (VilensFeature Self, VilensFeature Inherited)> _typeCache = new();

    private int _attributesRead;

    public FeatureMap(ModuleDef module, VilensFeature features)
    {
        features = Combine(features, module.Assembly);
        features = Combine(features, module);
        _moduleCache[module] = features;
    }

    private static Logger Log { get; } = new Logger(nameof(FeatureMap));

    internal void Cleanup()
    {
        foreach (var pair in _toRemove)
        {
            var b = pair.Value.CustomAttributes.Remove(pair.Key);
            Debug.Assert(b);
            Log.Trace("Removed ObfuscationAttribute from [{0}]", pair.Value);
        }

        Log.Debug("Found {0} ObfuscationAttributes. {1} were removed.", _attributesRead, _toRemove.Count);
    }

    private static VilensFeature GetFeatureFlags(ObfuscationAttribute attribute)
    {
        return attribute.Feature is null ? VilensFeature.All : FeatureExtensions.Parse(attribute.Feature);
    }

    private VilensFeature GetFeaturesModule(ModuleDef module)
    {
        return _moduleCache[module];
    }

    private (VilensFeature Self, VilensFeature Inherited) GetFeaturesType(TypeDef type)
    {
        return _typeCache.GetOrAdd(type, t =>
        {
            VilensFeature parentFeatures;
            if (t.IsNested)
            {
                // Type is nested => the declaring type is the parent
                parentFeatures = GetFeaturesType(t.DeclaringType).Inherited;
            }
            else
            {
                // Type is not nested => the module is the parent
                parentFeatures = GetFeaturesModule(t.Module);
            }
            var self = Combine(parentFeatures, t, useApplyToMembers: false);
            var inherited = Combine(parentFeatures, t, useApplyToMembers: true);
            return (self, inherited);
        });
    }

    private VilensFeature GetFeaturesMember(IMemberDef member)
    {
        Debug.Assert(member is not TypeDef);
        return _memberCache.GetOrAdd(member, m =>
        {
            VilensFeature parentFeatures = GetFeaturesType(m.DeclaringType).Inherited;
            return Combine(parentFeatures, m);
        });
    }

    public VilensFeature GetFeatures(IDnlibDef def)
    {
        switch (def)
        {
            case AssemblyDef assembly:
                return GetFeaturesModule(assembly.ManifestModule);
            case ModuleDef moduleDef:
                return GetFeaturesModule(moduleDef);
            case TypeDef type:
                return GetFeaturesType(type).Self;
            case IMemberDef member:
                return GetFeaturesMember(member);
            default:
                Log.Error("Unknown IDnlibDef [{0}] of type [{1}]", def, def.GetType());
                Debug.Fail("Unknown Type");
                return VilensFeature.None;
        }
    }

    private VilensFeature Combine(VilensFeature parentFeatures, IHasCustomAttribute def, bool useApplyToMembers = false)
    {
        var flags = parentFeatures;
        foreach (var attr in GetObfuscationAttributes(def))
        {
            if (attr.ApplyToMembers || !useApplyToMembers)
            {
                flags = Combine(flags, attr);
            }
        }
        return flags;
    }

    private static VilensFeature Combine(VilensFeature parentFeatures, ObfuscationAttribute? childAttribute)
    {
        if (childAttribute is null)
        {
            return parentFeatures;
        }
        var childFlags = GetFeatureFlags(childAttribute);
        VilensFeature childFeatures;
        if (childAttribute.Exclude)
        {
            // Only flags set by the parent AND NOT excluded by the child.
            childFeatures = parentFeatures & ~childFlags;
        }
        else
        {
            // Only flags set by the parent OR by the child.
            childFeatures = parentFeatures | childFlags;
        }
        return childFeatures;
    }

    private List<ObfuscationAttribute> GetObfuscationAttributes(IHasCustomAttribute def)
    {
        List<ObfuscationAttribute> list = [];
        foreach (CustomAttribute attribute in def.CustomAttributes.Where(ca => ca.TypeFullName == s_AttributeName))
        {
            Debug.Assert(attribute != null);
            ObfuscationAttribute obfuscationAttribute = Parse(attribute!);
            Log.Trace("[{0}] has ObfuscationAttribute with ApplyToMembers={1}, Exclude={2}, Feature={3}, StripAfterObfuscation={4}",
                def, obfuscationAttribute.ApplyToMembers, obfuscationAttribute.Exclude, obfuscationAttribute.Feature, obfuscationAttribute.StripAfterObfuscation);
            _ = Interlocked.Increment(ref _attributesRead);
            if (obfuscationAttribute.StripAfterObfuscation && _toRemove.TryAdd(attribute!, def))
            {
                Log.Trace("ObfuscationAttribute will be removed from [{0}]", def);
            }
            list.Add(obfuscationAttribute);
        }
        return list;
    }

    private static ObfuscationAttribute Parse(CustomAttribute custom)
    {
        Debug.Assert(custom != null);
        Debug.Assert(custom!.TypeFullName == s_AttributeName);
        ObfuscationAttribute attribute = new();
        foreach (var property in custom.Properties)
        {
            switch (property.Name.String)
            {
                case nameof(ObfuscationAttribute.ApplyToMembers):
                    attribute.ApplyToMembers = (bool)property.Value;
                    break;
                case nameof(ObfuscationAttribute.Feature):
                    attribute.Feature = (UTF8String)property.Value;
                    break;
                case nameof(ObfuscationAttribute.Exclude):
                    attribute.Exclude = (bool)property.Value;
                    break;
                case nameof(ObfuscationAttribute.StripAfterObfuscation):
                    attribute.StripAfterObfuscation = (bool)property.Value;
                    break;
                default:
                    Debug.Fail($"Unknown property: {property}");
                    break;
            }
        }
        return attribute;
    }
}
