using System;
using System.Drawing;
using Common;
using static Common.Image;

namespace PowerScheme.Utility;

internal static class ImageExtensions
{
    public static Icon GetIcon(this ImageItem iconName)
    {
        var bitmap = iconName.GetImage();
        var icon = bitmap.CreateIcon(IconSize.Pixels24X24);

        return icon;
    }

    public static Bitmap GetImage(this ImageItem imageName) =>
        CloneBitmap(GetImageFromResources(imageName));

    private static Bitmap CloneBitmap(Bitmap src) => new(src);

    private static Bitmap GetImageFromResources(ImageItem imageName)
    {
        var resources = Properties.Resources.ResourceManager.GetObject(imageName.ToString());
        var bitmap = resources as Bitmap;

        return bitmap
               ?? throw new AccessViolationException($"{imageName} not found.");
    }
}
