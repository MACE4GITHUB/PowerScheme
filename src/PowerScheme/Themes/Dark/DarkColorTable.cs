using System.Drawing;
using System.Windows.Forms;

namespace PowerScheme.Themes.Dark;

public class DarkColorTable : ProfessionalColorTable, IStyleTheme
{
    public Color BackColor => Color.FromArgb(31, 31, 31);

    public Color ForeColor => Color.White;

    public Font Font => new("Segoe UI", 10f);

    public Color SeparatorBackColor => BackColor;
    public Color SeparatorForeColor => Color.FromArgb(70, 70, 70);

    public Color BorderColor => BackColor;

    public Color SelectedColor => Color.FromArgb(40, 180, 180, 180);
    public Color PressedColor => Color.FromArgb(60, 180, 180, 180);

    public Color ArrowColor => SeparatorForeColor;

    public int ButtonCornerRadius => 5;
    public int ButtonBorderThickness => 1;
    public Color ButtonBorderColor => Color.FromArgb(69, 69, 69);
    public Color ButtonBackColor => Color.FromArgb(55, 55, 55);
    public Color ButtonForeColor => Color.White;
    public Color ButtonMouseOverBackColor => Color.FromArgb(59, 59, 59);
    public Color ButtonMouseDownBackColor => Color.FromArgb(50, 50, 50);

    public override Color ImageMarginGradientBegin => BackColor;
    public override Color ImageMarginGradientMiddle => BackColor;
    public override Color ImageMarginGradientEnd => BackColor;
}
