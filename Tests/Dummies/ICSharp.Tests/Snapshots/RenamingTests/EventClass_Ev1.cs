using System;
using System.Runtime.CompilerServices;
using System.Threading;

public event EventHandler? Ev1
{
    [CompilerGenerated]
    add
    {
        EventHandler eventHandler = this.a;
        EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
            eventHandler = Interlocked.CompareExchange(ref this.a, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
    [CompilerGenerated]
    remove
    {
        EventHandler eventHandler = this.a;
        EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
            eventHandler = Interlocked.CompareExchange(ref this.a, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
}