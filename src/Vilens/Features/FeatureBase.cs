using dnlib.DotNet;
using Vilens.Data;
using Vilens.Logging;

namespace Vilens.Features;

internal abstract class FeatureBase(Scrambler scrambler)
{
    public abstract Logger Log { get; }

    public ModuleDefMD Module => Scrambler.Module;

    public Database Database => Scrambler.Database;

    public CancellationToken Cancellation => Scrambler.Cancellation;

    public Scrambler Scrambler { get; } = scrambler;

    public abstract void Execute();

    protected bool IsInScope(IMemberDef member)
    {
        return member.IsInScope(Scrambler.Scope);
    }
}
