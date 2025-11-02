private event System.EventHandler? c
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    add
    {
        System.EventHandler eventHandler = f;
        System.EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
            eventHandler = System.Threading.Interlocked.CompareExchange(ref f, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
    [System.Runtime.CompilerServices.CompilerGenerated]
    remove
    {
        System.EventHandler eventHandler = f;
        System.EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
            eventHandler = System.Threading.Interlocked.CompareExchange(ref f, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
}