using System.Linq;
using System.Windows.Forms;
using Common;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme.Model.Menu.Lid;

using static MenuLookup;

internal class LidMenuBuilder(
    IPowerSchemeService power) :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.Lid;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        var dropDownItems = Root.DropDownItems;

        var isShowSleepOption = RegistryService.IsShowSleepOption;
        var isShowHibernateOption = RegistryService.IsShowHibernateOption;

        var activePowerSchemeGuid = power.ActivePowerScheme.Guid;
        var activeLid = (Lid)RegistryService.GetLidOption(activePowerSchemeGuid);

        var pictureName = ImageItem.Empty;
        if (IsHiddenLid(activeLid, isShowSleepOption, isShowHibernateOption))
        {
            activeLid = Lid.Nothing;
            power.SetLid((int)activeLid);
            pictureName = ImageItem.Nothing;
        }

        var lidMenuItems = MenuItems
            .Where(mi => mi.Value.MenuItmKind == MenuItmKind.Lid &&
                         !IsHiddenLid((Lid)mi.Value.Tag!, isShowSleepOption, isShowHibernateOption))
            .ToList();

        foreach (var lidMenuItem in lidMenuItems)
        {
            var lidItem = MenuItemFactory.Create(lidMenuItem.Key);

            var lid = (Lid)lidMenuItem.Value.Tag!;
            lidItem.Tag = lid;
            if (activeLid == lid)
            {
                pictureName = lidMenuItem.Value.Picture;
            }

            var command = new LidCommand(power);
            lidItem.BindCommand(command);

            dropDownItems.Add(lidItem);
        }

        var prev = Root.Image;
        Root.Image = pictureName.GetImage();
        prev?.Dispose();

        return Root;
    }

    private static bool IsHiddenLid(Lid lid, bool isShowSleepOption, bool isShowHibernateOption) =>
        (lid == Lid.Sleep && !isShowSleepOption) ||
        (lid == Lid.Hibernate && !isShowHibernateOption);
}
