using System;
using System.IO;
using System.Linq;

namespace Updater.Common;

public sealed class Arguments
{
    private Arguments(string[] args)
    {
        ApiUrl = new ApiUrl(GetKeyedValue(args, "--api"));
        LocalFilePath = new LocalFilePath(GetKeyedValue(args, "--path"));
        Suffix = new Suffix(GetKeyedValue(args, "--suffix", "_new"));
        FileExtension = new FileExtension(Path.GetExtension(LocalFilePath.Value));
        LocalDirectoryPath = new LocalDirectoryPath(Path.GetDirectoryName(LocalFilePath.Value));
        FileNameWithoutExtension = new FileNameWithoutExtension(Path.GetFileNameWithoutExtension(LocalFilePath.Value));
        DownloadFilePath = new DownloadFilePath(Path.Combine(LocalDirectoryPath.Value, $"{FileNameWithoutExtension}{Suffix}{FileExtension}"));

        KillOldVersion = HasKey(args, "--kill") && IsExecutable();
        ReplaceOldVersion = HasKey(args, "--replace") || KillOldVersion;
        LaunchAfterUpdate = HasKey(args, "--launchAfterUpdate") && KillOldVersion;
    }

    public static Arguments Create(string[] args)
    {
        if (args.Length < 2 ||
            !args.Any(x => x.StartsWith("--api")) ||
            !args.Any(x => x.StartsWith("--path")))
        {
            throw new ArgumentException("Usage: Updater --api:\"apiUrl\" --path:\"path-to-app.exe\" [--suffix:\"_new\"] [--kill] [--replace] [--launchAfterUpdate]");
        }

        return new Arguments(args);
    }

    public bool LaunchAfterUpdate { get; }

    public bool ReplaceOldVersion { get; }

    public bool KillOldVersion { get; }

    public ApiUrl ApiUrl { get; }

    public LocalFilePath LocalFilePath { get; }

    public DownloadFilePath DownloadFilePath { get; }

    public FileExtension FileExtension { get; }

    public LocalDirectoryPath LocalDirectoryPath { get; }

    public FileNameWithoutExtension FileNameWithoutExtension { get; }

    public Suffix Suffix { get; }

    private static string GetKeyedValue(
        string[] args,
        string key,
        string defaultValue = "")
    {
        var value = defaultValue;
        var parts = (from arg in args
                     where arg.StartsWith($"{key}:", StringComparison.OrdinalIgnoreCase)
                     let x = arg.Split([':'], 2)
                     select x)
                     .FirstOrDefault();

        if (parts != null && parts.Length == 2)
        {
            value = parts[1].Trim();
        }

        return value;
    }

    private static bool HasKey(string[] args, string key) =>
        Array.Exists(args, arg => arg.Equals(key, StringComparison.OrdinalIgnoreCase));

    private bool IsExecutable() =>
        FileExtension.Value.Equals(".exe", StringComparison.OrdinalIgnoreCase);
}

public sealed class DownloadFilePath : BasePrimitive
{
    public DownloadFilePath(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Download file path cannot be null or empty.");
        }
    }
}

public sealed class FileNameWithoutExtension : BasePrimitive
{
    public FileNameWithoutExtension(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("File name without extension cannot be null or empty.");
        }
    }
}

public sealed class LocalDirectoryPath : BasePrimitive
{
    public LocalDirectoryPath(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Local directory path cannot be null or empty.");
        }

        if (!Directory.Exists(value))
        {
            throw new ArgumentException($"Directory does not exist: {value}");
        }
    }
}

public sealed class FileExtension : BasePrimitive
{
    public FileExtension(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("File extension cannot be null or empty.");
        }
    }
}

public sealed class Suffix : BasePrimitive
{
    public Suffix(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Suffix cannot be null or empty.");
        }
    }
}

public sealed class LocalFilePath : BasePrimitive
{
    public LocalFilePath(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Local path cannot be null or empty.");
        }

        if (!File.Exists(value))
        {
            throw new ArgumentException($"File not found at specified path: {value}");
        }
    }
}

public sealed class ApiUrl : BasePrimitive
{
    public ApiUrl(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Api URL cannot be null or empty.");
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("Invalid URL format. Must be absolute and start with http or https.");
        }
    }
}

public abstract class BasePrimitive(string value)
{
    public string Value { get; private protected set; } = value;

    public override string ToString() => Value;
}
