using System;
using System.Collections.Generic;
using Common;
using Languages;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Model;

public class PowerScheme(
    Guid guid,
    bool isNative,
    ImageItem picture,
    bool isVisible = true,
    bool isMaxPerformance = false) :
    IPowerScheme
{
    public string Name => GetName(Guid);

    public string? Description => GetDescription(Guid);

    public bool IsNative { get; } = isNative;

    public bool IsVisible { get; } = isVisible;

    public bool IsMaxPerformance { get; } = isMaxPerformance;

    public bool IsActive => PowerManager.GetActivePlan() == Guid;

    public Guid Guid { get; } = guid;

    public ImageItem Picture { get; } = picture;

    /// <summary>
    /// Native (built-in) Windows power schemes are not renamed in the registry.
    /// Their name and description are bound to the application language instead.
    /// </summary>
    private static readonly IReadOnlyDictionary<Guid, NativeSchemeText> NativeSchemeTexts =
        new Dictionary<Guid, NativeSchemeText>
        {
            { new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"), l => (l.HighName, l.HighDescription) },
            { new Guid("381b4222-f694-41f0-9685-ff5bb260df2e"), l => (l.BalanceName, l.BalanceDescription) },
            { new Guid("a1841308-3541-4fab-bc81-f71556f20b4a"), l => (l.LowName, l.LowDescription) },
        };

    private static string GetName(Guid guid)
    {
        if (NativeSchemeTexts.TryGetValue(guid, out var getText))
        {
            return getText(Language.Current).Name;
        }

        return PowerManager.GetPlanName(guid) ?? string.Empty;
    }

    private static string? GetDescription(Guid guid)
    {
        if (NativeSchemeTexts.TryGetValue(guid, out var getText))
        {
            return getText(Language.Current).Description;
        }

        return PowerManager.GetPlanDescription(guid);
    }

    private delegate (string Name, string? Description) NativeSchemeText(Language language);
}
