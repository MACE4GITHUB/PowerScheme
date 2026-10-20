using System.Drawing;
using System.Windows.Forms;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Controls;
using PowerScheme.Themes;
using PowerScheme.Utility;

namespace PowerScheme.Tests;

[STATestClass]
public sealed class ThemeServiceTests
{
    [TestMethod]
    [DataRow(120, 70)]
    [DataRow(240, 160)]
    [DataRow(90, 28)]
    public void MenuCorners_BackgroundAndBorderAreSymmetricAtDifferentSizes(int width, int height)
    {
        var renderer = new ToolStripMenuRenderer(ThemeKind.Dark.GetTheme());
        using var menu = new ContextMenuStrip { AutoSize = false, Size = new Size(width, height) };
        using var bitmap = new Bitmap(menu.Width, menu.Height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(graphics, menu));
            Assert.IsNotNull(menu.Region);
            graphics.SetClip(menu.Region, System.Drawing.Drawing2D.CombineMode.Intersect);
            renderer.DrawToolStripBorder(new ToolStripRenderEventArgs(graphics, menu));
        }

        Assert.AreEqual(0, bitmap.GetPixel(0, 0).A);
        Assert.AreEqual(255, bitmap.GetPixel(7, 7).A);
        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
            var topLeft = bitmap.GetPixel(column, row).A;
            Assert.AreEqual(topLeft, bitmap.GetPixel(bitmap.Width - 1 - column, row).A,
                    $"Top corners differ at ({column}, {row}) for {width}x{height}.");
                Assert.AreEqual(topLeft, bitmap.GetPixel(column, bitmap.Height - 1 - row).A,
                    $"Left corners differ at ({column}, {row}) for {width}x{height}.");
                Assert.AreEqual(topLeft, bitmap.GetPixel(bitmap.Width - 1 - column, bitmap.Height - 1 - row).A,
                    $"Opposite corners differ at ({column}, {row}) for {width}x{height}.");
            }
        }
    }

    [TestMethod]
    public void ApplyToolStripTheme_UsesCustomRendererForNestedLanguageMenu()
    {
        using var menu = new ContextMenuStrip();
        using var settings = new ToolStripMenuItem("Settings");
        using var languages = new ToolStripMenuItem("Language");
        using var selected = new ToolStripMenuItem("Selected") { Checked = true, Image = ImageItem.Check.GetImage() };
        menu.Items.Add(settings);
        settings.DropDownItems.Add(languages);
        languages.DropDownItems.Add(selected);

        ThemeService.ApplyToolStripTheme(menu);

        Assert.IsInstanceOfType<ToolStripMenuRenderer>(languages.DropDown.Renderer);
        Assert.AreSame(menu.Renderer, languages.DropDown.Renderer);
    }

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
    public void RenderCheckedItemImage_DrawsOnlyTransparentGlyphWithoutBackgroundOrBorder(int kind, bool selected)
    {
        var theme = ((ThemeKind)kind).GetTheme();
        var style = (IStyleTheme)theme;
        var renderer = new ToolStripMenuRenderer(theme);
        var image = ImageItem.Check.GetImage();
        var rectangle = new Rectangle(6, 6, image.Width, image.Height);
        using var actual = new Bitmap(image.Width + 12, image.Height + 12);
        using var expected = new Bitmap(actual.Width, actual.Height);
        using var menu = new ToolStripDropDownMenu();
        using var item = new ToolStripMenuItem { Checked = true, Image = image };
        menu.Items.Add(item);
        if (selected)
        {
            item.Select();
        }
        var background = selected ? style.SelectedColor : style.BackColor;
        using (var graphics = Graphics.FromImage(expected))
        {
            graphics.Clear(background);
            graphics.DrawImage(image, rectangle);
        }
        using (var graphics = Graphics.FromImage(actual))
        {
            graphics.Clear(background);
            renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(graphics, item, image, rectangle));
        }

        for (var row = 0; row < actual.Height; row++)
        {
            for (var column = 0; column < actual.Width; column++)
            {
                Assert.AreEqual(expected.GetPixel(column, row).ToArgb(), actual.GetPixel(column, row).ToArgb(),
                    $"Unexpected check background or border at ({column}, {row}).");
            }
        }
    }

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