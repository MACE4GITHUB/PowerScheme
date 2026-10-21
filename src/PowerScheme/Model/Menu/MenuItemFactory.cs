using System.Drawing;
using System.Windows.Forms;
using PowerScheme.Utility;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu;

internal static class MenuItemFactory
{
    internal static ToolStripMenuItem Create(
        MenuItm menuItm,
        string? text = null,
        Bitmap? image = null)
    {
        var item = new ToolStripMenuItem
        {
            Name = $"{menuItm}",
            Text = text ?? MenuItems[menuItm].Name,
            Image = image ?? MenuItems[menuItm].Picture.GetMenuImage(),
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            ToolTipText = MenuItems[menuItm].Description
        };

        return item;
    }

    internal static void Update(ToolStripMenuItem item, MenuItm kind, string? text = null)
    {
        item.Name = kind.ToString();
        item.Text = text ?? MenuItems[kind].Name;
        item.ToolTipText = MenuItems[kind].Description;
        var image = MenuItems[kind].Picture.GetMenuImage();
        if (!ReferenceEquals(item.Image, image))
        {
            var previous = item.Image;
            item.Image = image;
            ImageExtensions.DisposeOwnedImage(previous);
        }
    }
}
