using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    private void Log(LogLevel level, string message, Exception? exception, params ReadOnlySpan<object?> args)
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
            else if (args.Length == 3)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args[0], args[1], args[2]);
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
    public void Trace<T>([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, T arg1) => Log(LogLevel.Trace, message, null, arg1);
    [Conditional("DEBUG")]
    public void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Trace, message, null, args);

    [Conditional("DEBUG")]
    public void Debug<T>([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, T arg1) => Log(LogLevel.Debug, message, null, arg1);
    [Conditional("DEBUG")]
    public void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Debug, message, null, args);

    public void Info<T>([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, T arg1) => Log(LogLevel.Info, message, null, arg1);
    public void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Info, message, null, args);

    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Warn<T>([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, T arg1) => Log(LogLevel.Warn, message, null, arg1);
    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Warn, message, null, args);

    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Error<T>([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, T arg1) => Log(LogLevel.Error, message, null, arg1);
    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Error, message, null, args);
    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Error(Exception ex, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Error, message, ex, args);

    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Fatal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Fatal, message, null, args);
    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Fatal(Exception ex, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message, params ReadOnlySpan<object?> args) => Log(LogLevel.Fatal, message, ex, args);
    [ExcludeFromCodeCoverage] // This is *supposed* to be dead code, so it's a good thing when it has no coverage
    public void Fatal(Exception ex) => Log(LogLevel.Fatal, "", ex);
}
