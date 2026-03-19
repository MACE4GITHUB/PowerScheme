using System.Drawing;
using System.Windows.Forms;
using PowerScheme.Model;
using RegistryManager;

namespace PowerScheme.Themes;

internal static class ThemeService
{
    private static ThemeKind ThemeKind => (ThemeKind)RegistryService.GetTheme(AppInfo.CompanyName, AppInfo.ProductName);

    public static void ApplyToolStripTheme(ToolStrip toolStrip)
    {
        var theme = ThemeKind.GetTheme();

        toolStrip.Renderer = new ToolStripMenuRenderer(theme);
    }

    public static void ApplyControlTheme(Control control)
    {
        var theme = ThemeKind.GetTheme();
        var styleTheme = (IStyleTheme)theme;

        control.BackColor = styleTheme.BackColor;
        control.ForeColor = styleTheme.ForeColor;
        control.Font = styleTheme.Font;

        SetLabelsColor(control, styleTheme.ForeColor);
        SetButtonsColor(control, styleTheme);
    }

    private static void SetLabelsColor(Control parent, Color color)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is Label lbl)
            {
                lbl.ForeColor = color;
            }

            if (c.HasChildren)
            {
                SetLabelsColor(c, color);
            }
        }
    }

    private static void SetButtonsColor(Control parent, IStyleTheme styleTheme)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is Button btn)
            {
                btn.BackColor = styleTheme.ButtonBackColor;
                btn.ForeColor = styleTheme.ButtonForeColor;
                btn.FlatAppearance.MouseOverBackColor = styleTheme.ButtonMouseOverBackColor;
                btn.FlatAppearance.MouseDownBackColor = styleTheme.ButtonMouseDownBackColor;
            }

            if (c.HasChildren)
            {
                SetButtonsColor(c, styleTheme);
            }
        }
    }
}
