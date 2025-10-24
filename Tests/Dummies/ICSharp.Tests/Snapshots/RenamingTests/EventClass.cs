using System;
using System.Runtime.CompilerServices;
using System.Threading;

public class EventClass
{
    [CompilerGenerated]
    private EventHandler? m_a;
    [CompilerGenerated]
    private EventHandler? m_b;
    [CompilerGenerated]
    private EventHandler? m_c;
    [CompilerGenerated]
    private EventHandler? d;
    [CompilerGenerated]
    private EventHandler? e;
    [CompilerGenerated]
    private EventHandler? f;
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
    internal event EventHandler? a
    {
        [CompilerGenerated]
        add
        {
            EventHandler eventHandler = this.b;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref this.b, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            EventHandler eventHandler = this.b;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref this.b, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    protected event EventHandler? Ev3
    {
        [CompilerGenerated]
        add
        {
            EventHandler eventHandler = this.c;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref this.c, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            EventHandler eventHandler = this.c;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref this.c, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    protected internal event EventHandler? Ev4
    {
        [CompilerGenerated]
        add
        {
            EventHandler eventHandler = d;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref d, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            EventHandler eventHandler = d;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref d, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    private protected event EventHandler? b
    {
        [CompilerGenerated]
        add
        {
            EventHandler eventHandler = e;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref e, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            EventHandler eventHandler = e;
            EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
                eventHandler = Interlocked.CompareExchange(ref e, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
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
}