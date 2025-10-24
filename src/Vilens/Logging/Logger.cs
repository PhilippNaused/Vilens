using System.Diagnostics;
using NLog;
using NLog.Targets;

namespace Vilens.Logging;

internal sealed class Logger(string name)
{
    public string Name => name;

#if DEBUG
    private const string FileLayout = "${longdate} [${processid}:${pad:padding=2:padcharacter=0:${threadid}}] ${level:uppercase=true} ${logger} - ${message:withexception=true}";
    private const string ConsoleLayout = "${when:when=level<=LogLevel.Info:inner=:else=${level:format=FullName}\\: }${logger:shortName=true} - ${message:withexception=true}";
#else
    private const string FileLayout = "${longdate} [${processid}] ${level:uppercase=true} ${message:withexception=true}";
    private const string ConsoleLayout = "${when:when=level<=LogLevel.Info:inner=:else=${level:format=FullName}\\: }${message:withexception=true}";
#endif

    private readonly NLog.Logger _logger = LogManager.GetLogger(name);

    private static readonly FileTarget TargetFile = new("targetFile")
    {
        Layout = FileLayout,
        KeepFileOpen = true,
    };

    private static readonly ColoredConsoleTarget ConsoleLog = new("console")
    {
        EnableAnsiOutput = true,
        Layout = ConsoleLayout
    };

    internal static void SetTargetFile(string path)
    {
        TargetFile.FileName = path;
        LogManager.Configuration?.AddRule(LogLevel.Trace, LogLevel.Fatal, TargetFile);
        LogManager.ReconfigExistingLoggers();
    }

    internal static void EnableLog()
    {
        LogManager.Configuration?.AddRule(LogLevel.Info, LogLevel.Fatal, ConsoleLog);
        LogManager.ReconfigExistingLoggers();
    }

    static Logger()
    {
        var config = new NLog.Config.LoggingConfiguration();
        ConsoleLog.RowHighlightingRules.Add(new("level == LogLevel.Warn", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange));
        ConsoleLog.RowHighlightingRules.Add(new("level == LogLevel.Error", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));
        ConsoleLog.RowHighlightingRules.Add(new("level == LogLevel.Fatal", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange));

        // Apply config
        LogManager.Configuration = config;

#if DEBUG
        LogManager.ThrowExceptions = true;
        LogManager.GlobalThreshold = LogLevel.Trace;
#else
        LogManager.GlobalThreshold = LogLevel.Info;
#endif
    }

    [Conditional("DEBUG")]
    public void Trace<T>(string message, T arg1) => _logger.Trace(message, arg1);
    [Conditional("DEBUG")]
    public void Trace(string message, params object?[] args) => _logger.Trace(message, args);
    [Conditional("DEBUG")]
    public void Debug<T>(string message, T arg1) => _logger.Debug(message, arg1);
    [Conditional("DEBUG")]
    public void Debug(string message, params object?[] args) => _logger.Debug(message, args);

    public void Info<T>(string message, T arg1) => _logger.Info(message, arg1);
    public void Info(string message, params object?[] args) => _logger.Info(message, args);
    public void Warn<T>(string message, T arg1) => _logger.Warn(message, arg1);
    public void Warn(string message, params object?[] args) => _logger.Warn(message, args);

    public void Error<T>(string message, T arg1) => _logger.Error(message, arg1);
    public void Error(string message, params object?[] args) => _logger.Error(message, args);
    public void Error(Exception ex, string message, params object?[] args) => _logger.Error(ex, message, args);

    public void Fatal(string message, params object?[] args) => _logger.Fatal(message, args);
    public void Fatal(Exception ex, string message, params object?[] args) => _logger.Fatal(ex, message, args);
    public void Fatal(Exception ex) => _logger.Fatal(ex);
}
