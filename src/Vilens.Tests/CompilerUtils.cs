using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Vilens.Tests;

internal class CompilerUtils
{
    private static readonly List<MetadataReference> s_References = GetReferences();

    private static CSharpCompilation GetCompilation(List<string> sourceCode, CSharpCompilationOptions? options = null, CSharpParseOptions? options2 = null)
    {
        var syntaxTrees = sourceCode.Select(code => CSharpSyntaxTree.ParseText(code, options2, cancellationToken: TestContext.CurrentContext.CancellationToken));
        //TestContext.WriteLine($"Assemblies:\n{string.Join(",\n", s_References.Select(r => r.Display).OrderBy(n => n))}");

        var compilation = CSharpCompilation.Create("Test",
            syntaxTrees,
            s_References,
            options ?? new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

        return compilation;
    }

    private static List<MetadataReference> GetReferences()
    {
        Type[] types =
        [
            typeof(object)
        ];

        var assemblies = types
            .Select(t => t.Assembly)
#if NETCOREAPP
            .Append(Assembly.Load("System.Collections, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"))
            .Append(Assembly.Load("System.Runtime, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"))
#else
            // .Append(Assembly.Load("netstandard, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"))
            // .Append(Assembly.Load("System.ValueTuple, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"))
#endif
            .Select(a => a.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path)
            .Select(MetadataReference (path) => MetadataReference.CreateFromFile(path))
            .ToList();
        return assemblies;
    }

    public static byte[] Compile(string sourceCode, out byte[] pdbData, out string frameworkName)
    {
        var attr = typeof(CompilerUtils).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()!;
        var code2 = $"""
            [assembly: System.Runtime.Versioning.TargetFramework("{attr.FrameworkName}", FrameworkDisplayName = "{attr.FrameworkDisplayName}")]
            """;
        frameworkName = attr.FrameworkDisplayName!;
        var compilation = GetCompilation([sourceCode, code2]);
        var assemblyStream = new MemoryStream();
        var pdbStream = new MemoryStream();
        var result = compilation.Emit(assemblyStream, pdbStream, cancellationToken: TestContext.CurrentContext.CancellationToken);
        var assembly = result.Success ? assemblyStream.ToArray() : null;
        if (!result.Success)
        {
            Assert.Fail(string.Join("\n", result.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Error)));
        }
        Assert.That(assembly, Is.Not.Null);
        pdbData = pdbStream.ToArray();
        return assembly;
    }
}
