# Vilens

[![NuGet Version](https://img.shields.io/nuget/vpre/Vilens.MSBuild)](https://www.nuget.org/packages/Vilens.MSBuild)  
[![License](https://img.shields.io/github/license/PhilippNaused/Vilens)](/LICENSE)

Vilens is an experimental assembly obfuscator for .NET.

It integrates directly in the build process via the [`Vilens.MSBuild`](https://www.nuget.org/packages/Vilens.MSBuild) NuGet package.

## Usage

Add a package reference to [`Vilens.MSBuild`](https://www.nuget.org/packages/Vilens.MSBuild).  
You can either enable features globally by adding the `<VilensFeatures>FeatureName</VilensFeatures>` property to you projects or use the `[Obfuscation(Exclude = false, Feature = "FeatureName")]` attribute.

e.g.:

```cs
// This will rename the enum and all it's members.
[Obfuscation(Exclude = false, Feature = "Renaming")]
internal enum Enum1
{
    Value1, Value2, Value3
}

// This will rename the enum itself, but not it's members.
[Obfuscation(Exclude = true, Feature = "Renaming", ApplyToMembers = true)]
[Obfuscation(Exclude = false, Feature = "Renaming", ApplyToMembers = false)]
internal enum Enum2
{
    Value1, Value2, Value3
}
```

## Features

### Renaming

Changes the names of symbols to something meaningless.  
This will automatically exclude symbols that are part of the public API or are called via reflection.

<details>
<summary>Example</summary>

Before:

```cs
namespace MyNameSpace;

internal class User
{
    public string Name;
    private byte[] PasswordHash;
    public User Create(string name, string password) { ... }
}
```

After:

```cs
// global namespace

internal class a
{
    public string a;
    private byte[] b;
    public a a(string _, string _) { ... }
}
```

</details>

### AttributeCleaning

Removed custom attributes that are known to not affect the runtime.
This will:
- Remove Tuple names (e.g. `(int Count, string Name)` becomes `(int, int)`)
- Turn extension methods into regular ones. (i.e. removes the `this` keyword from the first parameter)
- Remove Debug hints like `[DebuggerDisplay]` or `[DebuggerTypeProxy]`
- Remove annotations about state machines. (This can sometimes prevent the decompiler from correctly decompiling methods that use the `async` or `yield` keywords.)
- Remove nullable annotations. (i.e. remove all info added by the nullable reference type feature)
- And much more.

<details>
<summary>Example</summary>

Before:

```cs
[DebuggerStepThrough]
public static (int Count, int Sum) TestMethod(this int[]? numbers) { ... }
```

After:

```cs
internal static (int, int) TestMethod(int[] numbers)
```

</details>

### Corruption

Replaces the types of local variables that are functionally the same, but confuse the decompiler.
e.g. replaces `int` with an empty enum with base type `int`. And replaces all reference types (i.e. `class`) with `object`.

It also adds nested types with invalid code that cause most decompilers to crash when trying to open it.
Since the invalid code is dead code, it doesn't affect the runtime.

<details>
<summary>Example</summary>

Before:

```cs
private static Visibility Max(params ReadOnlySpan<IList<MethodDef>> methodLists)
{
    Visibility visibility = Visibility.Private;
    ReadOnlySpan<IList<MethodDef>> readOnlySpan = methodLists;
    for (int i = 0; i < readOnlySpan.Length; i++)
    {
        foreach (MethodDef item in readOnlySpan[i])
        {
            Visibility vis = item.GetVisibility();
            if (vis > visibility)
            {
                visibility = vis;
                if (visibility == Visibility.Public)
                {
                    return visibility;
                }
            }
        }
    }
    return visibility;
}
```

After:

```
Error decompiling @06000227 Vilens.Data.VisibilityExtensions.Max
 ---> System.InvalidCastException: Cast from String to Int64 not supported.
   at ICSharpCode.Decompiler.Util.CSharpPrimitiveCast.CSharpPrimitiveCastUnchecked(TypeCode targetType, Object input) in CSharpPrimitiveCast.cs:line 767
   at ICSharpCode.Decompiler.CSharp.Syntax.TypeSystemAstBuilder.<ConvertEnumValue>g__PrepareConstant|146_1(IField field) in TypeSystemAstBuilder.cs:line 1375
   at System.Linq.Enumerable.IEnumerableWhereIterator`1.ToArray() in offset 71
   at ICSharpCode.Decompiler.CSharp.Syntax.TypeSystemAstBuilder.ConvertEnumValue(IType type, Int64 val) in TypeSystemAstBuilder.cs:line 1290
   at ICSharpCode.Decompiler.CSharp.Syntax.TypeSystemAstBuilder.ConvertConstantValue(IType expectedType, IType type, Object constantValue) in TypeSystemAstBuilder.cs:line 1119
   at ICSharpCode.Decompiler.CSharp.Syntax.TypeSystemAstBuilder.ConvertConstantValue(ResolveResult rr) in TypeSystemAstBuilder.cs:line 991
   at ICSharpCode.Decompiler.CSharp.ExpressionBuilder.VisitLdcI4(LdcI4 inst, TranslationContext context) in ExpressionBuilder.cs:line 551
   at ICSharpCode.Decompiler.CSharp.ExpressionBuilder.VisitStLoc(StLoc inst, TranslationContext context) in ExpressionBuilder.cs:line 758
   at ICSharpCode.Decompiler.CSharp.StatementBuilder.VisitStLoc(StLoc inst) in StatementBuilder.cs:line 115
   at ICSharpCode.Decompiler.CSharp.StatementBuilder.ConvertBlockContainer(BlockStatement blockStatement, BlockContainer container, IEnumerable`1 blocks, Boolean isLoop) in StatementBuilder.cs:line 1442
   at ICSharpCode.Decompiler.CSharp.StatementBuilder.ConvertBlockContainer(BlockContainer container, Boolean isLoop) in StatementBuilder.cs:line 1342
   at ICSharpCode.Decompiler.CSharp.StatementBuilder.VisitBlockContainer(BlockContainer container) in StatementBuilder.cs:line 1228
   at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in CSharpDecompiler.cs:line 1811
-- continuing with outer exception (ICSharpCode.Decompiler.DecompilerException) --
   at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in CSharpDecompiler.cs:line 1826
   at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.DoDecompile(IMethod method, DecompileRun decompileRun, ITypeResolveContext decompilationContext, ExtensionInfo extensionInfo) in CSharpDecompiler.cs:line 1666
   at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.Decompile(IEnumerable`1 definitions) in CSharpDecompiler.cs:line 1041
   at ICSharpCode.Decompiler.CSharp.CSharpDecompiler.Decompile(EntityHandle[] definitions) in CSharpDecompiler.cs:line 1000
   at ICSharpCode.ILSpy.CSharpLanguage.DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options) in offset 220
   at ICSharpCode.ILSpy.TextView.DecompilerTextView.<>c__DisplayClass58_0.<DecompileAsync>b__0() in offset 35
```

After (if invalid code is removed):

```cs
private static Visibility Max(params ReadOnlySpan<IList<MethodDef>> methodLists)
{
    Visibility visibility = Visibility.Private;
    ReadOnlySpan<IList<MethodDef>> readOnlySpan = methodLists;
    for (c c2 = (c)0u; (int)c2 < readOnlySpan.Length; c2++)
    {
        object enumerator = readOnlySpan[(int)c2].GetEnumerator();
        try
        {
            while (((IEnumerator)enumerator).MoveNext())
            {
                Visibility vis = ((IEnumerator<MethodDef>)enumerator).Current.GetVisibility();
                if (vis > visibility)
                {
                    visibility = vis;
                    if (visibility == Visibility.Public)
                    {
                        return visibility;
                    }
                }
            }
        }
        finally
        {
            ((IDisposable)enumerator)?.Dispose();
        }
    }
    return visibility;
}
```

</details>

### ControlFlow

Puts parts of the method body into a random order and connects them with goto and switch instructions.
The instructions are still executed in the same order as before, but the decompiler can't determine the correct control flow.

<details>
<summary>Example</summary>

Before:

```cs
private static void Compress(List<List<Instruction>> blocks)
{
    for (int i = blocks.Count - 1; i > 1; i--)
    {
        if (IsSmallBlock(blocks[i]))
        {
            blocks[i - 1].AddRange(blocks[i]);
            blocks[i].Clear();
        }
    }
    if (blocks.Count > 1 && IsSmallBlock(blocks[0]))
    {
        blocks[1].InsertRange(0, blocks[0]);
        blocks[0].Clear();
    }
}
```

After:

```cs
private static void Compress(List<List<Instruction>> blocks)
{
    uint num = 100411883u;
    int num2 = 147097722;
    int i = default(int);
    while (true)
    {
        switch (num = (uint)(num2 + (int)num) % 11u)
        {
        case 3u:
            if (i <= 1)
            {
                num2 = 432092322;
                continue;
            }
            num = 305233924u;
            goto case 6u;
        case 6u:
            if (IsSmallBlock(blocks[i]))
            {
                num2 = 1329818811;
                continue;
            }
            num = 48868414u;
            break;
        case 2u:
            i = blocks.Count - 1;
            num2 = 722711189;
            continue;
        case 4u:
            num = 384967806u;
            goto case 3u;
        case 9u:
            blocks[1].InsertRange(0, blocks[0]);
            num2 = 1162092670;
            continue;
        case 5u:
            if (blocks.Count <= 1)
            {
                num = 1810152399u;
                return;
            }
            num2 = 78239372;
            continue;
        case 8u:
            blocks[i - 1].AddRange(blocks[i]);
            num2 = 647387006;
            continue;
        case 0u:
            blocks[0].Clear();
            return;
        case 7u:
            if (!IsSmallBlock(blocks[0]))
            {
                num = 2045213049u;
                return;
            }
            num2 = 33018361;
            continue;
        case 10u:
            blocks[i].Clear();
            num2 = 1303312848;
            continue;
        }
        i--;
        num2 = 810483522;
    }
}
```

</details>

### StringHiding

Removes all the string constants from method bodies and places them in a binary blob that gets decoded at runtime.

<details>
<summary>Example</summary>

```cs
internal Scrambler(byte[] data, byte[]? pdbData, VilensSettings settings, CancellationToken cancellation)
{
    cancellation.ThrowIfCancellationRequested();
    Settings = settings;
    Cancellation = cancellation;
    Log.Info("Selected features: {0}", settings.Features);
    Log.Info("Selected scope: {0}", settings.Scope);
    Log.Info("AOT Safe Mode: {0}", settings.AotSafeMode);
    Stopwatch stopwatch = Stopwatch.StartNew();
    ModuleCreationOptions modCreationOptions = new ModuleCreationOptions(new ModuleContext(new NullResolver()))
    {
        TryToLoadPdbFromDisk = false,
        PdbFileOrData = pdbData
    };
    Module = ModuleDefMD.Load(data, modCreationOptions);
    Module.LoadEverything(new DnlibCancellationToken(cancellation));
    if (!Module.IsILOnly)
    {
        throw new NotSupportedException("Assemblies containing unmanaged code are not supported.");
    }
    stopwatch.Restart();
    if (Scope == Visibility.Auto)
    {
        bool v = Module.Assembly.CustomAttributes.Any((CustomAttribute c) => c.TypeFullName == typeof(InternalsVisibleToAttribute).FullName);
        Settings.Scope = ((!v) ? Visibility.Internal : Visibility.Private);
        Log.Info("Setting Scope to {0}", Scope);
    }
    Database = new Database(Module, Settings.Features, cancellation);
}
```

After:

```cs
internal Scrambler(byte[] data, byte[]? pdbData, VilensSettings settings, CancellationToken cancellation)
{
    cancellation.ThrowIfCancellationRequested();
    Settings = settings;
    Cancellation = cancellation;
    Log.Info(a.b[8], settings.Features);
    Log.Info(a.b[2], settings.Scope);
    Log.Info(a.b[13], settings.AotSafeMode);
    Stopwatch stopwatch = Stopwatch.StartNew();
    ModuleCreationOptions modCreationOptions = new ModuleCreationOptions(new ModuleContext(new NullResolver()))
    {
        TryToLoadPdbFromDisk = false,
        PdbFileOrData = pdbData
    };
    Module = ModuleDefMD.Load(data, modCreationOptions);
    Module.LoadEverything(new DnlibCancellationToken(cancellation));
    if (!Module.IsILOnly)
    {
        throw new NotSupportedException(a.b[9]);
    }
    stopwatch.Restart();
    if (Scope == c.Auto)
    {
        bool v = Module.Assembly.CustomAttributes.Any((CustomAttribute customAttribute) => customAttribute.TypeFullName == typeof(InternalsVisibleToAttribute).FullName);
        Settings.Scope = ((!v) ? c.Internal : c.Private);
        Log.Info(a.b[1], Scope);
    }
    Database = new Database(Module, Settings.Features, cancellation);
}
```

</details>
