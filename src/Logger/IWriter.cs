namespace Logger;

public interface IWriter
{
    void Write(LogLevel level, string message);
}
