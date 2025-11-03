using System.Diagnostics;
using System.Globalization;

namespace Vilens.Logging;

internal sealed class Logger(string name)
{
    public string Name => name;

    private static LogFile? _file;

    public static bool ConsoleEnabled { get; set; }

    public static LogLevel LogLevel { get; set; }
#if DEBUG
        = LogLevel.Trace;
#else
        = LogLevel.Info;
#endif

    internal static void SetTargetFile(string path)
    {
        _file?.Dispose();
        _file = new LogFile(path);
    }

    internal static void CloseTargetFile()
    {
        _file?.Dispose();
        _file = null;
    }

    private void Log<T>(LogLevel level, string message, Exception? exception, params ReadOnlySpan<T> args)
    {
        if (level < LogLevel)
        {
            return;
        }
        var file = _file;
        bool logToFile = file is not null;
        bool logToConsole = ConsoleEnabled && level >= LogLevel.Debug;
        if (logToFile || logToConsole)
        {
            if (args.Length == 1)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args[0]);
            }
            else if (args.Length == 2)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args[0], args[1]);
            }
            else if (args.Length > 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args.ToArray());
            }
            if (logToFile)
                file!.Write(level, Name, message, exception);
            if (logToConsole)
                LogConsole(level, message, exception);
        }
    }

    private static void LogConsole(LogLevel level, string message, Exception? exception)
    {
        string text;
        if (level > LogLevel.Info)
        {
            if (exception is not null)
            {
                text = $"{level}: {message}\n{exception}";
            }
            else
            {
                text = $"{level}: {message}";
            }
        }
        else
        {
            System.Diagnostics.Debug.Assert(exception is null, "exception is null");
            text = message;
        }
        Console.WriteLine(text);
    }

    [Conditional("DEBUG")]
    public void Trace<T>(string message, T arg1) => Log(LogLevel.Trace, message, null, arg1);
    [Conditional("DEBUG")]
    public void Trace(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Trace, message, null, args);

    [Conditional("DEBUG")]
    public void Debug<T>(string message, T arg1) => Log(LogLevel.Debug, message, null, arg1);
    [Conditional("DEBUG")]
    public void Debug(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Debug, message, null, args);

    public void Info<T>(string message, T arg1) => Log(LogLevel.Info, message, null, arg1);
    public void Info(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Info, message, null, args);

    public void Warn<T>(string message, T arg1) => Log(LogLevel.Warn, message, null, arg1);
    public void Warn(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Warn, message, null, args);

    public void Error<T>(string message, T arg1) => Log(LogLevel.Error, message, null, arg1);
    public void Error(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Error, message, null, args);
    public void Error(Exception ex, string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Error, message, ex, args);

    public void Fatal(string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Fatal, message, null, args);
    public void Fatal(Exception ex, string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Fatal, message, ex, args);
    public void Fatal(Exception ex) => Log<string>(LogLevel.Fatal, "", ex);
}
