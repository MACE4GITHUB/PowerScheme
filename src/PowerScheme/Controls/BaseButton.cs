using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PowerScheme.Themes;

namespace PowerScheme.Controls;

public class BaseButton : Button
{
    public int CornerRadius { get; set; } = 8;

    public int BorderThickness { get; set; } = 1;

    private bool _hover;
    private bool _pressed;

    private bool IsDesignMode =>
        DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    public BaseButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRegion();
    }

    private void UpdateRegion()
    {
        if (Width <= 0 || Height <= 0)
            return;

        using var path = ThemeService.RoundedRect(ClientRectangle, CornerRadius);
        Region = new Region(path);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!IsDesignMode)
        {
            _hover = true;
            Invalidate();
        }
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (!IsDesignMode)
        {
            _hover = false;
            _pressed = false;
            Invalidate();
        }
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!IsDesignMode)
        {
            _pressed = true;
            Invalidate();
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (!IsDesignMode)
        {
            _pressed = false;
            Invalidate();
        }
        base.OnMouseUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var dpi = e.Graphics.DpiX / 96f;
        var radius = (int)(CornerRadius * dpi);
        var border = (int)(BorderThickness * dpi);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;
        rect.Inflate(-1, -1);

        var back =
            _pressed ? FlatAppearance.MouseDownBackColor :
            _hover ? FlatAppearance.MouseOverBackColor :
            BackColor;

        using var path = ThemeService.RoundedRect(rect, radius);
        using var brush = new SolidBrush(back);
        using var pen = new Pen(FlatAppearance.BorderColor, border);
        e.Graphics.FillPath(brush, path);
        e.Graphics.DrawPath(pen, path);

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            rect,
            ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
        );
    }
}
