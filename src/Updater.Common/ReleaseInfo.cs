using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
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

    private static async Task<GitHubReleaseInfo> GetGitHubReleaseInfoAsync(ApiUrl apiUrl, FileExtension fileExtension)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("request"); // GitHub requires User-Agent
        var json = await client.GetStringAsync(apiUrl.Value);

        var tagMatch = Regex.Match(json, @"""tag_name"":\s*""([^""]+)""");
        if (!tagMatch.Success)
        {
            throw new ArgumentException("tag_name not found");
        }

        var tag = tagMatch.Groups[1].Value;
        if (tag.StartsWith("v"))
        {
            tag = tag.Substring(1);
        }

        var draftMatch = Regex.Match(json, @"""draft"":\s*(true|false)");
        if (!draftMatch.Success)
        {
            throw new ArgumentException("draft not found");
        }

        var draft = bool.Parse(draftMatch.Groups[1].Value);

        var prereleaseMatch = Regex.Match(json, @"""prerelease"":\s*(true|false)");
        if (!prereleaseMatch.Success)
        {
            throw new ArgumentException("prerelease not found");
        }

        var prerelease = bool.Parse(prereleaseMatch.Groups[1].Value);

        var assetMatch = Regex.Match(json, $@"""name"":\s*""([^""]+{Regex.Escape(fileExtension.Value)})""[\s\S]*?""browser_download_url"":\s*""([^""]+)""");
        if (!assetMatch.Success)
        {
            throw new ArgumentException($"Asset with extension {fileExtension} not found.");
        }

        var assetUrl = assetMatch.Groups[2].Value;

        return new GitHubReleaseInfo
        {
            Version = new Version(tag),
            AssetUrl = assetUrl,
            Draft = draft,
            Prerelease = prerelease
        };
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
