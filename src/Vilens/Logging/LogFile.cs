namespace Vilens.Logging;

internal sealed class LogFile(string path) : IDisposable
{
    private readonly object _lock = new();
    private readonly StreamWriter _writer = new(File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read));

    public void Dispose()
    {
        _writer.Dispose();
    }

    public void Write(LogLevel level, string name, string message, Exception? exception = null)
    {
#if NETCOREAPP
        int processId = Environment.ProcessId;
#else
        int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
#endif
#if DEBUG
        string line = $"{DateTime.Now:s} [{processId}:{Environment.CurrentManagedThreadId:00}] {level.ToString().ToUpper()} {name} - {message}";
#else
        string line = $"{DateTime.Now:s} [{processId}] {level.ToString().ToUpper()} {message}";
#endif
        if (exception is not null)
        {
            line = $"{line}{Environment.NewLine}{exception}";
        }
        lock (_lock)
        {
            _writer.WriteLine(line);
        }
    }
}
