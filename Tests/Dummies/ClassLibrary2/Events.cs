#pragma warning disable

using System.Reflection;

namespace ClassLibrary2;

[Obfuscation(Exclude = false, Feature = "Trimming;AttributeCleaning")]
public class Events
{
    public event EventHandler Event1;
    public static event EventHandler Event2;
    internal event EventHandler Event3;
    internal static event EventHandler Event4;
    private event EventHandler Event5;
    private static event EventHandler Event6;

    public void Invoke()
    {
        Event1?.Invoke(this, EventArgs.Empty);
        Event2?.Invoke(this, EventArgs.Empty);
        Event3?.Invoke(this, EventArgs.Empty);
        Event4?.Invoke(this, EventArgs.Empty);
        Event5?.Invoke(this, EventArgs.Empty);
        Event6?.Invoke(this, EventArgs.Empty);
    }
}
