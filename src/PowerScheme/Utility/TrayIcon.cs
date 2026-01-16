using Common;
using System.Drawing;
using static Common.Image;

namespace PowerScheme.Utility;

internal static class TrayIcon
{
    public static Icon GetIcon(ImageItem iconName)
    {
        var bitmap = GetImage(iconName);
        var icon = bitmap.CreateIcon(IconSize.Pixels24X24);
        return icon;
    }

    public static Bitmap GetImage(ImageItem imageName)
    {
        var resources = Properties.Resources.ResourceManager.GetObject(imageName.ToString());
        var bitmap = resources as Bitmap;
        return bitmap;
    }
}
