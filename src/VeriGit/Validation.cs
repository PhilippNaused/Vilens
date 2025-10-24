using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace VeriGit;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1849 // Call async methods when in an async method

public static class Validation
{
    public static void Validate(string actual, string extension = "txt", string? targetName = null, [CallerFilePath] string callerFilePath = "")
    {
        string sourceDir = Directory.GetParent(callerFilePath)?.FullName ?? throw new InvalidOperationException();
        var ctx = TestContext.CurrentContext;
        var test = ctx.Test;
        var fileName = test.FullName;
        fileName = FileNameEscape(fileName);
        char sep = Path.DirectorySeparatorChar;
        if (test.ClassName is not null && fileName.StartsWith(test.ClassName + "."))
        {
            fileName = $"{test.ClassName}{sep}{fileName.Substring(test.ClassName!.Length + 1)}";
        }
        if (test.Namespace is not null && fileName.StartsWith(test.Namespace + "."))
        {
            fileName = fileName.Substring(test.Namespace!.Length + 1);
        }
        if (targetName is not null)
        {
            fileName = $"{fileName}{sep}{targetName}";
        }
        fileName = $"{fileName}.{extension}";
        var filePath = Path.Combine(sourceDir, "Snapshots", fileName);
        DiffFile(actual, filePath);
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

    private static void DiffFile(string actual, string path)
    {
        bool overwrite;
        if (File.Exists(path))
        {
            var before = File.ReadAllText(path, encoding);
            overwrite = before != actual;
        }
        else
        {
            Directory.GetParent(path)!.Create();
            overwrite = true;
        }

        if (overwrite)
        {
            File.WriteAllText(path, actual, encoding);
        }

        var status = GetFileStatus(path);
        var text = path;
        if (status is FileStatus.Modified)
        {
            text = RunCommand("git", $"diff \"{path}\"");
        }
        Assert.That(status, Is.EqualTo(FileStatus.Unmodified), text);
    }

    private static FileStatus GetFileStatus(string path)
    {
        if (!File.Exists(path))
        {
            return FileStatus.Missing;
        }
        var status = RunCommand("git", $"status -z --no-renames \"{path}\"");
        if (string.IsNullOrWhiteSpace(status))
        {
            return FileStatus.Unmodified;
        }
        Debug.Assert(status.Length >= 2, "status.Length >= 2");
        //char x = status[0];
        char y = status[1];

        // https://git-scm.com/docs/git-status#_output
        if (y == ' ')
            return FileStatus.Unmodified;
        if (y == '?')
            return FileStatus.Untracked;
        if (y == 'D') // deleted
            return FileStatus.Missing;
        if (y == 'M')
            return FileStatus.Modified;
        Debug.Fail($"Unexpected git status:\n{status}");
        return FileStatus.Unknown;
    }

    private enum FileStatus
    {
        Unknown,
        Missing,
        Modified,
        Untracked,
        Unmodified
    }

    private const int MaxProcessCount = 1;

    private static readonly Semaphore semaphore = new(MaxProcessCount, MaxProcessCount);

    private static string RunCommand(string filePath, string arguments)
    {
        return RunCommandAsync(filePath, arguments).Result;
    }

    private static async Task<string> RunCommandAsync(string filePath, string arguments)
    {
        semaphore.WaitOne();
        try
        {
            var token = TestContext.CurrentContext.CancellationToken;
            var info = new ProcessStartInfo(filePath, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
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
