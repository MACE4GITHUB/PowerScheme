using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Model;

internal sealed class LeftContextMenu(
    IContainer components,
    IPowerSchemeService power) :
    ContextMainMenu(components, power)
{
    private bool _isBuilt;

    public override void UpdateMenu()
    {
        BuildMenu();
    }

    public override void ClearMenu()
    {
        if (!_isBuilt || Items.Count == 0)
        {
            return;
        }

        for (var index = Items.Count - 1; index >= 0; index--)
        {
            var toolStripItem = Items[index];

            toolStripItem.Click -= ItemMenuPowerOnClick;

            // Dispose image assigned to the item to release GDI+/native resources.
            // Remove reference from the ToolStripItem first to avoid holding a disposed Image.
            var img = toolStripItem.Image;
            if (img is not null)
            {
                toolStripItem.Image = null;
                img.Dispose();
            }

            toolStripItem.Tag = null;
            toolStripItem.Text = null;

            toolStripItem.Dispose();
        }

        Items.Clear();

        _isBuilt = false;
    }

    protected override void BuildContextMenu()
    {
        ClearMenu();

        if (Power == null)
        {
            return;
        }

        foreach (var powerScheme in Power.DefaultPowerSchemes)
        {
            var src = GetImage(powerScheme.Picture);
            var item = new ToolStripMenuItem
            {
                Tag = new StatePowerScheme(powerScheme),
                Text = powerScheme.Name,
                // Clone the source bitmap so each ToolStripItem owns its Image instance.
                // That allows safe disposal in ClearMenu without affecting the shared resource.
                Image = src is null
                    ? null
                    : new Bitmap(src)
            };

            src?.Dispose();

            item.Click += ItemMenuPowerOnClick;

            Items.Add(item);
        }

        if (!Power.UserPowerSchemes.Any())
        {
            _isBuilt = true;

            return;
        }

        Items.Add(new ToolStripSeparator());

        foreach (var powerScheme in Power.UserPowerSchemes)
        {
            var src = GetImage(powerScheme.Picture);
            var item = new ToolStripMenuItem
            {
                Tag = new StatePowerScheme(powerScheme),
                Text = powerScheme.Name,
                Image = src is null
                    ? null
                    : new Bitmap(src)
            };

            src?.Dispose();

            item.Click += ItemMenuPowerOnClick;

            Items.Add(item);
        }

        _isBuilt = true;
    }

    private void ItemMenuPowerOnClick(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem menu)
        {
            return;
        }

        if (menu.Tag is not StatePowerScheme statePowerScheme)
        {
            return;
        }

        Power?.SetActivePowerScheme(statePowerScheme.PowerScheme);

        ClearMenu();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearMenu();
            Power = null;
        }

        base.Dispose(disposing);
    }
}
