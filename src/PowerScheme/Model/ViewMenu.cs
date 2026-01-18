using Common;

namespace PowerScheme.Model;

internal class ViewMenu(
    string name,
    ImageItem picture,
    object? tag = null,
    string? description = null)
{
    public ViewMenu(string name, string? description = null) :
        this(name, ImageItem.Unknown, null, description)
    { }

    public string Name { get; } = name;

    public string? Description { get; } = description;

    public ImageItem Picture { get; } = picture;

    public object? Tag { get; } = tag;
}
