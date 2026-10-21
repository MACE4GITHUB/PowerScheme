using System;
using Common;
using PowerSchemeServiceAPI.Model;

namespace PowerScheme.Tests.TestDoubles;

internal sealed class TestScheme(Guid guid, bool isNative = true, bool isMaxPerformance = false) : IPowerScheme
{
    public Guid Guid { get; } = guid;
    public bool IsNative { get; } = isNative;
    public bool IsMaxPerformance { get; } = isMaxPerformance;
    public bool IsVisible => true;
    public bool IsActive => throw new NotSupportedException();
    public string Name { get; set; } = "Test scheme";
    public string? Description => throw new NotSupportedException();
    public ImageItem Picture { get; set; } = ImageItem.Empty;
}
