using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Model;
using PowerScheme.Model.Command;
using PowerScheme.Model.Menu;
using PowerScheme.Model.Menu.PowerSchemes;
using PowerScheme.Services;
using PowerScheme.Tests.TestDoubles;
using PowerScheme.Utility;
using PowerSchemeServiceAPI.Model;
using Updater.Common;

namespace PowerScheme.Tests;

[STATestClass]
[DoNotParallelize]
public sealed class MenuItemLifetimeTests
{
    public TestContext TestContext { get; set; } = null!;

    private sealed class CountingCommand : IMenuCommand
    {
        internal int Count { get; private set; }
        public void Execute() => Count++;
    }

    private static ICommand? BoundCommand(ToolStripMenuItem item)
    {
        var bindings = (ConditionalWeakTable<ToolStripMenuItem, CommandEventHandlerAdapter>)
            typeof(MenuItemExtensions).GetField("_map", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        return bindings.TryGetValue(item, out var adapter) ? adapter.Command : null;
    }

    [TestMethod]
    public void BindCommandOnce_StatelessFactoryRunsOnlyOnceUntilUnbound()
    {
        using var item = new ToolStripMenuItem();
        var command = new CountingCommand();
        var created = 0;
        Func<ICommand> factory = () =>
        {
            created++;
            return command;
        };

        for (var index = 0; index < 1000; index++)
        {
            item.BindCommandOnce(factory);
        }
        item.PerformClick();
        Assert.AreEqual(1, created);
        Assert.AreEqual(1, command.Count);
        item.UnbindCommand();
        item.PerformClick();
        Assert.AreEqual(1, command.Count);
        item.BindCommandOnce(factory);
        item.PerformClick();
        Assert.AreEqual(2, created);
        Assert.AreEqual(2, command.Count);
        item.UnbindCommand();
    }

    [TestMethod]
    public void BindCommandOnce_StatefulFactoryRunsOnceAndCommandReadsLatestTag()
    {
        var first = new TestScheme(Guid.NewGuid());
        var latest = new TestScheme(Guid.NewGuid());
        var power = new TestPowerService(first);
        using var item = new ToolStripMenuItem { Tag = new StatePowerScheme(first) };
        var created = 0;
        Func<TestPowerService, ICommand> factory = service =>
        {
            created++;
            return new PowerSchemeCommand(service);
        };

        for (var index = 0; index < 1000; index++)
        {
            item.BindCommandOnce(power, factory);
        }
        item.Tag = new StatePowerScheme(latest);
        item.PerformClick();

        Assert.AreEqual(1, created);
        Assert.AreEqual(1, power.Activations.Count);
        Assert.AreSame(latest, power.Activations[0]);
        item.UnbindCommand();
    }

    [DllImport("user32.dll")]
    private static extern int GetGuiResources(IntPtr process, int flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr window);

    private readonly struct PopupMemorySnapshot(
        long managed, long privateBytes, long workingSet, int handles, int gdi, int user)
    {
        internal int Gdi { get; } = gdi;
        internal int User { get; } = user;

        public override string ToString() =>
            $"Managed={managed}; Private={privateBytes}; WorkingSet={workingSet}; Handles={handles}; GDI={Gdi}; USER={User}";
    }

    private static PopupMemorySnapshot PopupMemory(bool collect)
    {
        var managed = GC.GetTotalMemory(collect);
        using var process = Process.GetCurrentProcess();
        process.Refresh();
        return new PopupMemorySnapshot(managed, process.PrivateMemorySize64, process.WorkingSet64,
            process.HandleCount, GetGuiResources(process.Handle, 0), GetGuiResources(process.Handle, 1));
    }

    private static int ShowPopupTree(ToolStripDropDown popup, ICommand safeCommand)
    {
        popup.Update();
        Application.DoEvents();
        Assert.IsTrue(popup.Visible);
        Assert.IsTrue(IsWindowVisible(popup.Handle));
        var shown = 1;
        foreach (ToolStripItem entry in popup.Items)
        {
            if (entry is not ToolStripMenuItem item)
            {
                continue;
            }
            item.BindCommand(safeCommand);
            if (!item.HasDropDownItems)
            {
                continue;
            }
            item.ShowDropDown();
            shown += ShowPopupTree(item.DropDown, safeCommand);
            item.HideDropDown();
            Application.DoEvents();
        }
        return shown;
    }

    [TestMethod]
    [TestCategory("Interactive")]
    [DataRow(false)]
    [DataRow(true)]
    public void VisibleMenus_RepeatedOpenCloseMeasureMemory(bool right)
    {
        if (Environment.GetEnvironmentVariable("POWERSCHEME_UI_MEMORY_TEST") != "1")
        {
            Assert.Inconclusive("Opt-in desktop test: set POWERSCHEME_UI_MEMORY_TEST=1.");
        }
        var cycles = int.TryParse(Environment.GetEnvironmentVariable("POWERSCHEME_UI_MEMORY_CYCLES"), out var configured)
            ? configured : 300;
        Assert.IsTrue(cycles > 0 && cycles <= 1000);
        var active = new TestScheme(Guid.NewGuid()) { Name = "Balanced", Picture = ImageItem.Balance };
        var native = new TestScheme(Guid.NewGuid()) { Name = "Performance", Picture = ImageItem.High };
        var user = new TestScheme(Guid.NewGuid(), isNative: false) { Name = "Custom", Picture = ImageItem.Empty };
        var power = new TestPowerService(active)
        {
            NativeSchemes = [active, native], UserSchemes = [user], Schemes = [active, native, user],
            CustomSchemes = [user], ExistsHibernate = true
        };
        var updates = new TestUpdateService { ReleaseInfo = AvailableRelease(2) };
        using var components = new Container();
        using ContextMainMenu menu = right
            ? new RightContextMenu(components, power, updates)
            : new LeftContextMenu(components, power);
        var area = Screen.PrimaryScreen!.WorkingArea;
        using var host = new Form
        {
            Text = "PowerScheme popup memory test", ShowInTaskbar = false, ShowIcon = false,
            StartPosition = FormStartPosition.Manual, Location = new Point(area.Left + 20, area.Top + 20),
            ClientSize = new Size(260, 80)
        };
        var safeCommand = new CountingCommand();
        Exception? failure = null;
        var totalPopups = 0;
        void Cycle()
        {
            menu.UpdateMenu();
            foreach (var item in MenuTree(menu.Items).OfType<ToolStripMenuItem>())
            {
                item.BindCommand(safeCommand);
            }
            try
            {
                menu.Show(host, new Point(10, 10));
                totalPopups += ShowPopupTree(menu, safeCommand);
            }
            finally
            {
                menu.Close(ToolStripDropDownCloseReason.CloseCalled);
                Application.DoEvents();
            }
            Assert.IsFalse(menu.Visible);
        }
        host.Shown += (_, _) => host.BeginInvoke((Action)(() =>
        {
            try
            {
                for (var index = 0; index < 20; index++)
                {
                    Cycle();
                }
                TestContext.WriteLine($"Menu={(right ? "right" : "left")}; warmup=20; cycles={cycles}.");
                TestContext.WriteLine($"Warmup, raw: {PopupMemory(false)}");
                var baseline = PopupMemory(true);
                TestContext.WriteLine($"Warmup, after diagnostic GC: {baseline}");
                for (var index = 1; index <= cycles; index++)
                {
                    Cycle();
                    if (index % 100 == 0 || index == cycles)
                    {
                        TestContext.WriteLine($"After {index} cycles, raw: {PopupMemory(false)}");
                    }
                }
                var final = PopupMemory(true);
                TestContext.WriteLine($"Final, after diagnostic GC: {final}; native popups shown={totalPopups}.");
                Assert.IsTrue(final.Gdi <= baseline.Gdi + 2, "GDI objects increased after warmup.");
                Assert.IsTrue(final.User <= baseline.User + 2, "USER objects increased after warmup.");
                Assert.AreEqual(0, power.Activations.Count);
                Assert.AreEqual(0, power.Writes.Count);
                Assert.AreEqual(0, updates.UpdateCount);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                host.Close();
            }
        }));
        Application.Run(host);
        if (failure != null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    private sealed class TestUpdateService : IUpdateService
    {
        public ReleaseInfo ReleaseInfo { get; set; } = ReleaseInfo.Empty;
        public int UpdateCount { get; private set; }
        public Task<ReleaseInfo> GetReleaseInfoAsync() => Task.FromResult(ReleaseInfo);
        public void Update() => UpdateCount++;
    }

    private static ReleaseInfo AvailableRelease(int major) =>
        (ReleaseInfo)Activator.CreateInstance(typeof(ReleaseInfo),
            BindingFlags.Instance | BindingFlags.NonPublic, null,
            [new Version(1, 0), new Version(major, 0), string.Empty, 0L], null)!;

    private static IEnumerable<ToolStripItem> MenuTree(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            yield return item;
            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                foreach (var child in MenuTree(menuItem.DropDownItems))
                {
                    yield return child;
                }
            }
        }
    }

    [TestMethod]
    public void RightMenu_InitialBuildDoesNotEnumerateDynamicSchemesAndFailedUpdateCanBeRetried()
    {
        using var components = new Container();
        IEnumerable<IPowerScheme> FailingSchemes()
        {
            yield return new TestScheme(Guid.NewGuid());
            throw new InvalidOperationException("Enumeration failed.");
        }
        var power = new TestPowerService(new TestScheme(Guid.Empty)) { CustomSchemes = FailingSchemes() };
        using var menu = new RightContextMenu(components, power, new TestUpdateService());

        menu.BuildMenu();
        menu.BuildMenu();

        Assert.AreEqual(8, menu.Items.Count);
        Assert.ThrowsExactly<InvalidOperationException>(() => menu.UpdateMenu());
        Assert.AreEqual(0, menu.Items.Count);
        power.CustomSchemes = [];
        menu.UpdateMenu();
        Assert.AreEqual(8, menu.Items.Count);
    }

    [TestMethod]
    public void RightMenu_RepeatedUpdatesReuseWholeTreeAndImages()
    {
        using var components = new Container();
        var active = new TestScheme(Guid.NewGuid());
        var other = new TestScheme(Guid.NewGuid()) { Name = "Other" };
        var power = new TestPowerService(active) { Schemes = [active, other], CustomSchemes = [other] };
        using var menu = new RightContextMenu(components, power, new TestUpdateService());
        menu.UpdateMenu();
        var original = MenuTree(menu.Items).ToArray();
        var commands = original.OfType<ToolStripMenuItem>()
            .Select(item => (Item: item, Command: BoundCommand(item))).ToArray();
        var images = original.Select(item => item.Image).ToArray();
        var renderer = menu.Renderer;
        var disposed = 0;
        foreach (var item in original)
        {
            item.Disposed += (_, _) => disposed++;
        }
        using var process = Process.GetCurrentProcess();
        var heapBefore = GC.GetTotalMemory(true);
        process.Refresh();
        var privateBefore = process.PrivateMemorySize64;
        var gdiBefore = GetGuiResources(process.Handle, 0);
        var userBefore = GetGuiResources(process.Handle, 1);

        for (var index = 0; index < 1000; index++)
        {
            menu.UpdateMenu();
        }

        var heapAfter = GC.GetTotalMemory(true);
        process.Refresh();
        var privateAfter = process.PrivateMemorySize64;
        for (var index = 0; index < 1000; index++)
        {
            menu.UpdateMenu();
        }
        var heapFinal = GC.GetTotalMemory(true);
        process.Refresh();
        TestContext.WriteLine($"Two batches of 1000 right-menu updates; managed heap: {heapBefore} -> {heapAfter} -> {heapFinal}; private bytes: {privateBefore} -> {privateAfter} -> {process.PrivateMemorySize64}; GDI: {gdiBefore} -> {GetGuiResources(process.Handle, 0)}; USER: {userBefore} -> {GetGuiResources(process.Handle, 1)}.");
        var current = MenuTree(menu.Items).ToArray();
        CollectionAssert.AreEqual(original, current);
        for (var itemIndex = 0; itemIndex < current.Length; itemIndex++)
        {
            Assert.AreSame(images[itemIndex], current[itemIndex].Image);
        }
        Assert.AreSame(renderer, menu.Renderer);
        Assert.AreEqual(0, disposed);
        foreach (var binding in commands)
        {
            Assert.AreSame(binding.Command, BoundCommand(binding.Item));
        }
        menu.Dispose();
        Assert.AreEqual(original.Length, disposed);
        Assert.AreEqual(0, power.Activations.Count);
    }

    [TestMethod]
    public void RightMenu_StructureChangesPreserveUnaffectedItems()
    {
        using var components = new Container();
        var power = new TestPowerService(new TestScheme(Guid.NewGuid()));
        using var menu = new RightContextMenu(components, power, new TestUpdateService());
        menu.UpdateMenu();
        var settings = (ToolStripMenuItem)menu.Items[nameof(MenuLookup.MenuItm.Parameters)]!;
        Assert.IsNull(settings.Image);
        var quit = menu.Items[nameof(MenuLookup.MenuItm.Quit)];
        var first = new TestScheme(Guid.NewGuid(), isNative: false) { Name = "Custom" };
        power.CustomSchemes = [first];
        power.ExistsHibernate = true;

        menu.UpdateMenu();

        Assert.AreSame(settings, menu.Items[nameof(MenuLookup.MenuItm.Parameters)]);
        Assert.AreSame(quit, menu.Items[nameof(MenuLookup.MenuItm.Quit)]);
        var custom = settings.DropDownItems[$"scheme_{first.Guid}"]!;
        var hibernate = menu.Items[nameof(MenuLookup.MenuItm.Hibernate)]!;
        var removed = 0;
        custom.Disposed += (_, _) => removed++;
        hibernate.Disposed += (_, _) => removed++;
        var renamed = new TestScheme(first.Guid, isNative: false) { Name = "Renamed" };
        power.CustomSchemes = [renamed];
        menu.UpdateMenu();
        Assert.AreSame(custom, settings.DropDownItems[$"scheme_{first.Guid}"]);
        StringAssert.Contains(custom.Text, "Renamed");
        Assert.AreSame(renamed, ((StatePowerScheme)custom.Tag!).PowerScheme);

        power.CustomSchemes = [];
        power.ExistsHibernate = false;
        menu.UpdateMenu();
        Assert.AreEqual(2, removed);
        Assert.IsNull(settings.DropDownItems[$"scheme_{first.Guid}"]);
        Assert.IsNull(menu.Items[nameof(MenuLookup.MenuItm.Hibernate)]);
        Assert.AreSame(settings, menu.Items[nameof(MenuLookup.MenuItm.Parameters)]);
    }

    [TestMethod]
    public void RightMenu_UpdateAvailabilityRefreshesExistingCommandWithoutDuplicateClicks()
    {
        using var components = new Container();
        var updates = new TestUpdateService();
        using var menu = new RightContextMenu(components, new TestPowerService(new TestScheme(Guid.Empty)), updates);
        menu.UpdateMenu();
        var version = (ToolStripMenuItem)menu.Items[nameof(MenuLookup.MenuItm.Version)]!;
        Assert.IsNull(version.Image);
        Assert.AreEqual(0, version.DropDownItems.Count);
        updates.ReleaseInfo = AvailableRelease(2);
        menu.UpdateMenu();
        var command = (ToolStripMenuItem)version.DropDownItems[nameof(MenuLookup.MenuItm.UpdateApp)]!;

        updates.ReleaseInfo = AvailableRelease(3);
        for (var index = 0; index < 100; index++)
        {
            menu.UpdateMenu();
        }

        Assert.AreSame(command, version.DropDownItems[nameof(MenuLookup.MenuItm.UpdateApp)]);
        StringAssert.Contains(command.Text, "3.0");
        command.PerformClick();
        Assert.AreEqual(1, updates.UpdateCount);
        var disposed = false;
        command.Disposed += (_, _) => disposed = true;
        updates.ReleaseInfo = ReleaseInfo.Empty;
        menu.UpdateMenu();
        Assert.IsTrue(disposed);
        Assert.AreEqual(0, version.DropDownItems.Count);
    }

    [TestMethod]
    public void RightMenu_LanguageChangeRefreshesTextAndSelectionWithoutReplacingItems()
    {
        var original = Language.Code;
        try
        {
            Language.SetLanguage("en");
            MenuLookup.RebuildMenuItems();
            using var components = new Container();
            using var menu = new RightContextMenu(components, new TestPowerService(new TestScheme(Guid.Empty)), new TestUpdateService());
            menu.UpdateMenu();
            var quit = menu.Items[nameof(MenuLookup.MenuItm.Quit)]!;
            var settings = (ToolStripMenuItem)menu.Items[nameof(MenuLookup.MenuItm.Parameters)]!;
            var languages = (ToolStripMenuItem)settings.DropDownItems[nameof(MenuLookup.MenuItm.Language)]!;
            var english = languages.DropDownItems["Language_en"]!;
            var englishCommand = BoundCommand((ToolStripMenuItem)english);

            Language.SetLanguage("ru");
            MenuLookup.RebuildMenuItems();
            menu.UpdateMenu();
            languages.DropDown.Opening += (_, args) => args.Cancel = true;
            languages.ShowDropDown();

            Assert.AreSame(quit, menu.Items[nameof(MenuLookup.MenuItm.Quit)]);
            Assert.AreEqual(Language.Current.Quit, quit.Text);
            Assert.AreSame(english, languages.DropDownItems["Language_en"]);
            Assert.AreSame(englishCommand, BoundCommand((ToolStripMenuItem)english));
            Assert.AreSame(ImageItem.Empty.GetMenuImage(), english.Image);
            Assert.AreSame(ImageItem.Check.GetMenuImage(), languages.DropDownItems["Language_ru"]!.Image);
        }
        finally
        {
            Language.SetLanguage(original);
            MenuLookup.RebuildMenuItems();
        }
    }

    [TestMethod]
    public void MenuImages_AreSharedAndSurviveDisposalOfEitherMenu()
    {
        var shared = ImageItem.Check.GetMenuImage();
        var first = MenuItemFactory.Create(MenuLookup.MenuItm.Parameters);
        first.CheckMenu(true);
        var second = MenuItemFactory.Create(MenuLookup.MenuItm.Parameters);
        second.CheckMenu(true);
        try
        {
            Assert.AreSame(shared, first.Image);
            Assert.AreSame(shared, second.Image);
            first.SafeRemove();
            Assert.IsTrue(second.Image!.Width > 0);
            Assert.AreSame(shared, ImageItem.Check.GetMenuImage());
            using var owned = ImageItem.Check.GetImage();
            Assert.AreNotSame(shared, owned);
        }
        finally
        {
            first.SafeRemove();
            second.SafeRemove();
        }
        Assert.IsTrue(shared.Width > 0);
        Assert.AreSame(ImageExtensions.GetMenuCheckImage(true, true), ImageExtensions.GetMenuCheckImage(true, true));
    }

    [TestMethod]
    public void LeftMenu_RepeatedUpdatesReuseItemsAndImages()
    {
        using var components = new Container();
        var native = new TestScheme(Guid.NewGuid()) { Name = "Native" };
        var user = new TestScheme(Guid.NewGuid(), isNative: false) { Name = "User" };
        var power = new TestPowerService(native) { NativeSchemes = [native], UserSchemes = [user] };
        using var menu = new LeftContextMenu(components, power);
        menu.UpdateMenu();
        var original = menu.Items.Cast<ToolStripItem>().ToArray();
        var commands = original.OfType<ToolStripMenuItem>()
            .Select(item => (Item: item, Command: BoundCommand(item))).ToArray();
        var disposed = 0;
        foreach (var item in original)
        {
            item.Disposed += (_, _) => disposed++;
        }

        for (var index = 0; index < 1000; index++)
        {
            var previous = menu.Items.Cast<ToolStripItem>().ToArray();
            var images = previous.Select(item => item.Image).OfType<Bitmap>().ToArray();
            menu.UpdateMenu();

            Assert.AreEqual(0, disposed);
            CollectionAssert.AreEqual(original, menu.Items.Cast<ToolStripItem>().ToArray());
            Assert.AreEqual(3, menu.Items.Count);
            Assert.AreEqual("Native", menu.Items[0].Text);
            Assert.AreEqual("User", menu.Items[2].Text);
            Assert.AreSame(native, ((StatePowerScheme)menu.Items[0].Tag!).PowerScheme);
            foreach (var image in images)
            {
                Assert.IsTrue(image.Width > 0);
            }
        }
        var finalImages = menu.Items.Cast<ToolStripItem>().Select(item => item.Image).OfType<Bitmap>().ToArray();
        foreach (var binding in commands)
        {
            Assert.AreSame(binding.Command, BoundCommand(binding.Item));
        }
        menu.Dispose();
        Assert.AreEqual(3, disposed);
        foreach (var image in finalImages)
        {
            Assert.IsTrue(image.Width > 0);
        }
        Assert.AreEqual(0, power.Activations.Count);
    }

    [TestMethod]
    public void LeftMenu_SchemeChangesReuseMatchingItemsAndRefreshCommandTags()
    {
        using var components = new Container();
        var first = new TestScheme(Guid.NewGuid()) { Name = "First" };
        var removed = new TestScheme(Guid.NewGuid()) { Name = "Removed" };
        var power = new TestPowerService(first) { NativeSchemes = [first, removed] };
        using var menu = new LeftContextMenu(components, power);
        menu.UpdateMenu();
        var retained = menu.Items[0];
        var disposed = false;
        menu.Items[1].Disposed += (_, _) => disposed = true;
        var renamed = new TestScheme(first.Guid) { Name = "Renamed" };
        var added = new TestScheme(Guid.NewGuid()) { Name = "Added" };
        power.NativeSchemes = [added, renamed];

        menu.UpdateMenu();

        Assert.AreSame(retained, menu.Items[1]);
        Assert.AreEqual("Renamed", retained.Text);
        Assert.IsTrue(disposed);
        for (var index = 0; index < 100; index++)
        {
            menu.UpdateMenu();
        }
        ((ToolStripMenuItem)retained).PerformClick();
        Assert.AreEqual(1, power.Activations.Count);
        Assert.AreSame(renamed, power.Activations[0]);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void LeftMenu_UnfinishedBuildStillReleasesOwnedItems(bool disposeMenu)
    {
        using var components = new Container();
        using var menu = new LeftContextMenu(components, new TestPowerService(new TestScheme(Guid.Empty)));
        using var image = new Bitmap(8, 8);
        var pending = new ToolStripMenuItem { Image = image };
        var disposed = false;
        pending.Disposed += (_, _) => disposed = true;
        menu.Items.Add(pending);

        if (disposeMenu)
        {
            menu.Dispose();
        }
        else
        {
            menu.ClearMenu();
        }

        Assert.IsTrue(disposed);
        Assert.ThrowsExactly<ArgumentException>(() => image.GetPixel(0, 0));
        Assert.AreEqual(0, menu.Items.Count);
    }

    [TestMethod]
    public void PowerSchemeBuilder_EnumerationFailureReleasesRootAndPartialItems()
    {
        var power = new TestPowerService(new TestScheme(Guid.Empty));
        var builder = new PowerSchemeMenuBuilder(power);
        var root = (ToolStripMenuItem)typeof(BaseMenuBuilder)
            .GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(builder)!;
        var rootImage = (Bitmap)root.Image!;
        Bitmap? childImage = null;
        var disposed = 0;
        root.Disposed += (_, _) => disposed++;
        IEnumerable<IPowerScheme> FailingSchemes()
        {
            yield return new TestScheme(Guid.NewGuid());
            var child = root.DropDownItems[0];
            childImage = (Bitmap)child.Image!;
            child.Disposed += (_, _) => disposed++;
            throw new InvalidOperationException("Enumeration failed.");
        }
        power.NativeSchemes = FailingSchemes();

        Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());

        Assert.AreEqual(2, disposed);
        Assert.IsNotNull(childImage);
        Assert.IsNull(root.Image);
        Assert.IsTrue(childImage.Width > 0);
        Assert.IsTrue(rootImage.Width > 0);
    }

    [TestMethod]
    public void LeftMenu_FailedUpdateCanBeRetriedWithoutStaleItems()
    {
        using var components = new Container();
        var scheme = new TestScheme(Guid.NewGuid());
        var power = new TestPowerService(scheme) { NativeSchemes = [scheme] };
        using var menu = new LeftContextMenu(components, power);
        menu.UpdateMenu();
        var previousImage = (Bitmap)menu.Items[0].Image!;
        IEnumerable<IPowerScheme> FailingSchemes()
        {
            yield return scheme;
            throw new InvalidOperationException("Enumeration failed.");
        }
        power.NativeSchemes = FailingSchemes();

        Assert.ThrowsExactly<InvalidOperationException>(() => menu.UpdateMenu());

        Assert.AreEqual(0, menu.Items.Count);
        Assert.IsTrue(previousImage.Width > 0);
        power.NativeSchemes = [scheme];
        menu.UpdateMenu();
        Assert.AreEqual(1, menu.Items.Count);
        Assert.IsTrue(menu.Items[0].Image!.Width > 0);
    }

    [TestMethod]
    public void RepeatedReplacements_ReleaseEveryOldBranchImmediately()
    {
        using var menu = new ContextMenuStrip();
        var disposed = 0;
        ToolStripMenuItem CreateBranch()
        {
            var root = new ToolStripMenuItem { Name = "Parameters", Image = new Bitmap(8, 8) };
            var child = new ToolStripMenuItem { Image = new Bitmap(8, 8) };
            root.DropDownItems.Add(child);
            root.Disposed += (_, _) => disposed++;
            child.Disposed += (_, _) => disposed++;
            return root;
        }
        menu.Items.Add(CreateBranch());

        for (var index = 0; index < 1000; index++)
        {
            var previous = (ToolStripMenuItem)menu.Items[0];
            var image = (Bitmap)previous.Image!;
            menu.Items.ReplaceMenu(CreateBranch);
            Assert.ThrowsExactly<ArgumentException>(() => image.GetPixel(0, 0));
            Assert.AreEqual((index + 1) * 2, disposed);
            Assert.AreEqual(1, menu.Items.Count);
        }
        menu.Items.RemoveMenu();
        Assert.AreEqual(2002, disposed);
        Assert.AreEqual(0, menu.Items.Count);
    }

    [TestMethod]
    [DataRow("Parameters", false, false)]
    [DataRow("Parameters", true, true)]
    [DataRow("Sleep", false, true)]
    [DataRow("Sleep", true, true)]
    [DataRow("Hibernate", false, true)]
    [DataRow("Hibernate", true, true)]
    public void CheckMenu_ReleasesPreviousImageAndPreservesExpectedGlyph(string name, bool isChecked, bool hasImage)
    {
        using var previous = new Bitmap(8, 8);
        var item = new ToolStripMenuItem { Name = name, Image = previous };
        try
        {
            item.CheckMenu(isChecked);

            Assert.ThrowsExactly<ArgumentException>(() => previous.GetPixel(0, 0));
            Assert.AreEqual(hasImage, item.Image != null);
            if (item.Image is Bitmap image)
            {
                Assert.IsTrue(image.Width > 0);
                Assert.IsTrue(image.Height > 0);
            }
        }
        finally
        {
            item.SafeRemove();
        }
    }

    [TestMethod]
    public void ReplaceMenu_DisposesOldBranchAndAllOwnedImages()
    {
        using var menu = new ContextMenuStrip();
        using var rootImage = new Bitmap(8, 8);
        using var childImage = new Bitmap(8, 8);
        using var nestedImage = new Bitmap(8, 8);
        var old = new ToolStripMenuItem { Name = "Parameters", Image = rootImage };
        var child = new ToolStripMenuItem { Image = childImage };
        var nested = new ToolStripMenuItem { Image = nestedImage };
        child.DropDownItems.Add(nested);
        old.DropDownItems.Add(child);
        menu.Items.Add(old);
        var disposed = new List<ToolStripMenuItem>();
        foreach (var item in new[] { old, child, nested })
        {
            item.Disposed += (_, _) => disposed.Add(item);
        }
        using var replacement = new ToolStripMenuItem { Name = "Parameters" };

        menu.Items.ReplaceMenu(() => replacement);

        Assert.AreSame(replacement, menu.Items[0]);
        CollectionAssert.AreEquivalent(new[] { old, child, nested }, disposed);
        foreach (var item in new[] { old, child, nested })
        {
            Assert.IsNull(item.Image);
        }
        foreach (var image in new[] { rootImage, childImage, nestedImage })
        {
            Assert.ThrowsExactly<ArgumentException>(() => image.GetPixel(0, 0));
        }
    }

    [TestMethod]
    public void ReplaceMenu_MissingSlotDisposesUnusedReplacement()
    {
        using var menu = new ContextMenuStrip();
        using var image = new Bitmap(8, 8);
        var unused = new ToolStripMenuItem { Name = "Missing", Image = image };
        var disposed = false;
        unused.Disposed += (_, _) => disposed = true;

        menu.Items.ReplaceMenu(() => unused);

        Assert.IsTrue(disposed);
        Assert.ThrowsExactly<ArgumentException>(() => image.GetPixel(0, 0));
        Assert.AreEqual(0, menu.Items.Count);
    }
}