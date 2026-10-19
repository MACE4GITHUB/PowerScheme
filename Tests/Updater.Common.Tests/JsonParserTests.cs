using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Updater.Common.Tests;

[TestClass]
public sealed class JsonParserTests
{
    [TestMethod]
    public void Parse_GitHubRelease_ReturnsExpectedValues()
    {
        const string json = """
            {
              "tag_name": "v3.2.1",
              "draft": false,
              "prerelease": false,
              "assets": [
                {
                  "name": "PowerSchemeSetup.exe",
                  "id": 123456789,
                  "browser_download_url": "https://github.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe"
                }
              ]
            }
            """;

        var root = JsonParser.Parse(json);

        var tag = Property(root, "tag_name");
        Assert.IsInstanceOfType(tag, typeof(JsonString), "tag_name must be a string.");
        Assert.AreEqual<string>("v3.2.1", ((JsonString)tag).Value);

        var draft = Property(root, "draft");
        Assert.IsInstanceOfType(draft, typeof(JsonBoolean), "draft must be a boolean.");
        Assert.IsFalse(((JsonBoolean)draft).Value, "draft must be false.");

        var assets = Property(root, "assets");
        Assert.IsInstanceOfType(assets, typeof(JsonArray), "assets must be an array.");
        var items = ((JsonArray)assets).Items;
        Assert.AreEqual<int>(1, items.Count);

        var asset = items[0];
        Assert.AreEqual<string>("PowerSchemeSetup.exe", ((JsonString)Property(asset, "name")).Value);
        Assert.AreEqual<long>(123456789L, ((JsonNumber)Property(asset, "id")).Integer!.Value);
        Assert.AreEqual<string>(
            "https://github.com/MACE4GITHUB/PowerScheme/releases/download/v3.2.1/PowerSchemeSetup.exe",
            ((JsonString)Property(asset, "browser_download_url")).Value);
    }

    [TestMethod]
    public void Parse_StringEscapes_AreDecoded()
    {
        const string json = """
            {
              "value": "quote: \" backslash: \\ slash: \/ backspace: \b formfeed: \f newline: \n carriage: \r tab: \t unicode: \u0041"
            }
            """;

        const string expected =
            "quote: \" backslash: \\ slash: / backspace: \b formfeed: \f newline: \n carriage: \r tab: \t unicode: A";

        var value = (JsonString)Property(JsonParser.Parse(json), "value");

        Assert.AreEqual<string>(expected, value.Value);
    }

    [TestMethod]
    public void Parse_UnicodeSurrogatePair_IsDecoded()
    {
        const string json = """{"value":"\uD83D\uDE00"}""";

        var value = (JsonString)Property(JsonParser.Parse(json), "value");

        Assert.AreEqual<string>("\uD83D\uDE00", value.Value);
    }

    [TestMethod]
    public void Parse_NestedContainers_AreParsed()
    {
        const string json = """{"outer":{"inner":[1,{"flag":true},null,"text"]}}""";

        var outer = Property(JsonParser.Parse(json), "outer");
        var inner = Property(outer, "inner");

        Assert.IsInstanceOfType(inner, typeof(JsonArray), "inner must be an array.");

        var items = ((JsonArray)inner).Items;
        Assert.AreEqual<int>(4, items.Count);
        Assert.AreEqual<long>(1L, ((JsonNumber)items[0]).Integer!.Value);
        Assert.IsTrue(((JsonBoolean)Property(items[1], "flag")).Value, "flag must be true.");
        Assert.IsInstanceOfType(items[2], typeof(JsonNull), "Third item must be null.");
        Assert.AreEqual<string>("text", ((JsonString)items[3]).Value);
    }

    [TestMethod]
    public void Parse_EmptyContainers_AreParsed()
    {
        const string json = """{"object":{},"array":[]}""";

        var root = JsonParser.Parse(json);

        Assert.AreEqual<int>(0, ((JsonArray)Property(root, "array")).Items.Count);
        Assert.IsInstanceOfType(Property(root, "object"), typeof(JsonObject), "object must be an object.");
    }

    [TestMethod]
    public void Parse_NumberFormats_AreParsed()
    {
        const string json = """{"integer":42,"negative":-7,"floating":1.5,"exponent":1.5e2}""";

        var root = JsonParser.Parse(json);

        Assert.AreEqual<long>(42L, ((JsonNumber)Property(root, "integer")).Integer!.Value);
        Assert.AreEqual<long>(-7L, ((JsonNumber)Property(root, "negative")).Integer!.Value);
        Assert.AreEqual(1.5d, ((JsonNumber)Property(root, "floating")).Value, 0.0001d);
        Assert.AreEqual(150d, ((JsonNumber)Property(root, "exponent")).Value, 0.0001d);
    }

    [TestMethod]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => JsonParser.Parse(null!));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("{\"a\":1,}")]
    [DataRow("{\"a\":}")]
    [DataRow("{\"a\" 1}")]
    [DataRow("{\"a\":\"unterminated}")]
    [DataRow("tru")]
    [DataRow("{\"a\":\"\\u12G4\"}")]
    [DataRow("{} extra")]
    [DataRow("[1 2]")]
    [DataRow("{\"a\":\"\\q\"}")]
    public void Parse_MalformedJson_ThrowsFormatException(string json)
    {
        Assert.ThrowsExactly<FormatException>(() => JsonParser.Parse(json));
    }

    private static JsonNode Property(JsonNode node, string name)
    {
        Assert.IsInstanceOfType(node, typeof(JsonObject), "Expected a JSON object.");

        var jsonObject = (JsonObject)node;

        Assert.IsTrue(jsonObject.TryGetValue(name, out var value), $"Property '{name}' not found.");

        return value!;
    }
}
