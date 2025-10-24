using System.Reflection;

namespace ClassLibrary2.Renaming;

[Obfuscation(Exclude = false, Feature = "Renaming")]
public class EventClass
{
    public event EventHandler? Ev1;
    internal event EventHandler? Ev2;
    protected event EventHandler? Ev3;
    protected internal event EventHandler? Ev4;
    private protected event EventHandler? Ev5;
    private event EventHandler? Ev6;
}
