public class ControlFlowClass3
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    private struct <Test1>d__0 : System.Runtime.CompilerServices.IAsyncStateMachine
    {
        public int <>1__state;
        public System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> <>t__builder;
        public System.Collections.Generic.IList<int> list;
        private System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter <>u__1;
        private System.Runtime.CompilerServices.TaskAwaiter <>u__2;
        private void MoveNext()
        {
            int num = <>1__state;
            int count;
            try
            {
                System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter awaiter3;
                System.Runtime.CompilerServices.TaskAwaiter awaiter2;
                System.Runtime.CompilerServices.TaskAwaiter awaiter;
                switch (num)
                {
                default:
                    awaiter3 = System.Threading.Tasks.Task.Yield().GetAwaiter();
                    if (!awaiter3.IsCompleted)
                    {
                        num = (<>1__state = 0);
                        <>u__1 = awaiter3;
                        <>t__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
                        return;
                    }
                    goto IL_0070;
                case 0:
                    awaiter3 = <>u__1;
                    <>u__1 = default(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter);
                    num = (<>1__state = -1);
                    goto IL_0070;
                case 1:
                    awaiter2 = <>u__2;
                    <>u__2 = default(System.Runtime.CompilerServices.TaskAwaiter);
                    num = (<>1__state = -1);
                    goto IL_00da;
                case 2:
                    {
                        awaiter = <>u__2;
                        <>u__2 = default(System.Runtime.CompilerServices.TaskAwaiter);
                        num = (<>1__state = -1);
                        break;
                    }
                    IL_00da:
                    awaiter2.GetResult();
                    list.Add(1);
                    awaiter = System.Threading.Tasks.Task.Delay(2).GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                        num = (<>1__state = 2);
                        <>u__2 = awaiter;
                        <>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                        return;
                    }
                    break;
                    IL_0070:
                    awaiter3.GetResult();
                    list.Add(0);
                    awaiter2 = System.Threading.Tasks.Task.Delay(1).GetAwaiter();
                    if (!awaiter2.IsCompleted)
                    {
                        num = (<>1__state = 1);
                        <>u__2 = awaiter2;
                        <>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
                        return;
                    }
                    goto IL_00da;
                }
                awaiter.GetResult();
                list.Add(2);
                count = list.Count;
            }
            catch (System.Exception exception)
            {
                <>1__state = -2;
                <>t__builder.SetException(exception);
                return;
            }
            <>1__state = -2;
            <>t__builder.SetResult(count);
        }
        void System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            this.MoveNext();
        }
        [System.Diagnostics.DebuggerHidden]
        private void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
        {
            <>t__builder.SetStateMachine(stateMachine);
        }
        void System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
        {
            //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
            this.SetStateMachine(stateMachine);
        }
    }
    [System.Runtime.CompilerServices.AsyncStateMachine(typeof(ClassLibrary2.ControlFlowClass3.<Test1>d__0))]
    public static System.Threading.Tasks.Task<int> Test1(System.Collections.Generic.IList<int> list)
    {
        uint num = 100362457u;
        int num2 = 378532787;
        ClassLibrary2.ControlFlowClass3.<Test1>d__0 stateMachine = default(ClassLibrary2.ControlFlowClass3.<Test1>d__0);
        while (true)
        {
            switch (num = (uint)(num2 + (int)num) % 5u)
            {
            case 2u:
                return stateMachine.<>t__builder.Task;
            case 3u:
                stateMachine.<>1__state = -1;
                num2 = 1556752077;
                break;
            case 4u:
                stateMachine.<>t__builder = System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.Create();
                num2 = 945183647;
                break;
            case 1u:
                stateMachine.list = list;
                num2 = 1264343497;
                break;
            default:
                stateMachine.<>t__builder.Start(ref stateMachine);
                num2 = 219709442;
                break;
            }
        }
    }
}