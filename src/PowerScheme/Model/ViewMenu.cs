using Common;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model;

internal class ViewMenu(
    string name,
    ImageItem picture,
    object? tag = null,
    string? description = null,
    MenuItmKind? menuItmKind = null)
{
    public ViewMenu(string name, string? description = null) :
        this(name, ImageItem.Unknown, null, description)
    { }

    public ViewMenu(
        string name,
        ImageItem picture,
        object tag,
        MenuItmKind menuItmKind) :
        this(name, picture, tag, null, menuItmKind)
    { }

    public MenuItmKind MenuItmKind { get; } = menuItmKind ?? MenuItmKind.Unspecified;

    public string Name { get; } = name;

    public string? Description { get; } = description;

    public ImageItem Picture { get; } = picture;

    public object? Tag { get; } = tag;
}
