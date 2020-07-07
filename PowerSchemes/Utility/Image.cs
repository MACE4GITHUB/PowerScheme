using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PowerSchemes.Utility
{
    public static class Image
    {
        public static Icon CreateIcon(this Bitmap sourceBitmap, IconSize iconSize)
        {
            var squareCanvas = sourceBitmap.CopyToSquareCanvas(Color.Transparent);
            squareCanvas = (Bitmap)squareCanvas.GetThumbnailImage((int)iconSize, (int)iconSize, null, new IntPtr());

            var iconResult = Icon.FromHandle(squareCanvas.GetHicon());

            return iconResult;
        }

        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, Color canvasBackground)
        {
            if (sourceBitmap == null) throw new ArgumentNullException(nameof(sourceBitmap));

            var maxSide = sourceBitmap.Width > sourceBitmap.Height ? sourceBitmap.Width : sourceBitmap.Height;

            var bitmapResult = new Bitmap(maxSide, maxSide, PixelFormat.Format32bppArgb);

            using (var graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.Clear(canvasBackground);

                var xOffset = (sourceBitmap.Width - maxSide) / 2;

                graphicsResult.DrawImage(sourceBitmap, new Point(xOffset, xOffset));
            }

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
}
