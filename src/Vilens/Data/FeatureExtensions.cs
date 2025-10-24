using dnlib.DotNet;

namespace Vilens.Data;
internal static class FeatureExtensions
{
    public static bool HasFeatures(this IDnlibDef def, VilensFeature features, FeatureMap map)
    {
        return (map.GetFeatures(def) & features) == features;
    }

    public static VilensFeature Parse(string value)
    {
        return value.Split([';', ','], StringSplitOptions.RemoveEmptyEntries).Select(ParseOne).Aggregate((a, b) => a | b);

        static VilensFeature ParseOne(string value)
        {
            if (Enum.TryParse<VilensFeature>(value.Trim(), true, out var feature))
            {
                return feature;
            }
            else
            {
                throw new ArgumentException($"Cannot parse obfuscation feature '{value}'");
            }
        }
    }
}
