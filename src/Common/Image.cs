using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Common;

public static class Image
{
    extension(Bitmap sourceBitmap)
    {
        public Icon CreateIcon(IconSize iconSize, Bitmap? addingBitmap = null)
        {
            var squareCanvas = sourceBitmap.CopyToSquareCanvas(Color.Transparent, addingBitmap);
            squareCanvas = (Bitmap)squareCanvas.GetThumbnailImage((int)iconSize, (int)iconSize, null, new IntPtr());

            var iconResult = Icon.FromHandle(squareCanvas.GetHicon());

            return iconResult;
        }

        public Bitmap CopyToSquareCanvas(Color canvasBackground, Bitmap? addingBitmap = null)
        {
            ArgumentNullException.ThrowIfNull(sourceBitmap);

            var maxSide = sourceBitmap.Width > sourceBitmap.Height ? sourceBitmap.Width : sourceBitmap.Height;

            var bitmapResult = new Bitmap(maxSide, maxSide, PixelFormat.Format32bppArgb);

            using var graphicsResult = Graphics.FromImage(bitmapResult);
            graphicsResult.Clear(canvasBackground);

            var xOffset = (sourceBitmap.Width - maxSide) / 2;

            graphicsResult.DrawImage(sourceBitmap, new Point(xOffset, xOffset));

            if (addingBitmap == null)
            {
                return bitmapResult;
            }

            graphicsResult.CompositingMode = CompositingMode.SourceOver;
            var minSide = maxSide / 2;
            graphicsResult.DrawImage(addingBitmap, new Rectangle(minSide, minSide, minSide, minSide));

            return bitmapResult;
        }
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