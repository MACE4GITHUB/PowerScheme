using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common;
using Languages;
using PowerScheme.Themes;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Settings;

internal class SettingsMenuBuilder :
    BaseMenuBuilder
{
    private readonly IPowerSchemeService _power;

    private const MenuItm MENU_ITM = MenuItm.Parameters;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal SettingsMenuBuilder(IPowerSchemeService power, ToolStripMenuItem? root = null) : base(root) =>
        _power = power ?? throw new ArgumentNullException(nameof(power));

    internal ToolStripMenuItem Build()
    {
        Root.Image = null;
        Root.ToolTipText = string.Empty;
        AddControlPanelScheme();
        AddRestoreDefault();
        AddThemes();
        AddLanguages();
        Menu.Separator();

        var withoutDeletedCount = _power.TypicalPowerSchemesWithoutDeleted.Count();
        var withDeletedCount = _power.TypicalPowerSchemesWithDeleted.Count();
        var onlyDeletedCount = withDeletedCount - withoutDeletedCount;

        if (onlyDeletedCount >= 0 && withoutDeletedCount != 0)
        {
            AddDeleteTypicalSchemes();
        }

        if (onlyDeletedCount > 0)
        {
            AddTypicalSchemes();
        }

        Menu.Separator();

        AddDynamicPowerSchemes();
        AddCustomPowerSchemes();

        return Complete();
    }

    private void AddControlPanelScheme()
    {
        var item = Menu.Item(
            MenuItm.ControlPanelSchemeWindows);

        item.BindCommandOnce(static () => new OpenPowerSchemesPanelCommand());
    }

    private void AddRestoreDefault()
    {
        var item = Menu.Item(
            MenuItm.RestoreDefaultPowerSchemes);

        item.BindCommandOnce(_power, static service => new RestoreDefaultPowerSchemesCommand(service));
    }

    private void AddThemes()
    {
        var item = Menu.Item(MenuItm.Themes);
        var themes = new MenuItemUpdater(item.DropDownItems);

        var itemDark = themes.Item(MenuItm.Dark);
        itemDark.Tag = ThemeKind.Dark;
        itemDark.BindCommandOnce(static () => new ChangeThemeCommand());

        var itemLight = themes.Item(MenuItm.Light);
        itemLight.Tag = ThemeKind.Light;
        itemLight.BindCommandOnce(static () => new ChangeThemeCommand());

        var itemBlue = themes.Item(MenuItm.Blue);
        itemBlue.Tag = ThemeKind.Blue;
        itemBlue.BindCommandOnce(static () => new ChangeThemeCommand());

        var itemGreen = themes.Item(MenuItm.Green);
        itemGreen.Tag = ThemeKind.Green;
        itemGreen.BindCommandOnce(static () => new ChangeThemeCommand());

        themes.Complete();
    }

    private void AddLanguages()
        => CreateLanguageMenu(item: Menu.Item(MenuItm.Language));

    internal ToolStripMenuItem CreateLanguageMenu(
        Func<IReadOnlyList<LanguageDefinition>>? discover = null,
        ToolStripMenuItem? item = null)
    {
        item ??= MenuItemFactory.Create(MenuItm.Language);
        discover ??= () => Language.GetAvailableLanguages(reportError: message => ErrorLog.Write(message));

        if (item.Tag is not LanguageMenuState state)
        {
            state = new LanguageMenuState(_power, discover);
            item.Tag = state;
            item.DropDownOpening += state.Refresh;
            state.Refresh(item, EventArgs.Empty);
        }
        return item;
    }

    private sealed class LanguageMenuState(IPowerSchemeService power, Func<IReadOnlyList<LanguageDefinition>> discover)
    {
        internal void Refresh(object? sender, EventArgs args)
        {
            if (sender is not ToolStripMenuItem item)
            {
                return;
            }
            var update = new MenuItemUpdater(item.DropDownItems);

            foreach (var language in discover())
            {
                var child = update.Item($"Language_{language.Code}", language.Name,
                    GetLanguageCheckImage(string.Equals(Language.Code, language.Code, StringComparison.OrdinalIgnoreCase)));
                child.Tag = language;
                child.Checked = false;
                child.BindCommandOnce(power, static service => new ChangeLanguageCommand(service));
            }
            update.Complete();
        }
    }

    private static Bitmap GetLanguageCheckImage(bool isChecked) =>
        isChecked
            ? ImageItem.Check.GetMenuImage()
            : ImageItem.Empty.GetMenuImage();

    private void AddTypicalSchemes()
    {
        var item = Menu.Item(
            MenuItm.CreateTypicalSchemes);

        item.BindCommandOnce(_power, static service => new CreateTypicalPowerSchemesCommand(service));
    }

    private void AddDeleteTypicalSchemes()
    {
        var item = Menu.Item(MenuItm.DeleteTypicalSchemes);

        item.BindCommandOnce(_power, static service => new DeleteTypicalPowerSchemesCommand(service));
    }

    private void AddDynamicPowerSchemes()
    {
        foreach (var scheme in _power.TypicalPowerSchemesWithDeleted)
        {
            var menuItem = MenuItems
                .First(x => x.Value.Picture == scheme.Picture)
                .Key;

            var item = Menu.Item(menuItem);

            var tag = new StatePowerScheme(scheme, ActionWithPowerScheme.Create);
            item.Text = _power.TextActionToggle(tag);
            item.Tag = _power.StatePowerSchemeToggle(tag);

            item.BindCommandOnce(_power, static service => new ActionPowerSchemeCommand(service));
        }
    }

    private void AddCustomPowerSchemes()
    {
        var customPowerSchemes = _power.CustomPowerSchemes.ToList();

        if (customPowerSchemes.Count == 0)
        {
            return;
        }

        Menu.Separator();

        foreach (var scheme in customPowerSchemes)
        {
            var item = Menu.Item($"scheme_{scheme.Guid}",
                $@"{Language.Current.DeleteCustomScheme} '{scheme.Name}'", scheme.Picture.GetMenuImage());

            var tag = new StatePowerScheme(scheme);
            item.Tag = tag;

            item.BindCommandOnce(_power, static service => new DeletePowerSchemeCommand(service));
        }
    }
}
