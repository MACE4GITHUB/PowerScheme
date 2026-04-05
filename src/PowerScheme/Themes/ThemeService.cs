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

    public static void ApplyControlTheme(Control control)
    {
        control.BackColor = StyleTheme.BackColor;
        control.ForeColor = StyleTheme.ForeColor;
        control.Font = StyleTheme.Font;

        SetControlsTheme(control);
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

    private static void SetControlsTheme(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            switch (c)
            {
                case Label ctl:
                    SetLabelTheme(ctl);
                    break;
                case BaseButton ctl:
                    SetButtonTheme(ctl);
                    break;
                case Panel ctl:
                    SetPanelTheme(ctl);
                    break;
                case NumericUpDown ctl:
                    SetNumericUpDownTheme(ctl);
                    break;
                case ComboBox ctl:
                    SetComboBoxTheme(ctl);
                    break;
            }

            if (c.HasChildren)
            {
                SetControlsTheme(c);
            }
        }
    }

    private static void SetComboBoxTheme(ComboBox ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.BackColor = StyleTheme.BackColor;
        ctl.ForeColor = StyleTheme.ForeColor;
    }

    private static void SetNumericUpDownTheme(NumericUpDown ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.BackColor = StyleTheme.BackColor;
        ctl.ForeColor = StyleTheme.ForeColor;
    }

    private static void SetPanelTheme(Panel ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.BackColor = StyleTheme.BackColor;
        ctl.ForeColor = StyleTheme.ForeColor;
    }

    public static void SetButtonTheme(BaseButton ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.CornerRadius = StyleTheme.ButtonCornerRadius;
        ctl.BorderThickness = StyleTheme.ButtonBorderThickness;
        ctl.BackColor = StyleTheme.ButtonBackColor;
        ctl.ForeColor = StyleTheme.ButtonForeColor;
        ctl.FlatAppearance.BorderColor = StyleTheme.ButtonBorderColor;
        ctl.FlatAppearance.MouseOverBackColor = StyleTheme.ButtonMouseOverBackColor;
        ctl.FlatAppearance.MouseDownBackColor = StyleTheme.ButtonMouseDownBackColor;
    }

    public static void SetAttentionButtonTheme(BaseButton ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.CornerRadius = StyleTheme.ButtonCornerRadius;
        ctl.BorderThickness = StyleTheme.ButtonBorderThickness;
        ctl.BackColor = StyleTheme.ButtonBackColor;
        ctl.ForeColor = StyleTheme.ButtonAttentionForeColor;
        ctl.FlatAppearance.BorderColor = StyleTheme.ButtonBorderColor;
        ctl.FlatAppearance.MouseOverBackColor = StyleTheme.ButtonMouseOverBackColor;
        ctl.FlatAppearance.MouseDownBackColor = StyleTheme.ButtonMouseDownBackColor;
    }

    private static void SetLabelTheme(Label ctl)
    {
        ctl.Font = StyleTheme.Font;
        ctl.ForeColor = StyleTheme.ForeColor;
    }
}
