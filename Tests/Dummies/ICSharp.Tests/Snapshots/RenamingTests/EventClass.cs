public class EventClass
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? m_a;
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? m_b;
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? m_c;
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? d;
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? e;
    [System.Runtime.CompilerServices.CompilerGenerated]
    private System.EventHandler? f;
    public event System.EventHandler? Ev1
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        add
        {
            System.EventHandler eventHandler = this.a;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.a, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [System.Runtime.CompilerServices.CompilerGenerated]
        remove
        {
            System.EventHandler eventHandler = this.a;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.a, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    internal event System.EventHandler? a
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        add
        {
            System.EventHandler eventHandler = this.b;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.b, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [System.Runtime.CompilerServices.CompilerGenerated]
        remove
        {
            System.EventHandler eventHandler = this.b;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.b, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    protected event System.EventHandler? Ev3
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        add
        {
            System.EventHandler eventHandler = this.c;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.c, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [System.Runtime.CompilerServices.CompilerGenerated]
        remove
        {
            System.EventHandler eventHandler = this.c;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref this.c, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    protected internal event System.EventHandler? Ev4
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        add
        {
            System.EventHandler eventHandler = d;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref d, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [System.Runtime.CompilerServices.CompilerGenerated]
        remove
        {
            System.EventHandler eventHandler = d;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref d, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
    private protected event System.EventHandler? b
    {
        [System.Runtime.CompilerServices.CompilerGenerated]
        add
        {
            System.EventHandler eventHandler = e;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Combine(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref e, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
        [System.Runtime.CompilerServices.CompilerGenerated]
        remove
        {
            System.EventHandler eventHandler = e;
            System.EventHandler eventHandler2;
            do
            {
                eventHandler2 = eventHandler;
                System.EventHandler value2 = (System.EventHandler)System.Delegate.Remove(eventHandler2, value);
                eventHandler = System.Threading.Interlocked.CompareExchange(ref e, value2, eventHandler2);
            }
            while ((object)eventHandler != eventHandler2);
        }
    }
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
}