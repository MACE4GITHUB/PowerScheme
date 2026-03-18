using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerScheme.Forms;

internal static class WindowStyling
{
    [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse);

    public static void ApplyRoundedCorners(Form form, int radius)
    {
        if (radius <= 0)
        {
            form.Region = null;
            return;
        }

        form.Region = Region.FromHrgn(CreateRoundRectRgn(
            0, 0, form.Width, form.Height, radius, radius));
    }
}
