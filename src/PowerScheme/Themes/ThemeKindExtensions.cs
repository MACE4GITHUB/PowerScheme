using System;
using System.Windows.Forms;
using PowerScheme.Themes.Blue;
using PowerScheme.Themes.Dark;
using PowerScheme.Themes.Green;
using PowerScheme.Themes.Light;

namespace PowerScheme.Themes;

internal static class ThemeKindExtensions
{
    public static ProfessionalColorTable GetTheme(this ThemeKind themeKind) =>
        themeKind switch
        {
            ThemeKind.Default => new LightColorTable(),
            ThemeKind.Dark => new DarkColorTable(),
            ThemeKind.Light => new LightColorTable(),
            ThemeKind.Blue => new BlueColorTable(),
            ThemeKind.Green => new GreenColorTable(),
            _ => throw new ArgumentOutOfRangeException(nameof(themeKind), themeKind, null)
        };
}
