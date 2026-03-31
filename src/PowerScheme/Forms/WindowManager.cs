using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PowerScheme.Forms;

internal static class WindowManager
{
    private static readonly Dictionary<Type, Form> _forms = [];

    public static T ShowForm<T>(bool popup = false, int radius = 0)
        where T : Form, new()
    {
        var type = typeof(T);

        if (!_forms.ContainsKey(type) || _forms[type].IsDisposed)
        {
            var form = new T();

            if (radius > 0)
                WindowStyling.ApplyRoundedCorners(form, radius);

            form.FormClosed += (_, _) => _forms.Remove(type);

            _forms[type] = form;
        }

        var f = _forms[type];

        if (!f.Visible)
        {
            if (popup)
                PositionNearCursor(f);

            f.Show();
        }
        else
        {
            f.BringToFront();
            f.Activate();
        }

        return (T)f;
    }


    private static void PositionNearCursor(Form form)
    {
        var cursor = Cursor.Position;
        var screen = Screen.FromPoint(cursor).WorkingArea;

        var x = cursor.X - 20;
        var y = cursor.Y + 10;

        if (x + form.Width > screen.Right)
            x = screen.Right - form.Width;

        if (y + form.Height > screen.Bottom)
            y = screen.Bottom - form.Height;

        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(x, y);
    }

    public static void HideForm<T>() where T : Form
    {
        var type = typeof(T);

        if (_forms.ContainsKey(type) && !_forms[type].IsDisposed)
        {
            _forms[type].Hide();
        }
    }

    public static bool IsOpen<T>() where T : Form
    {
        var type = typeof(T);
        return _forms.ContainsKey(type) && !_forms[type].IsDisposed && _forms[type].Visible;
    }
}
