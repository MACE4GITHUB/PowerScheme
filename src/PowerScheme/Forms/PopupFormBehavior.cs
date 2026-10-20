using System;
using System.Windows.Forms;
using PowerScheme.Themes;

namespace PowerScheme.Forms;

internal static class PopupFormBehavior
{
    public static void Attach(Form form, Action refresh, Action hide) =>
        Attach(form, refresh, hide, ThemeService.ApplyControlTheme);

    internal static void Attach(Form form, Action refresh, Action hide, Action<Control> applyTheme)
    {
        form.KeyPreview = true;
        form.Activated += (_, _) =>
        {
            applyTheme(form);
            refresh();
        };
        form.Deactivate += (_, _) => hide();
        form.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                hide();
            }
        };
    }
}
