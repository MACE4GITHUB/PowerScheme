using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PowerScheme.Controls;
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

        SetControlsTheme(control, styleTheme);
    }

    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        var d = radius * 2;
        var path = new GraphicsPath();

        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();

        return path;
    }

    private static void SetControlsTheme(Control parent, IStyleTheme styleTheme)
    {
        foreach (Control c in parent.Controls)
        {
            switch (c)
            {
                case Label ctl:
                    ctl.Font = styleTheme.Font;
                    ctl.ForeColor = styleTheme.ForeColor;
                    break;
                case BaseButton ctl:
                    ctl.Font = styleTheme.Font;
                    ctl.CornerRadius = styleTheme.ButtonCornerRadius;
                    ctl.BorderThickness = styleTheme.ButtonBorderThickness;
                    ctl.BackColor = styleTheme.ButtonBackColor;
                    ctl.ForeColor = styleTheme.ButtonForeColor;
                    ctl.FlatAppearance.BorderColor = styleTheme.ButtonBorderColor;
                    ctl.FlatAppearance.MouseOverBackColor = styleTheme.ButtonMouseOverBackColor;
                    ctl.FlatAppearance.MouseDownBackColor = styleTheme.ButtonMouseDownBackColor;
                    break;
                case Panel ctl:
                    ctl.BackColor = styleTheme.BackColor;
                    break;
            }

            if (c.HasChildren)
            {
                SetControlsTheme(c, styleTheme);
            }
        }
    }
}
