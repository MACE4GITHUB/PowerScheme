namespace Logger;

public sealed class Log(
    IWriter loggerProvider) : ILogger
{
    private readonly IWriter _loggerProvider = loggerProvider;

    public Log() :
        this(new ConsoleWriter())
    { }

    public void LogDebug(string message) =>
        LogMessage(LogLevel.Debug, message);

    public void LogInfo(string message) =>
        LogMessage(LogLevel.Information, message);

    public void LogWarning(string message) =>
        LogMessage(LogLevel.Warning, message);

    public void LogError(string message) =>
        LogMessage(LogLevel.Error, message);

    public void LogCritical(string message) =>
        LogMessage(LogLevel.Critical, message);

    private void LogMessage(LogLevel level, string message) =>
        _loggerProvider.Write(level, message);
}
