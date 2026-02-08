using System;
using System.IO;

namespace Logger;

public class FileWriter : IWriter
{
    public FileWriter(string path)
    {
        Path = path;
        FileName = "app.log";
        FullPath = System.IO.Path.Combine(Path, FileName);

        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }

        if (!File.Exists(FullPath))
        {
            using (File.Create(FullPath))
            {
                // Do nothing
            }
        }
    }

    public string Path { get; }

    public string FileName { get; }

    public string FullPath { get; }

    public void Write(LogLevel level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var resultMessage = $"{timestamp} [{level}] {message}";

        File.AppendAllText(FullPath, resultMessage + Environment.NewLine);
    }
}

