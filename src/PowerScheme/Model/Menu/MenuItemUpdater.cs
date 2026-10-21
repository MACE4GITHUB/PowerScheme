using System.Drawing;
using System.Windows.Forms;
using PowerScheme.Utility;

namespace PowerScheme.Model.Menu;

internal sealed class MenuItemUpdater(ToolStripItemCollection items)
{
    private int _position;

    internal ToolStripMenuItem Item(MenuLookup.MenuItm kind, string? text = null)
    {
        var item = items[kind.ToString()] as ToolStripMenuItem;
        if (item == null)
        {
            item = MenuItemFactory.Create(kind, text);
        }
        else
        {
            MenuItemFactory.Update(item, kind, text);
        }

        Place(item);
        return item;
    }

    internal ToolStripMenuItem Item(string key, string text, Bitmap image)
    {
        var item = items[key] as ToolStripMenuItem ?? new ToolStripMenuItem { Name = key };
        item.Text = text;
        if (!ReferenceEquals(item.Image, image))
        {
            var previous = item.Image;
            item.Image = image;
            ImageExtensions.DisposeOwnedImage(previous);
        }
        Place(item);
        return item;
    }

    internal void Separator()
    {
        var separator = _position < items.Count ? items[_position] as ToolStripSeparator : null;
        Place(separator ?? new ToolStripSeparator());
    }

    internal void Complete()
    {
        while (items.Count > _position)
        {
            var item = items[items.Count - 1];
            if (item is ToolStripMenuItem menuItem)
            {
                menuItem.SafeRemove();
            }
            else
            {
                item.Dispose();
            }
        }
    }

    private void Place(ToolStripItem item)
    {
        if (_position >= items.Count || !ReferenceEquals(items[_position], item))
        {
            items.Remove(item);
            items.Insert(_position, item);
        }
        _position++;
    }
}