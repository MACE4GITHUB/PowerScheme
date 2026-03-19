using System.Drawing;
using System.Windows.Forms;

namespace PowerScheme.Themes.Blue;

public class BlueColorTable : ProfessionalColorTable, IStyleTheme
{
    public Color BackColor => Color.FromArgb(53, 54, 132);

    public Color ForeColor => Color.White;

    public Font Font => new("Segoe UI", 10f);

    public Color SeparatorBackColor => BackColor;
    public Color SeparatorForeColor => Color.FromArgb(63, 65, 154);

    public Color BorderColor => SeparatorForeColor;

    public Color SelectedColor => Color.FromArgb(40, 0, 120, 215);
    public Color PressedColor => Color.FromArgb(60, 0, 120, 215);

    public Color ArrowColor => SeparatorForeColor;

    public Color ButtonBackColor => Color.FromArgb(80, 82, 182);
    public Color ButtonForeColor => Color.White;
    public Color ButtonMouseOverBackColor => Color.FromArgb(99, 101, 190);
    public Color ButtonMouseDownBackColor => Color.FromArgb(114, 116, 197);

    public override Color ImageMarginGradientBegin => BackColor;
    public override Color ImageMarginGradientMiddle => BackColor;
    public override Color ImageMarginGradientEnd => BackColor;
}
