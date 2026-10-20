using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Controls;
using PowerScheme.Themes;

namespace PowerScheme.Tests;

[STATestClass]
public sealed class ThemeServiceTests
{
    [TestMethod]
    [DataRow((int)ThemeKind.Default, false)]
    [DataRow((int)ThemeKind.Default, true)]
    [DataRow((int)ThemeKind.Dark, false)]
    [DataRow((int)ThemeKind.Dark, true)]
    [DataRow((int)ThemeKind.Light, false)]
    [DataRow((int)ThemeKind.Light, true)]
    [DataRow((int)ThemeKind.Blue, false)]
    [DataRow((int)ThemeKind.Blue, true)]
    [DataRow((int)ThemeKind.Green, false)]
    [DataRow((int)ThemeKind.Green, true)]
    public void SetButtonTheme_AppliesAllExistingStyleProperties(int kind, bool attention)
    {
        var style = (IStyleTheme)((ThemeKind)kind).GetTheme();
        using var button = new BaseButton();

        ThemeService.SetButtonTheme(button, style, attention);

        AssertButtonStyle(button, style, attention);
    }

    [TestMethod]
    [DataRow((int)ThemeKind.Default)]
    [DataRow((int)ThemeKind.Dark)]
    [DataRow((int)ThemeKind.Light)]
    [DataRow((int)ThemeKind.Blue)]
    [DataRow((int)ThemeKind.Green)]
    public void ApplyControlTheme_ThemesNestedSupportedControlsAndPreservesOtherBackgrounds(int kind)
    {
        var style = (IStyleTheme)((ThemeKind)kind).GetTheme();
        using var root = new UserControl();
        using var panel = new Panel();
        using var numeric = new NumericUpDown();
        using var comboBox = new ComboBox();
        using var label = new Label { BackColor = Color.Magenta };
        using var button = new BaseButton();
        using var other = new TextBox { BackColor = Color.Plum, ForeColor = Color.Orange };
        using var nested = new UserControl { BackColor = Color.Lime };
        using var nestedLabel = new Label { BackColor = Color.Yellow };
        root.Controls.Add(panel);
        panel.Controls.AddRange([numeric, comboBox, label, button, other, nested]);
        nested.Controls.Add(nestedLabel);
        ThemeService.SetButtonTheme(button, style, true);

        ThemeService.ApplyControlTheme(root, style);

        foreach (var control in new Control[] { root, panel, numeric, comboBox })
        {
            Assert.AreEqual(style.Font, control.Font);
            Assert.AreEqual(style.BackColor, control.BackColor);
            Assert.AreEqual(style.ForeColor, control.ForeColor);
        }

        Assert.AreEqual(style.Font, label.Font);
        Assert.AreEqual(style.ForeColor, label.ForeColor);
        Assert.AreEqual(Color.Magenta, label.BackColor);
        Assert.AreEqual(style.Font, nestedLabel.Font);
        Assert.AreEqual(style.ForeColor, nestedLabel.ForeColor);
        Assert.AreEqual(Color.Yellow, nestedLabel.BackColor);
        Assert.AreEqual(Color.Lime, nested.BackColor);
        Assert.AreEqual(Color.Plum, other.BackColor);
        Assert.AreEqual(Color.Orange, other.ForeColor);
        AssertButtonStyle(button, style, false);
        Assert.IsFalse(root.IsHandleCreated);
    }

    [TestMethod]
    [DataRow((int)ThemeKind.Dark, (int)ThemeKind.Light)]
    [DataRow((int)ThemeKind.Green, (int)ThemeKind.Blue)]
    public void Reapplication_UsesNewThemeAndResetsAttentionAppearance(int firstKind, int nextKind)
    {
        var first = (IStyleTheme)((ThemeKind)firstKind).GetTheme();
        var next = (IStyleTheme)((ThemeKind)nextKind).GetTheme();
        using var root = new UserControl();
        using var button = new BaseButton();
        root.Controls.Add(button);
        ThemeService.ApplyControlTheme(root, first);
        ThemeService.SetButtonTheme(button, first, true);
        AssertButtonStyle(button, first, true);

        ThemeService.ApplyControlTheme(root, next);

        Assert.AreEqual(next.BackColor, root.BackColor);
        Assert.AreEqual(next.ForeColor, root.ForeColor);
        AssertButtonStyle(button, next, false);

        ThemeService.SetButtonTheme(button, next, true);
        AssertButtonStyle(button, next, true);
    }

    private static void AssertButtonStyle(BaseButton button, IStyleTheme style, bool attention)
    {
        Assert.AreEqual(style.Font, button.Font);
        Assert.AreEqual(style.ButtonCornerRadius, button.CornerRadius);
        Assert.AreEqual(style.ButtonBorderThickness, button.BorderThickness);
        Assert.AreEqual(style.ButtonBackColor, button.BackColor);
        Assert.AreEqual(attention ? style.ButtonAttentionForeColor : style.ButtonForeColor, button.ForeColor);
        Assert.AreEqual(style.ButtonBorderColor, button.FlatAppearance.BorderColor);
        Assert.AreEqual(style.ButtonMouseOverBackColor, button.FlatAppearance.MouseOverBackColor);
        Assert.AreEqual(style.ButtonMouseDownBackColor, button.FlatAppearance.MouseDownBackColor);
    }
}