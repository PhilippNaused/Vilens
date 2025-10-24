#pragma warning disable

using System.Reflection;

namespace ClassLibrary1;

public class ExcludedClass
{
    [Obfuscation(Exclude = true)]
    private int ExcludedField1;

    [Obfuscation(Exclude = false)]
    private int ExcludedField2;

    private int ExcludedField3;

    [Obfuscation()]
    private int ExcludedField4;

    [Obfuscation(Exclude = true)]
    private void ExcludedMethod1() { }

    [Obfuscation(Exclude = false)]
    private void ExcludedMethod2() { }

    private void ExcludedMethod3() { }

    [Obfuscation()]
    private void ExcludedMethod4() { }
}

[Obfuscation(Exclude = true)]
public class ExcludedClass2
{
    [Obfuscation(Exclude = true)]
    private int ExcludedField1;

    [Obfuscation(Exclude = false)]
    private int ExcludedField2;

    private int ExcludedField3;

    [Obfuscation()]
    private int ExcludedField4;

    [Obfuscation(Exclude = true)]
    private void ExcludedMethod1() { }

    [Obfuscation(Exclude = false)]
    private void ExcludedMethod2() { }

    private void ExcludedMethod3() { }

    [Obfuscation()]
    private void ExcludedMethod4() { }
}
