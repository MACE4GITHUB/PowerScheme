using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Updater.Common;

public sealed class ReleaseInfo
{
    private ReleaseInfo(Version localVersion, Version remoteVersion, string assetUrl)
    {
        LocalVersion = localVersion;
        RemoteVersion = remoteVersion;
        NewVersionAvailable = LocalVersion < RemoteVersion;
        AssetUrl = assetUrl;
    }

    public static ReleaseInfo Empty =>
        new(new Version(), new Version(), string.Empty);

    public ReleaseInfo Clone() =>
        new(LocalVersion, RemoteVersion, AssetUrl);

    public static async Task<ReleaseInfo> CreateAsync(Arguments arguments)
    {
        GitHubReleaseInfo gitHubReleaseInfo;
        try
        {
            gitHubReleaseInfo = await GetGitHubReleaseInfoAsync(arguments.ApiUrl, arguments.FileExtension);
        }
        catch
        {
            throw new ArgumentException("The api.github service is unavailable.");
        }

        if (gitHubReleaseInfo.Draft || gitHubReleaseInfo.Prerelease)
        {
            throw new ArgumentException("Latest release is draft or prerelease. Skipping update.");
        }

        var localVersion = GetLocalFileVersion(arguments.LocalFilePath);

        return new ReleaseInfo(localVersion, gitHubReleaseInfo.Version, gitHubReleaseInfo.AssetUrl);
    }

    public Version RemoteVersion { get; }

    public Version LocalVersion { get; }

    public string AssetUrl { get; }

    public bool NewVersionAvailable { get; }

    public override string ToString() =>
        $"""
            New version available: {NewVersionAvailable}
            Current version: {LocalVersion}, Latest version: {RemoteVersion}"
            Download URL: {AssetUrl}
            """;

    private static async Task<GitHubReleaseInfo> GetGitHubReleaseInfoAsync(ApiUrl apiUrl, FileExtension fileExtension)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("request"); // GitHub requires User-Agent
        var json = await client.GetStringAsync(apiUrl.Value);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (!TryGetStringProperty(root, "tag_name", out var tag))
        {
            throw new ArgumentException("tag_name not found");
        }

        if (tag!.StartsWith("v"))
        {
            tag = tag.Substring(1);
        }

        if (!TryGetBooleanProperty(root, "draft", out var draft))
        {
            throw new ArgumentException("draft not found");
        }

        if (!TryGetBooleanProperty(root, "prerelease", out var prerelease))
        {
            throw new ArgumentException("prerelease not found");
        }

        var assetUrl = FindAssetUrl(root, fileExtension);
        if (assetUrl == null)
        {
            throw new ArgumentException($"Asset with extension {fileExtension} not found.");
        }

        return new GitHubReleaseInfo
        {
            Version = new Version(tag),
            AssetUrl = assetUrl,
            Draft = draft,
            Prerelease = prerelease
        };
    }

    private static bool TryGetStringProperty(JsonElement element, string propertyName, out string? value)
    {
        if (element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString();
            return value != null;
        }

        value = null;
        return false;
    }

    private static bool TryGetBooleanProperty(JsonElement element, string propertyName, out bool value)
    {
        if (element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            value = property.GetBoolean();
            return true;
        }

        value = false;
        return false;
    }

    private static string? FindAssetUrl(JsonElement root, FileExtension fileExtension)
    {
        if (!root.TryGetProperty("assets", out var assets) ||
            assets.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var asset in assets.EnumerateArray())
        {
            if (asset.ValueKind != JsonValueKind.Object ||
                !TryGetStringProperty(asset, "name", out var name) ||
                !name!.EndsWith(fileExtension.Value, StringComparison.OrdinalIgnoreCase) ||
                !TryGetStringProperty(asset, "browser_download_url", out var url))
            {
                continue;
            }

            return url;
        }

        return null;
    }

    private static Version GetLocalFileVersion(LocalFilePath localFilePath)
    {
        var info = FileVersionInfo.GetVersionInfo(localFilePath.Value);

        return new Version(info.FileVersion ?? "0.0.0.0");
    }

    private sealed class GitHubReleaseInfo
    {
        public Version Version { get; set; } = new Version();
        public string AssetUrl { get; set; } = string.Empty;
        public bool Draft { get; set; } = true;
        public bool Prerelease { get; set; } = true;
    }
}
