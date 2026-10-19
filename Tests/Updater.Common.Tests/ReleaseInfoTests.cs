using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Updater.Common.Tests;

[TestClass]
public sealed class ReleaseInfoTests
{
    private const string ApiUrlValue = "https://api.github.com/repos/MACE4GITHUB/PowerScheme/releases/latest";
    private const string AssetName = "PowerSchemeSetup.exe";
    private const string AssetUrlValue =
        "https://github.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe";

    private const string ValidJson = """
        {
          "tag_name": "v3.2.1",
          "draft": false,
          "prerelease": false,
          "assets": [
            {
              "name": "PowerSchemeSetup.exe",
              "id": 987654321,
              "browser_download_url": "https://github.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe"
            }
          ]
        }
        """;

    [TestMethod]
    public void ParseGitHubReleaseInfo_ValidRelease_ReturnsValues()
    {
        var info = ReleaseInfo.ParseGitHubReleaseInfo(ValidJson, CreateApiUrl(), AssetName);

        Assert.AreEqual<Version>(new Version("3.2.1"), info.Version);
        Assert.AreEqual<long>(987654321L, info.AssetId);
        Assert.AreEqual<string>(AssetUrlValue, info.AssetUrl);
        Assert.IsFalse(info.Draft, "draft must be false.");
        Assert.IsFalse(info.Prerelease, "prerelease must be false.");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_TagWithoutPrefix_IsParsed()
    {
        var json = ValidJson.Replace("\"tag_name\": \"v3.2.1\"", "\"tag_name\": \"3.2.1\"");

        var info = ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName);

        Assert.AreEqual<Version>(new Version("3.2.1"), info.Version);
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_DraftAndPrereleaseFlags_AreReturned()
    {
        var json = ValidJson
            .Replace("\"draft\": false", "\"draft\": true")
            .Replace("\"prerelease\": false", "\"prerelease\": true");

        var info = ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName);

        Assert.IsTrue(info.Draft, "draft must be true.");
        Assert.IsTrue(info.Prerelease, "prerelease must be true.");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_MissingTagName_ThrowsArgumentException()
    {
        var json = ValidJson.Replace("\"tag_name\": \"v3.2.1\",", string.Empty);

        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));

        StringAssert.Contains(exception.Message, "tag_name");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_MissingDraft_ThrowsArgumentException()
    {
        var json = ValidJson.Replace("\"draft\": false,", string.Empty);

        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));

        StringAssert.Contains(exception.Message, "draft");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_MissingPrerelease_ThrowsArgumentException()
    {
        var json = ValidJson.Replace("\"prerelease\": false,", string.Empty);

        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));

        StringAssert.Contains(exception.Message, "prerelease");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_WrongFlagType_ThrowsArgumentException()
    {
        var json = ValidJson.Replace("\"draft\": false", "\"draft\": \"false\"");

        Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_MissingAsset_ThrowsArgumentException()
    {
        var json = ValidJson.Replace("\"name\": \"PowerSchemeSetup.exe\"", "\"name\": \"Other.exe\"");

        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));

        StringAssert.Contains(exception.Message, AssetName);
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_NoAssets_ThrowsArgumentException()
    {
        const string json = """{"tag_name":"v3.2.1","draft":false,"prerelease":false}""";

        Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));
    }

    [TestMethod]
    [DataRow("http://github.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe")]
    [DataRow("https://evil.example.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe")]
    [DataRow("https://github.com/MACE4GITHUB/Other/releases/download/v3.2.1/PowerSchemeSetup.exe")]
    public void ParseGitHubReleaseInfo_DisallowedAssetUrl_ThrowsArgumentException(string assetUrl)
    {
        var json = ValidJson.Replace(AssetUrlValue, assetUrl);

        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo(json, CreateApiUrl(), AssetName));

        StringAssert.Contains(exception.Message, "not allowed");
    }

    [TestMethod]
    public void ParseGitHubReleaseInfo_InvalidJson_ThrowsFormatException()
    {
        Assert.ThrowsExactly<FormatException>(
            () => ReleaseInfo.ParseGitHubReleaseInfo("{", CreateApiUrl(), AssetName));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("http://api.github.com/repos/MACE4GITHUB/PowerScheme/releases/latest")]
    [DataRow("/repos/MACE4GITHUB/PowerScheme/releases/latest")]
    public void ApiUrl_InvalidValue_ThrowsArgumentException(string value)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new ApiUrl(value));
    }

    private static ApiUrl CreateApiUrl() => new(ApiUrlValue);
}
