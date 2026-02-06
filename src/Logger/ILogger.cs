namespace Logger;

public interface ILogger
{
    void LogCritical(string message);
    void LogDebug(string message);
    void LogError(string message);
    void LogInfo(string message);
    void LogWarning(string message);
}
