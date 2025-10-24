using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Extension;

public static class ICSharpCodeExtensions
{
    private const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private static readonly PropertyInfo _tokenProp = typeof(EntityHandle).GetProperty("Token", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly DecompilerSettings _compilerSettings;

#pragma warning disable CA1810 // Initialize reference type static fields inline
    static ICSharpCodeExtensions()
#pragma warning restore CA1810 // Initialize reference type static fields inline
    {
        Debug.Assert(_tokenProp != null);
        Debug.Assert(_tokenProp!.PropertyType == typeof(int));

        _compilerSettings = new DecompilerSettings(LanguageVersion.Latest)
        {
            AlwaysShowEnumMemberValues = true, // changes to enum values are breaking changes => always show them
            ShowXmlDocumentation = false, // the docs are not part of the API
            AutoLoadAssemblyReferences = false, // we don't need the references
            UseDebugSymbols = false, // we don't need the debug symbols
            ShowDebugInfo = false, // we don't need the debug info
            LoadInMemory = true, // faster than loading from disk
            FileScopedNamespaces = true
        };
        var format = _compilerSettings.CSharpFormattingOptions;
        format.IndentationString = "    "; // 4 spaces is the de facto standard for C#
        // use as few lines as possible:
        format.AutoPropertyFormatting = PropertyFormatting.SingleLine;
        format.MinimumBlankLinesBetweenMembers = 0;
        format.MinimumBlankLinesBetweenTypes = 0;
    }

    private static CSharpDecompiler GetDecompiler(this Assembly assembly)
    {
        return GetDecompiler(assembly.Location);
    }

    public static CSharpDecompiler GetDecompiler(byte[] assembly, string fileName)
    {
        using var ms = new MemoryStream(assembly);
#pragma warning disable CA2000 // Dispose objects before losing scope
        var peFile = new PEFile(fileName, ms, PEStreamOptions.PrefetchEntireImage);
#pragma warning restore CA2000 // Dispose objects before losing scope
        var resolver = new UniversalAssemblyResolver(fileName, false,
            peFile.DetectTargetFrameworkId(), peFile.DetectRuntimePack());
        return new CSharpDecompiler(peFile, resolver, _compilerSettings);
    }

    private static CSharpDecompiler GetDecompiler(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        return GetDecompiler(bytes, path);
    }

    public static string GetCode(this CSharpDecompiler decompiler)
    {
        var ts = decompiler.TypeSystem;
        var module = ts.MainModule;
        var sb = new StringBuilder().AppendLine($"// {module.FullAssemblyName}");

        foreach (var reference in module.MetadataFile!.AssemblyReferences)
        {
            sb = sb.AppendLine($"// Reference: {reference.FullName}");
        }

        foreach (var reference in module.MetadataFile.ModuleReferences)
        {
            sb = sb.AppendLine($"// Reference: {reference.Name}");
        }

        return sb.AppendLine(decompiler.DecompileWholeModuleAsString())
            .Replace("\r\n", "\n") // normalize line endings
            .Replace("\n\n", "\n") // remove empty lines
            .ToString();
    }

    public static string Disassemble(this Type type)
    {
        var decompiler = type.Assembly.GetDecompiler();
        var typeInfo = decompiler.TypeSystem.FindType(type).GetDefinition();
        var file = typeInfo!.ParentModule!.MetadataFile;
        var handle = (TypeDefinitionHandle)typeInfo.MetadataToken;

        var text = new PlainTextOutput();
        var dis = new ReflectionDisassembler(text, CancellationToken.None);
        dis.DisassembleType(file, handle);
        return text.ToString().Trim().NormalizeLineTerminators();
    }

    public static string Disassemble(this CSharpDecompiler decompiler)
    {
        var text = new PlainTextOutput();
        var dis = new ReflectionDisassembler(text, CancellationToken.None);
        dis.WriteModuleContents(decompiler.TypeSystem.MainModule.MetadataFile);
        return text.ToString().Trim().NormalizeLineTerminators();
    }

    public static string Decompile(this Type type)
    {
        var decompiler = type.Assembly.GetDecompiler();
        var handle = decompiler.TypeSystem.FindType(type).GetDefinition()!.MetadataToken;
        return decompiler.DecompileAsString(handle).Trim().NormalizeLineTerminators();
    }

    private static string NormalizeLineTerminators(this string text)
    {
        return text.Replace("\r\n", "\n");
    }

    public static MemberInfo[] GetAllMembers(this Type type) => type.GetMembers(BindingAttr);

    public static PropertyInfo[] GetAllProperties(this Type type) => type.GetProperties(BindingAttr);

    public static MethodInfo[] GetAllMethods(this Type type) => type.GetMethods(BindingAttr);

    public static EventInfo[] GetAllEvents(this Type type) => type.GetEvents(BindingAttr);

    public static string Decompile(this MemberInfo info)
    {
        var type = info.DeclaringType;
        Debug.Assert(type != null);
        var decompiler = type!.Assembly.GetDecompiler();
        var typeDef = decompiler.TypeSystem.FindType(type).GetDefinition();
        Debug.Assert(typeDef != null);
        var member = typeDef!.GetMembers(m => m.MetadataToken.Convert() == info.MetadataToken, GetMemberOptions.IgnoreInheritedMembers).Single();
        return decompiler.DecompileAsString(member.MetadataToken).Trim().NormalizeLineTerminators();
    }

    private static int Convert(this EntityHandle handle)
    {
        return (int)_tokenProp.GetValue(handle)!;
    }
}
