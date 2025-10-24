using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Vilens.Helpers;
using Vilens.Lab;

var mod = ModuleDefMD.Load(typeof(Class1).Module);
var file = new FileInfo(typeof(Class1).Assembly.Location);

var asmResolver = new AssemblyResolver();
var modCtx = new ModuleContext(asmResolver);

asmResolver.DefaultModuleContext = modCtx;

asmResolver.PostSearchPaths.Insert(0, file.Directory!.FullName);
foreach (var dependency in mod.GetAssemblyRefs())
{
    _ = asmResolver.Resolve(dependency, mod);
}
mod.Context = modCtx;

var class1 = mod.FindNormalThrow(typeof(Class1).FullName);
var class2 = mod.FindNormalThrow(typeof(Class2).FullName);
var class3 = mod.FindNormalThrow(typeof(Class3).FullName);
var enumType = mod.CorLibTypes.GetTypeRef(nameof(System), nameof(Enum));

class1.BaseType = enumType;
class1.Attributes &= ~TypeAttributes.BeforeFieldInit;
class1.Attributes |= TypeAttributes.Sealed;
class1.Methods.Remove(class1.FindDefaultConstructor());

var field = new FieldDefUser("value__", new FieldSig(mod.CorLibTypes.Int32), FieldAttributes.Public | FieldAttributes.RTSpecialName | FieldAttributes.SpecialName);
class1.Fields.Add(field);
var field2 = new FieldDefUser("Test123", new FieldSig(mod.CorLibTypes.Int32), FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal)
{
    Constant = new ConstantUser("What?")
};
class1.Fields.Add(field2);

var method = new MethodDefUser("meth", new MethodSig() { RetType = mod.CorLibTypes.Void }, MethodAttributes.Private | MethodAttributes.HideBySig)
{
    Body = new CilBody() { KeepOldMaxStack = true }
};
method.MethodSig.CallingConvention = CallingConvention.HasThis;
method.Body.Instructions.Add(OpCodes.Ldarg_3.ToInstruction());
var i0 = method.Body.Instructions.Append(Emit.Load(7));
var i1 = method.Body.Instructions.Append(Emit.GotoIfTrue(i0));
var i2 = method.Body.Instructions.Append(Emit.NoOp());
method.Body.Instructions.Add(Emit.Switch(new[] { i0, i1, i2 }));
method.Body.Instructions.Add(Emit.NewObject(method));
method.Body.Instructions.Add(Emit.Add());
method.Body.Instructions.Add(Emit.Return());
method.Body.Instructions.Optimize();
class3.Methods.Add(method);

Class5.Update(mod);

Class4b.Update(mod);
Class6b.Update(mod);

var folder = Path.Join(file.Directory!.FullName, "mod");
Directory.CreateDirectory(folder);
var path = Path.Join(folder, file.Name);
mod.Write(path);
Console.WriteLine(path);
