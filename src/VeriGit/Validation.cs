using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace VeriGit;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

public static class Validation
{
    public static void Validate(string actual, string extension = "txt", string? targetName = null, [CallerFilePath] string callerFilePath = "")
    {
        ValidateAsync(actual, extension, targetName, callerFilePath).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public static Task ValidateAsync(string actual, string extension = "txt", string? targetName = null, [CallerFilePath] string callerFilePath = "")
    {
        string sourceDir = Directory.GetParent(callerFilePath)?.FullName ?? throw new InvalidOperationException("Caller file path directory is null");
        string fileName = GetFilename();
        if (targetName is not null)
        {
            char sep = Path.DirectorySeparatorChar;
            fileName = $"{fileName}{sep}{targetName}";
        }
        fileName = $"{fileName}.{extension}";
        var filePath = Path.Combine(sourceDir, "Snapshots", fileName);
        return DiffFile(actual, filePath);
    }

    private static string GetFilename()
    {
        var ctx = TestContext.CurrentContext ?? throw new InvalidOperationException("TestContext.CurrentContext is null");
        var test = ctx.Test;
        var fileName = test.FullName;
        fileName = FileNameEscape(fileName);
        if (test.ClassName is not null && fileName.StartsWith(test.ClassName + "."))
        {
            char sep = Path.DirectorySeparatorChar;
            fileName = $"{test.ClassName}{sep}{fileName.Substring(test.ClassName!.Length + 1)}";
        }
        if (test.Namespace is not null && fileName.StartsWith(test.Namespace + "."))
        {
            fileName = fileName.Substring(test.Namespace!.Length + 1);
        }
        return fileName;
    }

    private static readonly Regex invalidPathChars = new($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);

    private static string FileNameEscape(string path)
    {
        return invalidPathChars.Replace(path, Escape);

        static string Escape(Match m)
        {
            var text = m.Value;
            Debug.Assert(text.Length == 1, "text.Length == 1");
            char c = text[0];
            return $"%{(short)c:X}";
        }
    }

    private static readonly Encoding encoding = new UTF8Encoding(false); // no BOM

    private static async Task DiffFile(string actual, string path)
    {
        bool overwrite;
        if (File.Exists(path))
        {
#if NETCOREAPP
            var before = await File.ReadAllTextAsync(path, encoding);
#else
            var before = File.ReadAllText(path, encoding);
#endif
            overwrite = before != actual;
        }
        else
        {
            var dir = Directory.GetParent(path)!;
            if (!dir.Exists)
            {
                dir.Create();
            }
            overwrite = true;
        }

        if (overwrite)
        {
#if NETCOREAPP
            await File.WriteAllTextAsync(path, actual, encoding);
#else
            File.WriteAllText(path, actual, encoding);
#endif
        }

        var status = await GetFileStatus(path);
        if (status is FileStatus.Modified)
        {
            string diff = await RunGitCommandAsync(path, $"diff \"{path}\"");
            throw new ValidationFailedException($"Validation failed for '{path}':{Environment.NewLine}{diff}", path, actual, diff);
        }
        if (status is not FileStatus.Unmodified)
        {
            throw new ValidationFailedException($"Validation failed for '{path}' (status: {status})", path, actual, null);
        }
    }

    private static async Task<FileStatus> GetFileStatus(string path)
    {
        if (!File.Exists(path))
        {
            return FileStatus.Missing;
        }
        var status = await RunGitCommandAsync(path, $"status -z --no-renames \"{path}\"");
        if (string.IsNullOrWhiteSpace(status))
        {
            return FileStatus.Unmodified;
        }
        Debug.Assert(status.Length >= 2, "status.Length >= 2");
        // https://git-scm.com/docs/git-status#_output
        //var x = (FileStatus)status[0]; // index status
        var y = (FileStatus)status[1]; // working tree status
#if NETCOREAPP
        Debug.Assert(Enum.IsDefined(y));
#else
        Debug.Assert(Enum.IsDefined(typeof(FileStatus), y));
#endif
        return y;
    }

    private enum FileStatus : ushort
    {
        Missing = 'D',
        Modified = 'M',
        Untracked = '?',
        Unmodified = ' '
    }

    private const int MaxProcessCount = 2;

    private static readonly SemaphoreSlim semaphore = new(MaxProcessCount, MaxProcessCount);

    private static async Task<string> RunGitCommandAsync(string filePath, string arguments)
    {
        var token = TestContext.CurrentContext.CancellationToken;
        await semaphore.WaitAsync(token);
        try
        {
            var info = new ProcessStartInfo("git", arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                EnvironmentVariables =
                {
                    ["GIT_OPTIONAL_LOCKS"] = "0", // avoid hanging due to lock contention
                    // cspell:ignore PATHSPECS
                    ["GIT_LITERAL_PATHSPECS"] = "0" // no globing
                }
            };
            var process = Process.Start(info)!;
#if NETCOREAPP
            var output = await process.StandardOutput.ReadToEndAsync(token);
            await process.WaitForExitAsync(token);
#else
            var output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();
#endif
            Assert.That(process.HasExited, Is.True, output);
            Assert.That(process.ExitCode, Is.Zero, output);
            return output;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
