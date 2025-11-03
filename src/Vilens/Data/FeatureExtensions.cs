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
        string[] segments = value.Split([';', ',', ' ', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return VilensFeature.None;
        return segments.Select(ParseOne).Aggregate((a, b) => a | b);

        static VilensFeature ParseOne(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return VilensFeature.None;
            }
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
