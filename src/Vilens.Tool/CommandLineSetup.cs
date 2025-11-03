using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using Vilens.Data;
using Vilens.Logging;

namespace Vilens;

internal static class CommandLineSetup
{
    internal static Command SetUpCommand(CancellationToken token)
    {
        var rootCommand = new RootCommand("Vilens IS an option!")
        {
            TreatUnmatchedTokensAsErrors = true
        };

        var targetFile = new Argument<FileInfo>("file")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "The .NET assembly file that will be scrambled."
        };
        targetFile.AcceptExistingOnly();
        rootCommand.Arguments.Add(targetFile);

        var strongNamingKey = new Option<FileInfo>("--strongNamingKey", "-snk")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "The strong naming key to sign the assembly with."
        };
        strongNamingKey.AcceptExistingOnly();
        rootCommand.Options.Add(strongNamingKey);

        var delaySign = new Option<bool>("--delaySign", "-ds")
        {
            Description = "Sign the assembly only using the public key."
        };
        rootCommand.Options.Add(delaySign);

        var features = new Option<VilensFeature>("--features", "-f")
        {
            Arity = ArgumentArity.OneOrMore,
            Description = "The features to apply.",
            AllowMultipleArgumentsPerToken = true,
            CustomParser = ParseFeatureFlags,
            DefaultValueFactory = _ => VilensFeature.All
        };
        rootCommand.Options.Add(features);

        var scope = new Option<Visibility>("--scope", "-s")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "TODO",
            DefaultValueFactory = _ => Visibility.Auto
        };
        rootCommand.Options.Add(scope);

#if DEBUG
        var debug = new Option<bool>("--debug");
        rootCommand.Options.Add(debug);
#endif

        rootCommand.SetAction(Handler);

        return rootCommand;

        async Task<int> Handler(ParseResult parsed, CancellationToken token)
        {
#if DEBUG
            if (parsed.GetValue(debug))
            {
                Debugger.Launch();
            }
#endif
            FileInfo file = parsed.GetRequiredValue(targetFile);
            Logger.SetTargetFile($"{file.FullName}.vilens.log");
            try
            {
                var snkFilePath = parsed.GetValue(strongNamingKey)?.FullName;
                var settings = new VilensSettings
                {
                    Features = parsed.GetValue(features),
                    Scope = parsed.GetValue(scope),
                    DelaySign = parsed.GetValue(delaySign),
                    StrongNamingKey = snkFilePath is not null
                        ? await File.ReadAllBytesAsync(snkFilePath, token)
                        : null
                };

                await ScrambleAsync(file, settings, token);
                return 0;
            }
            catch (OperationCanceledException ex)
            {
                new Logger(string.Empty).Error(ex.Message);
                return 3;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                new Logger(string.Empty).Fatal(ex);
                return 1;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }

    private static Logger Log { get; } = new("Vilens.Tool");

    public static async Task ScrambleAsync(FileInfo file, VilensSettings settings, CancellationToken token)
    {
        Log.Info("Loading assembly from: {0}", file.FullName);
        byte[] data = await File.ReadAllBytesAsync(file.FullName, token);
        Log.Trace("Read {0} bytes", data.Length);

        var pdbFile = new FileInfo(Path.ChangeExtension(file.FullName, ".pdb"));
        byte[]? pdbData = null;
        if (pdbFile.Exists)
        {
            Log.Debug("Loading PDB from: {0}", pdbFile.FullName);
            pdbData = await File.ReadAllBytesAsync(pdbFile.FullName, token);
            Log.Trace("Read {0} bytes", pdbData.Length);
        }
        else
        {
            Log.Debug("PDB file not found");
        }

        Scrambler.Scramble(data, pdbData, settings, out var newData, out var newPdbData, token);

        long oldSize = file.Length;
        long oldPdbSize = pdbFile.Exists ? pdbFile.Length : 0;

        Log.Info("Saving assembly to: {0}", file.FullName);

        await File.WriteAllBytesAsync(file.FullName, newData, token);
        file.Refresh();
        long deltaSize = file.Length - oldSize;
        double percent = (double)deltaSize / oldSize;
        Log.Info("File size changed by {0:p}", percent);

        if (pdbFile.Exists)
        {
            Debug.Assert(newPdbData.Length > 0, "pdbStream.Length > 0");
            await File.WriteAllBytesAsync(pdbFile.FullName, newPdbData, token);
            pdbFile.Refresh();
            long deltaPdbSize = pdbFile.Length - oldPdbSize;
            double percentPdb = (double)deltaPdbSize / oldPdbSize;
            Log.Debug("PDB size changed by {0:p}", percentPdb);
        }
    }

    private static VilensFeature ParseFeatureFlags(ArgumentResult result)
    {
        return result.Tokens.Select(t => FeatureExtensions.Parse(t.Value)).Aggregate((a, b) => a | b);
    }
}
