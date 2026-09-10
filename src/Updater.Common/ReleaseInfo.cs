using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Updater.Common;

public sealed class ReleaseInfo
{
    private ReleaseInfo(Version localVersion, Version remoteVersion, string assetUrl, long assetId)
    {
        LocalVersion = localVersion;
        RemoteVersion = remoteVersion;
        NewVersionAvailable = LocalVersion < RemoteVersion;
        AssetUrl = assetUrl;
        AssetId = assetId;
    }

    public static ReleaseInfo Empty =>
        new(new Version(), new Version(), string.Empty, 0);

    public ReleaseInfo Clone() =>
        new(LocalVersion, RemoteVersion, AssetUrl, AssetId);

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

        return new ReleaseInfo(localVersion, gitHubReleaseInfo.Version, gitHubReleaseInfo.AssetUrl, gitHubReleaseInfo.AssetId);
    }

    public Version RemoteVersion { get; }

    public Version LocalVersion { get; }

    public string AssetUrl { get; }

    public long AssetId { get; }

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

        var asset = FindAsset(root, fileExtension);
        if (asset == null)
        {
            throw new ArgumentException($"Asset with extension {fileExtension} not found.");
        }

        if (!IsAllowedDownloadUrl(asset.Url, apiUrl.Value))
        {
            throw new ArgumentException($"Download URL is not allowed: {asset.Url}");
        }

        return new GitHubReleaseInfo
        {
            Version = new Version(tag),
            AssetUrl = asset.Url,
            AssetId = asset.Id,
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

    private static ReleaseAsset? FindAsset(JsonElement root, FileExtension fileExtension)
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
                !asset.TryGetProperty("id", out var idElement) ||
                idElement.ValueKind != JsonValueKind.Number ||
                !TryGetStringProperty(asset, "browser_download_url", out var url))
            {
                continue;
            }

            return new ReleaseAsset(idElement.GetInt64(), url!);
        }

        return null;
    }

    public string GetAssetDownloadUrl(ApiUrl apiUrl)
    {
        var uri = new Uri(apiUrl.Value);
        var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 4 ||
            !segments[0].Equals("repos", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Api URL must follow the GitHub releases endpoint format.");
        }

        // https://api.github.com/repos/{owner}/{repo}/releases/assets/{id}
        var path = $"/repos/{segments[1]}/{segments[2]}/releases/assets/{AssetId}";
        var builder = new UriBuilder(uri) { Path = path, Query = string.Empty, Fragment = string.Empty };
        return builder.Uri.ToString();
    }

    private static bool IsAllowedDownloadUrl(string url, string apiUrl)
    {
        // Only HTTPS links to GitHub domains are trusted as update sources.
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        if (IsGitHubHost(uri.Host) && TryGetRepositoryPrefix(apiUrl, out var repositoryPrefix))
        {
            // For github.com the path must point to the release download of this repository.
            return uri.AbsolutePath.StartsWith(repositoryPrefix, StringComparison.OrdinalIgnoreCase);
        }

        return IsGitHubContentHost(uri.Host);
    }

    private static bool IsGitHubHost(string host) =>
        host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
        host.Equals("www.github.com", StringComparison.OrdinalIgnoreCase);

    private static bool IsGitHubContentHost(string host) =>
        host.Equals("objects.githubusercontent.com", StringComparison.OrdinalIgnoreCase) ||
        host.Equals("release-assets.githubusercontent.com", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetRepositoryPrefix(string apiUrl, out string prefix)
    {
        // api.github.com/repos/{owner}/{repo}/releases/latest
        if (Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri))
        {
            var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 4 &&
                segments[0].Equals("repos", StringComparison.OrdinalIgnoreCase))
            {
                prefix = $"/{segments[1]}/{segments[2]}/";
                return true;
            }
        }

        prefix = string.Empty;
        return false;
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
        public long AssetId { get; set; }
        public bool Draft { get; set; } = true;
        public bool Prerelease { get; set; } = true;
    }

    private sealed class ReleaseAsset(long id, string url)
    {
        public long Id { get; } = id;

        public string Url { get; } = url;
    }
}
