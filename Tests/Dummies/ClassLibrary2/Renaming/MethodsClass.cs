using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public class MethodsClass
{
    public MethodsClass(string str)
    {
    }

    internal MethodsClass(int str)
    {
    }

    private MethodsClass(bool str)
    {
    }

    public void PublicM(string str)
    {
    }

    internal void InternalM(string str)
    {
    }

    private void PrivateM(string str)
    {
    }

    public virtual void PublicV(string str)
    {
    }

    internal virtual void InternalV(string str)
    {
    }
}
