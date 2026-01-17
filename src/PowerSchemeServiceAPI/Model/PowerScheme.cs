using System;
using Common;
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
    public string Name { get; } = PowerManager.GetPlanName(guid);

    public string Description { get; } = PowerManager.GetPlanDescription(guid);

    public bool IsNative { get; } = isNative;

    public bool IsVisible { get; } = isVisible;

    public bool IsMaxPerformance { get; } = isMaxPerformance;

    public bool IsActive => PowerManager.GetActivePlan() == Guid;

    public Guid Guid { get; } = guid;

    public ImageItem Picture { get; } = picture;
}
