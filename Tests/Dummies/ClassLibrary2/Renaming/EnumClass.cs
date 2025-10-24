using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public static class EnumClass
{
    [Obfuscation(Feature = "Renaming", ApplyToMembers = true, Exclude = true)]
    [Obfuscation(Feature = "Renaming", ApplyToMembers = false, Exclude = false)]
    private enum Enum1
    {
        Value1 = 1, Value2 = 2, Value3 = 3,
    }

    [Obfuscation(Feature = "Renaming", ApplyToMembers = true, Exclude = false)]
    [Obfuscation(Feature = "Renaming", ApplyToMembers = false, Exclude = true)]
    private enum Enum2
    {
        Value1 = 1, Value2 = 2, Value3 = 3,
    }
}
