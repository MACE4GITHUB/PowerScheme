using System;
using System.Collections.Generic;
using System.Drawing;
using Common;
using static Common.Image;

namespace PowerScheme.Utility;

internal static class ImageExtensions
{
    private static readonly object MenuImageLock = new();
    private static readonly Dictionary<ImageItem, Bitmap> MenuImages = new();
    private static readonly HashSet<System.Drawing.Image> SharedMenuImages = new();
    private static Bitmap? _checkedShield;

    internal static Bitmap GetMenuImage(this ImageItem imageName)
    {
        lock (MenuImageLock)
        {
            if (!MenuImages.TryGetValue(imageName, out var image))
            {
                image = CloneBitmap(GetImageFromResources(imageName));
                MenuImages.Add(imageName, image);
                SharedMenuImages.Add(image);
            }

            return image;
        }
    }

    internal static Bitmap? GetMenuCheckImage(bool isChecked, bool addShield)
    {
        if (!isChecked)
        {
            return addShield ? ImageItem.Shield.GetMenuImage() : null;
        }

        if (!addShield)
        {
            return ImageItem.Check.GetMenuImage();
        }

        lock (MenuImageLock)
        {
            if (_checkedShield == null)
            {
                _checkedShield = ImageItem.Check.GetMenuImage().CopyToSquareCanvas(
                    Color.Transparent, ImageItem.Shield.GetMenuImage());
                SharedMenuImages.Add(_checkedShield);
            }

            return _checkedShield;
        }
    }

    internal static void DisposeOwnedImage(System.Drawing.Image? image)
    {
        lock (MenuImageLock)
        {
            if (image == null || SharedMenuImages.Contains(image))
            {
                return;
            }
        }

        image.Dispose();
    }

    public static Icon GetIcon(this ImageItem iconName)
    {
        using var bitmap = iconName.GetImage();
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
