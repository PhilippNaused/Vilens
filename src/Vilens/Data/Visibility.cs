using System.Reflection;

namespace Vilens.Data;

[Obfuscation(Feature = "Renaming", ApplyToMembers = true, Exclude = true)]
[Obfuscation(Feature = "Renaming", ApplyToMembers = false, Exclude = false)]
public enum Visibility
{
    Auto = -1,
    Private = 0,
    Internal = 1,
    Public = 2
}
