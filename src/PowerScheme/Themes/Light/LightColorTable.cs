using System.Drawing;
using System.Windows.Forms;

namespace PowerScheme.Themes.Light;

public class LightColorTable : ProfessionalColorTable, IStyleTheme
{
    public Color BackColor => Color.FromArgb(250, 250, 250);

    public Color ForeColor => Color.Black;

    public Font Font => new("Segoe UI", 10f);

    public Color SeparatorBackColor => BackColor;
    public Color SeparatorForeColor => Color.FromArgb(230, 230, 230);

    public Color BorderColor => Color.FromArgb(230, 230, 230);

    public Color SelectedColor => Color.FromArgb(20, 0, 120, 215);
    public Color PressedColor => Color.FromArgb(30, 0, 120, 215);

    public Color ArrowColor => SeparatorForeColor;

    public Color ButtonBackColor => Color.FromArgb(139, 147, 154);
    public Color ButtonForeColor => Color.White;
    public Color ButtonMouseOverBackColor => Color.FromArgb(158, 165, 171);
    public Color ButtonMouseDownBackColor => Color.FromArgb(176, 183, 187);

    public override Color ImageMarginGradientBegin => BackColor;
    public override Color ImageMarginGradientMiddle => BackColor;
    public override Color ImageMarginGradientEnd => BackColor;
}
