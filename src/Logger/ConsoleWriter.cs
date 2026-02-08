using System;

namespace Logger;

public class ConsoleWriter : IWriter
{
    public void Write(LogLevel level, string message) =>
        Console.WriteLine(message);
}
