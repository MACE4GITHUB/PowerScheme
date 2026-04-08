using System.Drawing;

namespace PowerScheme.Themes;

internal interface IStyleTheme
{
    Color BackColor { get; }
    Color ForeColor { get; }

    Color SeparatorBackColor { get; }
    Color SeparatorForeColor { get; }

    Color BorderColor { get; }

    Color SelectedColor { get; }
    Color PressedColor { get; }

    Color ArrowColor { get; }

    Font Font { get; }

    int ButtonCornerRadius { get; }
    int ButtonBorderThickness { get; }
    Color ButtonBackColor { get; }
    Color ButtonBorderColor { get; }
    Color ButtonForeColor { get; }
    Color ButtonAttentionForeColor { get; }
    Color ButtonMouseOverBackColor { get; }
    Color ButtonMouseDownBackColor { get; }
}
