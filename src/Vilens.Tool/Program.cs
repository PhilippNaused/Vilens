using System.Diagnostics;
using Vilens;
using Vilens.Logging;

var stopWatch = Stopwatch.StartNew();
var log = new Logger(nameof(Vilens));
try
{
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, args) =>
    {
        log.Debug("{key} was pressed", args.SpecialKey);
        log.Info("Canceling...");
        args.Cancel = true;
        cts.Cancel();
    };
    Logger.EnableLog();
    log.Debug("Starting command line: '{CommandLine}'", Environment.CommandLine);
    var command = CommandLineSetup.SetUpCommand(cts.Token);
    var exitCode = await command.Parse(args).InvokeAsync();
    log.Debug("Finished in {time}", stopWatch.Elapsed);
    return exitCode;
}
#pragma warning disable CA1031 // Do not catch general exception types
catch (Exception ex)
{
    log.Fatal(ex);
    return 2;
}
#pragma warning restore CA1031 // Do not catch general exception types
finally
{
    NLog.LogManager.Flush();
    NLog.LogManager.Shutdown();
}
