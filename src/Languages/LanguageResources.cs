using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Languages;

internal static class LanguageResources
{
    private static readonly Lazy<IReadOnlyDictionary<string, string>> _default = new(() =>
        (Discover(AppDomain.CurrentDomain.BaseDirectory, null).FirstOrDefault(language => language.Code == "en")
            ?? throw new FileNotFoundException("English fallback translation not found in the lang directory.")).Strings.Value);

    internal static string GetDefault(string name) => _default.Value[name];

    internal static Lazy<IReadOnlyDictionary<string, string>> CreateLoader(string locale, string? baseDirectory = null) =>
        new(() => Load(locale, baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory));

    private static IReadOnlyDictionary<string, string> Load(string locale, string baseDirectory)
    {
        var path = Path.Combine(baseDirectory, "lang", $"{locale}.xml");
        return LoadFile(path);
    }

    internal static IReadOnlyDictionary<string, string> LoadFile(string path)
    {
        using var resource = File.OpenRead(path);
        using var reader = XmlReader.Create(resource, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        var document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        if (document.Root?.Name != "resources")
        {
            throw new XmlException("Expected a resources root element.");
        }
        return document.Root.Elements("string").ToDictionary(
            element => (string)element.Attribute("name")!,
            element => element.Value,
            StringComparer.Ordinal);
    }

    internal static IReadOnlyList<LanguageDefinition> Discover(string baseDirectory, Action<string>? reportError)
    {
        var directory = Path.Combine(baseDirectory, "lang");
        if (!Directory.Exists(directory))
        {
            return [];
        }

        var languages = new List<LanguageDefinition>();
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var path in Directory.GetFiles(directory, "*.xml").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    using var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
                    if (reader.MoveToContent() != XmlNodeType.Element || reader.Name != "resources" || !reader.ReadToDescendant("language"))
                    {
                        throw new XmlException("Expected a resources/language element.");
                    }

                    var code = (reader.GetAttribute("code") ?? string.Empty).Trim().ToLowerInvariant();
                    var name = reader.ReadElementContentAsString().Trim();
                    if (string.IsNullOrEmpty(code) || !char.IsLetter(code[0]) || code.Any(character => !char.IsLetterOrDigit(character) && character != '-') || string.IsNullOrWhiteSpace(name))
                    {
                        throw new XmlException("Language code and display name are required.");
                    }
                    if (!codes.Add(code!))
                    {
                        throw new XmlException($"Duplicate language code: {code}");
                    }

                    languages.Add(new LanguageDefinition(code!, name, path));
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or XmlException or ArgumentException)
                {
                    reportError?.Invoke($"Cannot read translation metadata '{path}': {exception.Message}");
                }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            reportError?.Invoke($"Cannot scan translations '{directory}': {exception.Message}");
        }

        return languages.OrderBy(language => language.Name, StringComparer.CurrentCultureIgnoreCase).ToArray();
    }
}