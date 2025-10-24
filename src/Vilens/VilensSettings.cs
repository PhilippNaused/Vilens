using Vilens.Data;

namespace Vilens;

public sealed class VilensSettings
{
    public required VilensFeature Features { get; init; }
    public required Visibility Scope { get; set; }
    public NamingScheme NamingScheme { get; init; } = NamingScheme.Default;
    public bool DelaySign { get; init; }
#pragma warning disable CA1819 // Properties should not return arrays
    public byte[]? StrongNamingKey { get; init; }
#pragma warning restore CA1819 // Properties should not return arrays
}
