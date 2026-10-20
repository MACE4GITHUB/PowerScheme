using System;
using System.Collections.Generic;

namespace Languages;

public sealed class LanguageDefinition
{
    public string Code { get; }
    public string Name { get; }
    internal Lazy<IReadOnlyDictionary<string, string>> Strings { get; }

    internal LanguageDefinition(string code, string name, string path)
    {
        Code = code;
        Name = name;
        Strings = new(() => LanguageResources.LoadFile(path));
    }
}