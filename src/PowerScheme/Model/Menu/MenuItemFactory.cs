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
            Image = image ?? MenuItems[menuItm].Picture.GetImage(),
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            ToolTipText = MenuItems[menuItm].Description
        };

        return item;
    }
}
