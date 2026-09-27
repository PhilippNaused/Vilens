public class ControlFlowClass2
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    private sealed class <Test1>d__0 : System.Collections.Generic.IEnumerable<int>, System.Collections.Generic.IEnumerator<int>, System.Collections.IEnumerable, System.Collections.IEnumerator, System.IDisposable
    {
        private int <>1__state;
        private int <>2__current;
        private int <>l__initialThreadId;
        private System.Collections.Generic.IList<int> list;
        public System.Collections.Generic.IList<int> <>3__list;
        private int <i>5__2;
        int System.Collections.Generic.IEnumerator<int>.Current
        {
            [System.Diagnostics.DebuggerHidden]
            get
            {
                return <>2__current;
            }
        }
        object System.Collections.IEnumerator.Current
        {
            [System.Diagnostics.DebuggerHidden]
            get
            {
                return <>2__current;
            }
        }
        [System.Diagnostics.DebuggerHidden]
        public <Test1>d__0(int <>1__state)
        {
            this.<>1__state = <>1__state;
            <>l__initialThreadId = System.Environment.CurrentManagedThreadId;
        }
        [System.Diagnostics.DebuggerHidden]
        void System.IDisposable.Dispose()
        {
            <>1__state = -2;
        }
        private bool MoveNext()
        {
            uint num = 1290929669u;
            int num2 = 339567531;
            int num4 = default;
            int num3 = default;
            while (true)
            {
                switch (num = (uint)(num2 + (int)num) % 23u)
                {
                case 15u:
                    num4 = <i>5__2;
                    num2 = 348100071;
                    break;
                case 18u:
                    return true;
                case 22u:
                    <>1__state = -1;
                    num2 = 2092502066;
                    break;
                case 12u:
                    return false;
                case 9u:
                    <>2__current = 0;
                    num2 = 1995981347;
                    break;
                case 8u:
                    <i>5__2 = num4 + 1;
                    num2 = 21620407;
                    break;
                case 11u:
                    <>1__state = -1;
                    num2 = 2059005165;
                    break;
                case 10u:
                    <>1__state = 3;
                    num2 = 683737599;
                    break;
                case 3u:
                    return true;
                case 7u:
                    switch (num3)
                    {
                    case 2:
                        goto IL_0103;
                    case 0:
                        goto IL_0134;
                    case 1:
                        goto IL_0180;
                    case 3:
                        goto IL_018b;
                    }
                    num2 = 1695325669;
                    break;
                case 19u:
                    <>1__state = -1;
                    num2 = 1825481726;
                    break;
                case 17u:
                    <>2__current = int.MaxValue;
                    num2 = 1228136444;
                    break;
                case 4u:
                    <i>5__2 = 0;
                    num = 1438839981u;
                    goto case 1u;
                case 1u:
                    if (<i>5__2 >= list.Count)
                    {
                        num2 = 748755908;
                        break;
                    }
                    num = 563857440u;
                    goto case 20u;
                case 5u:
                case 6u:
                    return true;
                case 14u:
                    num3 = <>1__state;
                    num2 = 1832319764;
                    break;
                case 16u:
                    <>1__state = -1;
                    num2 = 883426014;
                    break;
                case 13u:
                    <>1__state = 2;
                    num2 = 1151743968;
                    break;
                case 20u:
                    <>2__current = list[<i>5__2];
                    num2 = 1850077650;
                    break;
                case 0u:
                case 21u:
                    return false;
                default:
                    {
                        <>1__state = 1;
                        num2 = 747950195;
                        break;
                    }
                    IL_018b:
                    num = 337821719u;
                    goto case 19u;
                    IL_0180:
                    num = 900905434u;
                    goto case 11u;
                    IL_0134:
                    num = 1913829119u;
                    goto case 16u;
                    IL_0103:
                    num = 2090411257u;
                    goto case 22u;
                }
            }
        }
        bool System.Collections.IEnumerator.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            return this.MoveNext();
        }
        [System.Diagnostics.DebuggerHidden]
        void System.Collections.IEnumerator.Reset()
        {
            throw new System.NotSupportedException();
        }
        [System.Diagnostics.DebuggerHidden]
        System.Collections.Generic.IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator()
        {
            uint num = 1288618440u;
            int num2 = 1257660708;
            ClassLibrary2.ControlFlowClass2.<Test1>d__0 obj = default;
            while (true)
            {
                switch (num = (uint)(num2 + (int)num) % 7u)
                {
                case 4u:
                    return obj;
                default:
                    if (<>l__initialThreadId != System.Environment.CurrentManagedThreadId)
                    {
                        num = 1204498874u;
                        break;
                    }
                    num2 = 595546346;
                    continue;
                case 2u:
                    obj = this;
                    num = 921502226u;
                    goto case 1u;
                case 0u:
                    if (<>1__state == -2)
                    {
                        num2 = 656849014;
                        continue;
                    }
                    num = 284961500u;
                    break;
                case 1u:
                    obj.list = <>3__list;
                    num2 = 1754145494;
                    continue;
                case 6u:
                    <>1__state = 0;
                    num2 = 1813395874;
                    continue;
                case 5u:
                    break;
                }
                obj = new ClassLibrary2.ControlFlowClass2.<Test1>d__0(0);
                num2 = 310942390;
            }
        }
        [System.Diagnostics.DebuggerHidden]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.Generic.IEnumerable<int>)this).GetEnumerator();
        }
    }
    [System.Runtime.CompilerServices.IteratorStateMachine(typeof(ClassLibrary2.ControlFlowClass2.<Test1>d__0))]
    public static System.Collections.Generic.IEnumerable<int> Test1(System.Collections.Generic.IList<int> list)
    {
        //yield-return decompiler failed: Unable to find new state assignment for yield return
        return new ClassLibrary2.ControlFlowClass2.<Test1>d__0(-2)
        {
            <>3__list = list
        };
    }
}