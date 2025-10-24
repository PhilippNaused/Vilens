using System.Runtime.CompilerServices;
using Vilens.Logging;

namespace Vilens.Features;

internal sealed class DisOptimize : FeatureBase
{
    /// <inheritdoc />
    public DisOptimize(Scrambler scrambler) : base(scrambler)
    {
        string asyncStateMachineName = typeof(IAsyncStateMachine).FullName!;
        var stateMachines = Database.Types.Where(t => t.Item.IsNestedPrivate && t.Item.Interfaces.Any(i => i.Interface.FullName == asyncStateMachineName)).ToList();
        foreach (var type in stateMachines)
        {
            Log.Trace("Found {type}", type);
        }
    }

    public override Logger Log { get; } = new Logger(nameof(DisOptimize));

    /// <inheritdoc />
    public override void Execute()
    {
        Cancellation.ThrowIfCancellationRequested();
        //TODO
    }
}
