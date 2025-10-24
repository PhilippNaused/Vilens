using System;
using System.Runtime.CompilerServices;
using System.Threading;

private event EventHandler? c
{
    [CompilerGenerated]
    add
    {
        EventHandler eventHandler = f;
        EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
            eventHandler = Interlocked.CompareExchange(ref f, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
    [CompilerGenerated]
    remove
    {
        EventHandler eventHandler = f;
        EventHandler eventHandler2;
        do
        {
            eventHandler2 = eventHandler;
            EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
            eventHandler = Interlocked.CompareExchange(ref f, value2, eventHandler2);
        }
        while ((object)eventHandler != eventHandler2);
    }
}