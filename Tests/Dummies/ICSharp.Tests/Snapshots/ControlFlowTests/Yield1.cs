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
            uint num = 100192212u;
            int num2 = 375760892;
            int num3 = default(int);
            int num4 = default(int);
            while (true)
            {
                switch (num = (uint)(num2 + (int)num) % 23u)
                {
                case 0u:
                    <>2__current = list[<i>5__2];
                    num2 = 33234726;
                    break;
                case 3u:
                case 5u:
                    num3 = <>1__state;
                    num2 = 292008071;
                    break;
                case 18u:
                    return true;
                case 12u:
                    if (<i>5__2 >= list.Count)
                    {
                        num2 = 872674006;
                        break;
                    }
                    num = 1503375910u;
                    goto case 0u;
                case 4u:
                    return true;
                case 22u:
                    return false;
                case 11u:
                    <>1__state = -1;
                    num2 = 481525288;
                    break;
                default:
                    <>1__state = 2;
                    num2 = 2079632632;
                    break;
                case 17u:
                    return true;
                case 21u:
                    <i>5__2 = 0;
                    num = 1079264846u;
                    goto case 12u;
                case 13u:
                    <>2__current = 0;
                    num2 = 408010848;
                    break;
                case 15u:
                    <>1__state = 1;
                    num2 = 1416034010;
                    break;
                case 8u:
                    num4 = <i>5__2;
                    num2 = 139756696;
                    break;
                case 14u:
                    <>2__current = int.MaxValue;
                    num2 = 1801453014;
                    break;
                case 16u:
                    <>1__state = 3;
                    num2 = 585965160;
                    break;
                case 19u:
                    <>1__state = -1;
                    num2 = 1463793004;
                    break;
                case 20u:
                    <>1__state = -1;
                    num2 = 407400980;
                    break;
                case 6u:
                    <>1__state = -1;
                    num2 = 1674538784;
                    break;
                case 9u:
                    return false;
                case 10u:
                    <i>5__2 = num4 + 1;
                    num2 = 2124247457;
                    break;
                case 7u:
                    {
                        switch (num3)
                        {
                        case 2:
                            break;
                        case 3:
                            goto IL_00f1;
                        case 0:
                            goto IL_018d;
                        default:
                            goto IL_01e5;
                        case 1:
                            goto IL_01ef;
                        }
                        num = 897954759u;
                        goto case 6u;
                    }
                    IL_01ef:
                    num = 2145820531u;
                    goto case 19u;
                    IL_01e5:
                    num2 = 395232071;
                    break;
                    IL_018d:
                    num = 1272545276u;
                    goto case 11u;
                    IL_00f1:
                    num = 552593696u;
                    goto case 20u;
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
            uint num = 100461309u;
            int num2 = 302859090;
            ClassLibrary2.ControlFlowClass2.<Test1>d__0 <Test1>d__1 = default(ClassLibrary2.ControlFlowClass2.<Test1>d__0);
            while (true)
            {
                switch (num = (uint)(num2 + (int)num) % 7u)
                {
                case 2u:
                    <Test1>d__1.list = <>3__list;
                    num2 = 843551536;
                    break;
                case 6u:
                    if (<>1__state != -2)
                    {
                        num = 221466847u;
                        goto default;
                    }
                    num2 = 1554335589;
                    break;
                case 3u:
                    <>1__state = 0;
                    num2 = 1121830271;
                    break;
                case 1u:
                    if (<>l__initialThreadId != System.Environment.CurrentManagedThreadId)
                    {
                        num = 13271041u;
                        goto default;
                    }
                    num2 = 1234272482;
                    break;
                default:
                    <Test1>d__1 = new ClassLibrary2.ControlFlowClass2.<Test1>d__0(0);
                    num2 = 293950610;
                    break;
                case 5u:
                    <Test1>d__1 = this;
                    num = 1262219016u;
                    goto case 2u;
                case 4u:
                    return <Test1>d__1;
                }
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