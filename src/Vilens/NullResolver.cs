using dnlib.DotNet;

namespace Vilens;

internal sealed class NullResolver : IAssemblyResolver
{
    /// <inheritdoc />
    public AssemblyDef? Resolve(IAssembly assembly, ModuleDef sourceModule)
    {
        return null;
    }
}
