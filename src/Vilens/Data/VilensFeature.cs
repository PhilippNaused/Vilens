using System.Reflection;

namespace Vilens.Data;

[Flags]
[Obfuscation(Feature = "Renaming", ApplyToMembers = true, Exclude = true)]
[Obfuscation(Feature = "Renaming", ApplyToMembers = false, Exclude = false)]
public enum VilensFeature
{
    None = 0b0,
    Renaming = 0b1,
    AttributeCleaning = 0b10,
    PropertyInline = 0b100,
    Corruption = 0b1000,
    ControlFlow = 0b1_0000,
    Trimming = 0b10_0000,
    StringHiding = 0b100_0000,
    All = Renaming | AttributeCleaning | PropertyInline | Corruption | ControlFlow | Trimming | StringHiding,
}
