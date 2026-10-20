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

    private static ProfessionalColorTable Theme => ThemeKind.GetTheme();

    private static IStyleTheme StyleTheme => (IStyleTheme)Theme;

    public static void ApplyToolStripTheme(ToolStrip toolStrip) =>
        toolStrip.Renderer = new ToolStripMenuRenderer(Theme);

    public static void ApplyControlTheme(Control control) =>
        ApplyControlTheme(control, StyleTheme);

    internal static void ApplyControlTheme(Control control, IStyleTheme style)
    {
        control.BackColor = style.BackColor;
        control.ForeColor = style.ForeColor;
        control.Font = style.Font;

        SetControlsTheme(control, style);
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

    private static void SetControlsTheme(Control parent, IStyleTheme style)
    {
        foreach (Control c in parent.Controls)
        {
            switch (c)
            {
                case Label ctl:
                    SetLabelTheme(ctl, style);
                    break;
                case BaseButton ctl:
                    SetButtonTheme(ctl, style);
                    break;
                case Panel:
                case NumericUpDown:
                case ComboBox:
                    SetControlTheme(c, style);
                    break;
            }

            if (c.HasChildren)
            {
                SetControlsTheme(c, style);
            }
        }
    }

    private static void SetControlTheme(Control ctl, IStyleTheme style)
    {
        ctl.Font = style.Font;
        ctl.BackColor = style.BackColor;
        ctl.ForeColor = style.ForeColor;
    }

    public static void SetButtonTheme(BaseButton ctl) =>
        SetButtonTheme(ctl, StyleTheme);

    public static void SetAttentionButtonTheme(BaseButton ctl) =>
        SetButtonTheme(ctl, StyleTheme, true);

    internal static void SetButtonTheme(BaseButton ctl, IStyleTheme style, bool attention = false)
    {
        ctl.Font = style.Font;
        ctl.CornerRadius = style.ButtonCornerRadius;
        ctl.BorderThickness = style.ButtonBorderThickness;
        ctl.BackColor = style.ButtonBackColor;
        ctl.ForeColor = attention ? style.ButtonAttentionForeColor : style.ButtonForeColor;
        ctl.FlatAppearance.BorderColor = style.ButtonBorderColor;
        ctl.FlatAppearance.MouseOverBackColor = style.ButtonMouseOverBackColor;
        ctl.FlatAppearance.MouseDownBackColor = style.ButtonMouseDownBackColor;
    }

    private static void SetLabelTheme(Label ctl, IStyleTheme style)
    {
        ctl.Font = style.Font;
        ctl.ForeColor = style.ForeColor;
    }
}
