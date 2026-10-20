using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PowerScheme.Themes;

internal class ToolStripMenuRenderer(ProfessionalColorTable colorTable) :
    ToolStripProfessionalRenderer(colorTable)
{
    private const int RADIUS = 5;

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Image != null)
        {
            e.Graphics.DrawImage(e.Image, e.ImageRectangle);
        }
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        var g = e.Graphics;
        using var path = CreateMenuOutline(e.ToolStrip);
        using var brush = new SolidBrush(((IStyleTheme)ColorTable).BackColor);
        var state = g.Save();
        try
        {
            g.SetClip(path, CombineMode.Intersect);
            g.FillRectangle(brush, e.ToolStrip.ClientRectangle);
        }
        finally
        {
            g.Restore(state);
        }

        e.ToolStrip.Region = new Region(path);
        e.ToolStrip.ForeColor = ((IStyleTheme)ColorTable).ForeColor;
        e.ToolStrip.Font = ((IStyleTheme)ColorTable).Font;
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        var g = e.Graphics;
        using var path = CreateMenuOutline(e.ToolStrip);
        using var pen = new Pen(((IStyleTheme)ColorTable).BorderColor, 2) { Alignment = PenAlignment.Inset };
        var state = g.Save();
        try
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
        finally
        {
            g.Restore(state);
        }
    }

    private static GraphicsPath CreateMenuOutline(ToolStrip menu) =>
        ThemeService.RoundedRect(new RectangleF(-0.5f, -0.5f, menu.Width, menu.Height), RADIUS);

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);

        using var b = new SolidBrush(((IStyleTheme)ColorTable).SeparatorBackColor);
        g.FillRectangle(b, rect);

        var y = rect.Height / 2;

        using var pen = new Pen(((IStyleTheme)ColorTable).SeparatorForeColor, 1);
        g.DrawLine(pen, 0, y, rect.Width, y);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(2, 1, e.Item.Bounds.Width - 4, e.Item.Bounds.Height - 2);

        if (e.Item.Selected && !e.Item.Pressed)
        {
            using var path = ThemeService.RoundedRect(rect, RADIUS);
            using var brush = new SolidBrush(((IStyleTheme)ColorTable).SelectedColor);
            g.FillPath(brush, path);
        }
        else if (e.Item.Pressed)
        {
            using var path = ThemeService.RoundedRect(rect, RADIUS);
            using var brush = new SolidBrush(((IStyleTheme)ColorTable).PressedColor);
            g.FillPath(brush, path);
        }
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        const int ARROW_SIZE = 8;

        var x = e.ArrowRectangle.X + ((e.ArrowRectangle.Width - ARROW_SIZE) / 2);
        var y = e.ArrowRectangle.Y + ((e.ArrowRectangle.Height - ARROW_SIZE) / 2);

        var arrowPoints = new Point[]
        {
            new(x, y),
            new(x, y + ARROW_SIZE),
            new(x + ARROW_SIZE, y + (ARROW_SIZE / 2))
        };

        using var brush = new SolidBrush(((IStyleTheme)ColorTable).ArrowColor);
        e.Graphics.FillPolygon(brush, arrowPoints);
    }
}
