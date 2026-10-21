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
    internal override void UpdateMenu()
    {
        BuildMenu();
    }

    internal override void ClearMenu()
    {
        if (Items.Count == 0)
        {
            return;
        }

        Items.RemoveMenu();
        Items.Clear();
    }

    protected override void BuildContextMenu()
    {
        try
        {
            PowerSchemeMenuBuilder.Update(Items, Power);
            ThemeService.ApplyToolStripTheme(this);
        }
        catch
        {
            ClearMenu();
            throw;
        }
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
