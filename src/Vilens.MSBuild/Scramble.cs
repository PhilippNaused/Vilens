using System.Diagnostics;
using Microsoft.Build.Framework;
using Vilens.Data;
using Vilens.Logging;

namespace Vilens.MSBuild;

public sealed class Scramble : Microsoft.Build.Utilities.Task, ICancelableTask, IDisposable
{
    private readonly CancellationTokenSource cts = new();

    public void Cancel()
    {
        cts.Cancel();
    }

    public void Dispose()
    {
        cts.Dispose();
    }

    [Required]
    public required string Features { get; set; }

    [Required]
    public required string Scope { get; set; }

    [Required]
    public required ITaskItem Assembly { get; set; }

    [Required]
    public required ITaskItem LogFile { get; set; }

    public ITaskItem? PdbFile { get; set; }

    public ITaskItem? StrongNamingKey { get; set; }

    public bool DelaySign { get; set; }

    public override bool Execute()
    {
        if (cts.IsCancellationRequested)
        {
            return false;
        }

        try
        {
            ExecuteInner();
        }
        catch (OperationCanceledException e)
        {
            Log.LogWarningFromException(e, false);
            return false;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            Log.LogErrorFromException(e, true, true, Assembly.ItemSpec);
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return !Log.HasLoggedErrors;
    }

    private void ExecuteInner()
    {
        var sw = Stopwatch.StartNew();

        Log.LogMessage("Logging to {0}", LogFile.ItemSpec);
        Logger.SetTargetFile(LogFile.ItemSpec);

        var features = FeatureExtensions.Parse(Features);
        var scope =
#if NETCOREAPP
            Enum.Parse<Visibility>(Scope.Trim(), true);
#else
            (Visibility)Enum.Parse(typeof(Visibility), Scope.Trim(), true);
#endif

        Log.LogMessage($"Features: {features:F}");
        Log.LogMessage($"Scope: {scope:G}");
        Log.LogMessage($"Assembly: {Assembly.ItemSpec}");
        byte[] data = File.ReadAllBytes(Assembly.ItemSpec);
        Log.LogMessage($"Assembly size: {data.Length} bytes");
        byte[]? pdbData = null;
        byte[]? snkBytes = null;
        if (PdbFile is not null)
        {
            Log.LogMessage($"PdbFile: {PdbFile.ItemSpec}");
            pdbData = File.ReadAllBytes(PdbFile.ItemSpec);
            Log.LogMessage($"PdbFile size: {pdbData.Length} bytes");
        }
        else
        {
            Log.LogMessage("PdbFile is null");
        }

        if (StrongNamingKey is not null)
        {
            Log.LogMessage($"StrongNamingKey: {StrongNamingKey.ItemSpec}");
            snkBytes = File.ReadAllBytes(StrongNamingKey.ItemSpec);
            Log.LogMessage($"StrongNamingKey size: {snkBytes.Length} bytes");
            Log.LogMessage($"DelaySign: {DelaySign}");
        }
        else
        {
            Log.LogMessage("StrongNamingKey is null");
        }

        cts.Token.ThrowIfCancellationRequested();

        var settings = new VilensSettings
        {
            Features = features,
            Scope = scope,
            DelaySign = DelaySign,
            StrongNamingKey = snkBytes
        };

        Scrambler.Scramble(data, pdbData, settings, out byte[] newData, out byte[] newPdbData, cts.Token);

        Log.LogMessage($"Saving assembly to: {Assembly.ItemSpec}");
        File.WriteAllBytes(Assembly.ItemSpec, newData);
        double percent = (double)(newData.Length - data.Length) / data.Length;
        Log.LogMessage($"Assembly size changed by {percent:p}");

        if (pdbData is not null)
        {
            Debug.Assert(PdbFile is not null, "PdbFile is not null");
            Log.LogMessage($"Saving PDB to: {PdbFile!.ItemSpec}");
            File.WriteAllBytes(PdbFile!.ItemSpec, newPdbData);
            double percentPdb = (double)(newPdbData.Length - pdbData.Length) / pdbData.Length;
            Log.LogMessage($"PDB size changed by {percentPdb:p}");
        }

        var tagFile = Assembly.ItemSpec + ".vilens.done";
        Log.LogMessage($"Creating tag file: {tagFile}");
        File.WriteAllBytes(tagFile, []);
        var assemblyTime = File.GetLastWriteTimeUtc(Assembly.ItemSpec);
        while (File.GetLastWriteTimeUtc(tagFile) <= assemblyTime)
        {
            cts.Token.ThrowIfCancellationRequested();
            Thread.Sleep(5);
            File.WriteAllBytes(tagFile, []); // touch
                                             // we need to touch the tag file to indicate that the assembly has been processed
                                             // normally, we would do this using the Touch task of MSBuild, but that only uses second precision on linux for some reason (bug?)
        }

        Logger.CloseTargetFile();
        Log.LogMessage(MessageImportance.High, "Obfuscated {0}", Assembly.ItemSpec);
        Log.LogMessage("Task completed in {0}", sw.Elapsed);
    }
}
