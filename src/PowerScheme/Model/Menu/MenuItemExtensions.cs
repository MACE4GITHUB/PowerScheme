using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Common;
using PowerScheme.Model.Command;
using PowerScheme.Utility;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu;

internal static class MenuItemExtensions
{
    #region ConditionalWeakTable

    private static readonly ConditionalWeakTable<ToolStripMenuItem, CommandEventHandlerAdapter> _map
    = new();

    internal static void BindCommand(this ToolStripMenuItem item, ICommand command)
    {
        var adapter = new CommandEventHandlerAdapter(command);

        _map.Add(item, adapter);
        item.Click += adapter.Handle;
    }

    internal static void UnbindCommand(this ToolStripMenuItem item)
    {
        if (!_map.TryGetValue(item, out var adapter))
        {
            return;
        }

        item.Click -= adapter.Handle;
        _map.Remove(item);
    }

    internal static void SafeRemove(this ToolStripMenuItem parent, ToolStripMenuItem item)
    {
        item.UnbindCommand();
        parent.DropDownItems.Remove(item);
        item.Dispose();
    }

    internal static void SafeRemove(this ToolStripMenuItem menu)
    {
        menu.UnbindCommand();
        menu.RemoveDropDownItems();
        menu.Dispose();
    }

    internal static void ChildrenSafeRemove(this ToolStripMenuItem menu)
    {
        menu.UnbindCommand();
        menu.RemoveDropDownItems();
    }

    #endregion

    internal static ToolStripItemCollection ReplaceMenu(
        this ToolStripItemCollection collection,
        Func<ToolStripMenuItem> getMenu)
    {
        var menuItem = getMenu();

        if (string.IsNullOrWhiteSpace(menuItem.Name))
        {
            throw new ArgumentException(@"Menu item must have a non-empty name.", nameof(menuItem.Name));
        }

        if (collection[menuItem.Name] is not ToolStripMenuItem item)
        {
            return collection;
        }

        var index = collection.IndexOf(item);
        item.ChildrenSafeRemove();
        collection.Remove(item);
        collection.Insert(index, menuItem);

        return collection;
    }

    internal static ToolStripItemCollection RemoveMenu(
        this ToolStripItemCollection collection,
        MenuItm menuItm)
    {
        if (collection[nameof(menuItm)] is not ToolStripMenuItem item)
        {
            return collection;
        }

        item.SafeRemove();

        return collection;
    }

    internal static ToolStripItemCollection RemoveMenu(
        this ToolStripItemCollection collection)
    {
        for (var index = collection.Count - 1; index >= 0; index--)
        {
            var item = collection[index];

            if (item is not ToolStripMenuItem toolStripItem)
            {
                item.Dispose();
                continue;
            }

            toolStripItem.SafeRemove();
        }

        return collection;
    }

    internal static bool GetCheckedOption(this ToolStripMenuItem menuItem)
    {
        if (menuItem.Tag is not bool isChecked)
        {
            return false;
        }

        var prev = menuItem.Image;
        prev?.Dispose();

        menuItem.CheckMenu(isChecked);

        return isChecked;
    }

    internal static void CheckMenu(this ToolStripItem item, bool isChecked)
    {
        var addShield = item.Name is nameof(MenuItm.Hibernate) or nameof(MenuItm.Sleep);

        var prev = item.Image;
        var next = GetImageIfCheck(isChecked, addShield);
        item.Image = next;
        prev?.Dispose();
    }

    private static Bitmap? GetImageIfCheck(bool isChecked, bool addShield)
    {
        var bitmap = ImageItem.Check.GetImage();

        if (!addShield)
        {
            return isChecked ? bitmap : null;
        }

        var shield = ImageItem.Shield.GetImage();

        return isChecked
            ? bitmap.CopyToSquareCanvas(Color.Transparent, shield)
            : shield;
    }

    #region Private ConditionalWeakTable Methods

    private static void RemoveDropDownItems(this ToolStripMenuItem menu)
    {
        var dropDownItems = menu.DropDownItems;

        if (dropDownItems.Count == 0)
        {
            return;
        }

        for (var index = dropDownItems.Count - 1; index >= 0; index--)
        {
            var item = dropDownItems[index];
            if (item is not ToolStripMenuItem toolStripItem)
            {
                item.Dispose();
                continue;
            }

            var img = toolStripItem.Image;
            if (img is not null)
            {
                toolStripItem.Image = null;
                img.Dispose();
            }

            toolStripItem.Tag = null;
            toolStripItem.Text = null;

            if (toolStripItem.DropDownItems.Count > 0)
            {
                toolStripItem.RemoveDropDownItems();
            }

            menu.SafeRemove(toolStripItem);
        }

        if (dropDownItems.Count > 0)
        {
            dropDownItems.Clear();
        }
    }

    #endregion
}
