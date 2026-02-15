using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using NUnit.Framework;
using Vilens.Helpers;
using MethodAttributes = dnlib.DotNet.MethodAttributes;

namespace Vilens.Tests;

internal class RegressionTest
{
    [Test]
    public void InvalidProgramExceptionMono()
    {
        // For some reason, Mono's JIT compiler has a strange bug where calculates the stack height incorrectly when an instruction after an unconditional branch has a stack height other than 0.
        // This issue doesn't occur on the official .NET runtimes (both Core and Framework).

        var assembly = new AssemblyDefUser("TestAssembly", new Version(1, 0, 0, 0));
        var module = new ModuleDefUser("TestAssembly", Guid.NewGuid(), new AssemblyRefUser("netstandard", new Version(2, 0, 0, 0), new PublicKeyToken("cc7b13ffcd2ddd51")));
        assembly.Modules.Add(module);
        module.RuntimeVersion = MDHeaderRuntimeVersion.MS_CLR_40;
        var type = new TypeDefUser("TestType", module.CorLibTypes.Object.ToTypeDefOrRef());
        module.Types.Add(type);
        var method = new MethodDefUser("TestMethod", MethodSig.CreateStatic(module.CorLibTypes.String), MethodAttributes.Public | MethodAttributes.Static);
        type.Methods.Add(method);

        method.Body = new CilBody();

        /*
         0 br 2 // stack height = 0
         1 ret // stack height = 1 -> 0
         2 ldstr "Hello World!" // stack height = 0 -> 1
         3 br 1 // stack height = 1
         */

        var ret = Emit.Return();
        var ldstr = Emit.Load("Hello World!");

        method.Body.Instructions.Add(Emit.Goto(ldstr));
        method.Body.Instructions.Add(ret);
        method.Body.Instructions.Add(ldstr);
        method.Body.Instructions.Add(Emit.Goto(ret));

        method.Body.MaxStack = 1;
        method.Body.Instructions.Optimize();

        var stackHeights = StackHelper.GetStackHeights(method.Body);

        ushort?[] expected = [0, 1, 0, 1];
        Assert.That(stackHeights, Is.EqualTo(expected));

        var success = dnlib.DotNet.Writer.MaxStackCalculator.GetMaxStack(method.Body.Instructions, method.Body.ExceptionHandlers, out _);
        Assert.That(success, Is.False); // dnlib's MaxStackCalculator2 should fail to calculate the max stack for this method, because is has a similar bug to Mono's JIT compiler.

        method.Body.KeepOldMaxStack = !success;

        var ms = new MemoryStream();
        module.Write(ms);

        var assembly2 = Assembly.Load(ms.ToArray());
        var testType = assembly2.GetType("TestType")!;
        var testMethod = testType.GetMethod("TestMethod")!;

        bool isMono = Type.GetType("Mono.Runtime") != null;

        if (isMono)
        {
            var ex = Assert.Throws<TargetInvocationException>(delegate
            {
                _ = testMethod.Invoke(null, []);
            });
            Assert.That(ex.InnerException, Is.TypeOf<InvalidProgramException>());
            Assert.That(ex.InnerException, Has.Message.StartsWith("Invalid IL code in TestType:TestMethod (): IL_0002: ret"));
        }
        else
        {
            var result = testMethod.Invoke(null, []);
            Assert.That(result, Is.EqualTo("Hello World!"));
        }
    }
}
