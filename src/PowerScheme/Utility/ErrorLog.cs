using System;
using System.IO;
using System.Text;

namespace PowerScheme.Utility;

internal static class ErrorLog
{
    internal const int MaxFileBytes = 1024 * 1024;
    private static readonly object _sync = new();
    internal static string DirectoryPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PowerScheme");

    internal static void Write(string message, string? directory = null)
    {
        try
        {
            var text = $"{DateTime.Now}: {message}";
            var newline = Encoding.UTF8.GetBytes(Environment.NewLine);
            var buffer = new byte[Math.Min(Encoding.UTF8.GetByteCount(text), MaxFileBytes - newline.Length)];
            var characters = text.ToCharArray();
            Encoding.UTF8.GetEncoder().Convert(characters, 0, characters.Length,
                buffer, 0, buffer.Length, true, out _, out var bytesUsed, out _);

            lock (_sync)
            {
                var logDirectory = directory ?? DirectoryPath;
                Directory.CreateDirectory(logDirectory);
                using var stream = new FileStream(Path.Combine(logDirectory, "errors.log"),
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                if (stream.Length + bytesUsed + newline.Length > MaxFileBytes)
                {
                    stream.SetLength(0);
                }

                stream.Position = stream.Length;
                stream.Write(buffer, 0, bytesUsed);
                stream.Write(newline, 0, newline.Length);
            }
        }
        catch (Exception exception)
        {
            System.Diagnostics.Trace.TraceError("Error log write failed: {0}; original message: {1}", exception, message);
        }
    }
}