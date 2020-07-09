namespace PowerScheme.Utility
{
    using System.Drawing;
    using static Image;

    public static class TrayIcon
    {
        public static Icon GetIcon(string iconName)
        {
            var bitmap = GetImage(iconName);
            var icon = bitmap.CreateIcon(IconSize.Pixels16X16);
            return icon;
        }

        public static Bitmap GetImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) imageName = "Unknown";
            var resources = Properties.Resources.ResourceManager.GetObject(imageName);
            var bitmap = resources as Bitmap;
            return bitmap;
        }
    }
}
