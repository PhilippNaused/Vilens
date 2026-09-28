using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using dnlib.Threading;
using Vilens.Data;
using Vilens.Features;
using Vilens.Helpers;
using Vilens.Logging;

namespace Vilens;

public sealed class Scrambler
{
    internal Scrambler(byte[] data, byte[]? pdbData, VilensSettings settings, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        Settings = settings;
        Cancellation = cancellation;
        Log.Info("Selected features: {0}", settings.Features);
        Log.Info("Selected scope: {0}", settings.Scope);
        Log.Info("AOT Safe Mode: {0}", settings.AotSafeMode);
        var sw = Stopwatch.StartNew();

        var modCtx = new ModuleContext(new NullResolver());
        var modCreationOptions = new ModuleCreationOptions(modCtx)
        {
            TryToLoadPdbFromDisk = false,
            PdbFileOrData = pdbData
        };

        Module = ModuleDefMD.Load(data, modCreationOptions);
        Module.LoadEverything(new DnlibCancellationToken(cancellation));
        Log.Debug("Loaded assembly: {0}", Module.Assembly);

        if (!Module.IsILOnly)
        {
            throw new NotSupportedException("Assemblies containing unmanaged code are not supported.");
        }

        Log.Debug("Loaded assembly in {0}", sw.Elapsed);
        sw.Restart();
        Log.Debug("Preparing data");
        if (Scope == Visibility.Auto)
        {
            bool v = Module.Assembly.CustomAttributes.Any(c => c.TypeFullName == typeof(InternalsVisibleToAttribute).FullName);
            Settings.Scope = v ? Visibility.Private : Visibility.Internal;
            Log.Info("Setting Scope to {0}", Scope);
        }
        Database = new Database(Module, Settings.Features, cancellation);
        Log.Debug("Prepared data in {0}", sw.Elapsed);
    }

    private static Logger Log { get; } = new(nameof(Scrambler));

    public ModuleDefMD Module { get; }
    public Visibility Scope => Settings.Scope;
    public VilensSettings Settings { get; }
    public CancellationToken Cancellation { get; }
    internal Database Database { get; }

    public static void Scramble(byte[] data,
        byte[]? pdbData,
        VilensSettings settings,
        out byte[] newData,
        out byte[] newPdbData,
        CancellationToken cancellation)
    {
        try
        {
            var scrambler = new Scrambler(data, pdbData, settings, cancellation);
            scrambler.Execute();
            scrambler.Save(out newData, out newPdbData, settings.DelaySign, settings.StrongNamingKey);
        }
        catch (AggregateException e)
        {
            if (e.InnerExceptions.Any(x => x is not OperationCanceledException))
            {
                throw;
            }
            if (e.InnerExceptions.FirstOrDefault() is not OperationCanceledException op)
            {
                throw;
            }
            throw op;
        }
    }

    internal Xoshiro128 GetRandom()
    {
        // Use the name of the module as the seed to make the randomness deterministic.
        return new Xoshiro128(Module.Name.Data);
    }

    internal void Execute()
    {
        Func<FeatureBase>[] tasks =
        [
            () => new PropertyInline(this),
            // () => new Trimming(this),
            () => new StringHiding(this),
            () => new DisOptimize(this),
            () => new ControlFlow(this),
            () => new Corruption(this),
            () => new AttributeCleaning(this),
            () => new Renaming(this),
        ];

        Log.Debug("Executing features");
        var sw = Stopwatch.StartNew();

        foreach (var func in tasks)
        {
            Cancellation.ThrowIfCancellationRequested();
            FeatureBase feature = func();
            Log.Debug("Starting {0}", feature.Log.Name);
            sw.Restart();
            feature.Execute();
            Log.Debug("Finished {0} in {1}", feature.Log.Name, sw.Elapsed);
        }

        Database.FeatureMap.Cleanup();
    }

    internal void Save(out byte[] data, out byte[] pdbData, bool delaySign, byte[]? strongNamingKey)
    {
        var dataStream = new MemoryStream();
        var pdbStream = new MemoryStream();

        var writerOptions = new ModuleWriterOptions(Module)
        {
            WritePdb = true,
            PdbStream = pdbStream,
        };

        if (strongNamingKey != null)
        {
            Module.Assembly.CustomAttributes.RemoveAll("System.Reflection.AssemblySignatureKeyAttribute");
            writerOptions.DelaySign = delaySign;
            if (delaySign)
            {
                var signatureKey = new StrongNamePublicKey(strongNamingKey);
                writerOptions.StrongNamePublicKey = signatureKey;
            }
            else
            {
                var signatureKey = new StrongNameKey(strongNamingKey);
                writerOptions.StrongNameKey = signatureKey;
            }
        }

        Module.Write(dataStream, writerOptions);
        data = dataStream.ToArray();
        pdbData = pdbStream.ToArray();
    }

    private readonly struct DnlibCancellationToken(CancellationToken token) : ICancellationToken
    {
        public void ThrowIfCancellationRequested() => token.ThrowIfCancellationRequested();
    }
}
