using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Common;

public static class Image
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static Icon CreateIcon(
        this Bitmap sourceBitmap,
        IconSize iconSize,
        Bitmap? addingBitmap = null)
    {
        if (sourceBitmap == null)
        {
            throw new ArgumentNullException(nameof(sourceBitmap));
        }

        // Create a squared and resized thumbnail as managed Bitmaps and dispose them deterministically.
        using var squareCanvas = sourceBitmap.CopyToSquareCanvas(Color.Transparent, addingBitmap);
        var size = (int)iconSize;
        using var thumbnail = new Bitmap(size, size, PixelFormat.Format32bppArgb);

        using (var g = Graphics.FromImage(thumbnail))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingMode = CompositingMode.SourceOver;
            g.Clear(Color.Transparent);
            g.DrawImage(squareCanvas, 0, 0, size, size);
        }

        // Create an HICON, clone the Icon so the clone owns its handle, then destroy the original HICON.
        var hIcon = thumbnail.GetHicon();
        try
        {
            using var iconFromHandle = Icon.FromHandle(hIcon);
            var clone = (Icon)iconFromHandle.Clone();
            return clone;
        }
        finally
        {
            DestroyIcon(hIcon);
        }
    }

    public static Bitmap CopyToSquareCanvas(
        this Bitmap sourceBitmap,
        Color canvasBackground,
        Bitmap? addingBitmap = null)
    {
        if (sourceBitmap == null)
        {
            throw new ArgumentNullException(nameof(sourceBitmap));
        }

        var maxSide = Math.Max(sourceBitmap.Width, sourceBitmap.Height);

        var bitmapResult = new Bitmap(maxSide, maxSide, PixelFormat.Format32bppArgb);

        using var graphicsResult = Graphics.FromImage(bitmapResult);
        graphicsResult.Clear(canvasBackground);

        var xOffset = (maxSide - sourceBitmap.Width) / 2;
        var yOffset = (maxSide - sourceBitmap.Height) / 2;

        graphicsResult.DrawImage(sourceBitmap, new Point(xOffset, yOffset));

        if (addingBitmap == null)
        {
            return bitmapResult;
        }

        graphicsResult.CompositingMode = CompositingMode.SourceOver;
        var minSide = maxSide / 2;
        graphicsResult.DrawImage(addingBitmap, new Rectangle(minSide, minSide, minSide, minSide));

        return bitmapResult;
    }

    public enum IconSize
    {
        Pixels16X16 = 16,
        Pixels24X24 = 24,
        Pixels32X32 = 32,
        Pixels48X48 = 48,
        Pixels64X64 = 64,
        Pixels96X96 = 96,
        Pixels128X128 = 128
    }
}
