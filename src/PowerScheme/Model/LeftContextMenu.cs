using System.ComponentModel;
using PowerScheme.Model.Menu;
using PowerScheme.Model.Menu.PowerSchemes;
using PowerScheme.Themes;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

internal sealed class LeftContextMenu(
    IContainer components,
    IPowerSchemeService power) :
    ContextMainMenu(components, power)
{
    private bool _isBuilt;

    internal override void UpdateMenu()
    {
        ClearMenu();

        BuildMenu();
    }

    internal override void ClearMenu()
    {
        if (!_isBuilt || Items.Count == 0)
        {
            return;
        }

        Items.RemoveMenu();
        Items.Clear();

        _isBuilt = false;
    }

    protected override void BuildContextMenu()
    {
        var powerSchemeMenu = new PowerSchemeMenuBuilder(Power).Build();

        while (powerSchemeMenu.DropDownItems.Count > 0)
        {
            var item = powerSchemeMenu.DropDownItems[0];
            Items.Add(item);
        }

        ThemeService.ApplyToolStripTheme(this);

        _isBuilt = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearMenu();
        }

        base.Dispose(disposing);
    }
}
