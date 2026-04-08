using System.Drawing;
using System.Windows.Forms;

namespace PowerScheme.Themes.Green;

public class GreenColorTable : ProfessionalColorTable, IStyleTheme
{
    public Color BackColor => Color.FromArgb(19, 111, 39);

    public Color ForeColor => Color.White;

    public Font Font => new("Segoe UI", 10f);

    public Color SeparatorBackColor => BackColor;
    public Color SeparatorForeColor => Color.FromArgb(16, 97, 34);

    public Color BorderColor => SeparatorForeColor;

    public Color SelectedColor => Color.FromArgb(40, 30, 172, 62);
    public Color PressedColor => Color.FromArgb(60, 30, 172, 62);

    public Color ArrowColor => SeparatorForeColor;

    public int ButtonCornerRadius => 5;
    public int ButtonBorderThickness => 1;
    public Color ButtonBorderColor => ButtonBackColor;
    public Color ButtonBackColor => Color.FromArgb(26, 149, 53);
    public Color ButtonForeColor => Color.White;
    public Color ButtonAttentionForeColor => Color.FromArgb(255, 200, 101);
    public Color ButtonMouseOverBackColor => Color.FromArgb(29, 163, 58);
    public Color ButtonMouseDownBackColor => Color.FromArgb(31, 173, 63);

    public override Color ImageMarginGradientBegin => BackColor;
    public override Color ImageMarginGradientMiddle => BackColor;
    public override Color ImageMarginGradientEnd => BackColor;
}
