using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme.Tests;

[STATestClass]
public sealed class NotifyIconMouseClickTests
{
    [TestMethod]
    [DataRow(MouseButtons.Left)]
    [DataRow(MouseButtons.Right)]
    public void ShowContextMenu_SupportedButton_UpdatesAndShowsOnlySelectedMenu(MouseButtons button)
    {
        using var viewModel = new TestViewModel();
        var selectedMenu = viewModel.GetMenu(button);
        var otherMenu = viewModel.GetMenu(button == MouseButtons.Left ? MouseButtons.Right : MouseButtons.Left);
        var showCount = 0;

        ViewService.ShowContextMenu(viewModel, button, icon =>
        {
            showCount++;
            Assert.AreSame(viewModel.NotifyIcon, icon);
            Assert.AreSame(selectedMenu, icon.ContextMenuStrip);
            Assert.AreEqual(1, selectedMenu.UpdateCount);
            Assert.AreEqual(0, otherMenu.UpdateCount);
        });

        Assert.AreEqual(1, showCount);
        Assert.IsNull(viewModel.NotifyIcon.ContextMenuStrip);
    }

    [TestMethod]
    [DataRow(MouseButtons.None)]
    [DataRow(MouseButtons.Middle)]
    [DataRow(MouseButtons.XButton1)]
    [DataRow(MouseButtons.XButton2)]
    [DataRow(MouseButtons.Left | MouseButtons.Right)]
    public void ShowContextMenu_UnsupportedButton_DoesNotAccessViewModelOrShowMenu(MouseButtons button)
    {
        ViewService.ShowContextMenu(null!, button, _ => Assert.Fail("Unsupported buttons must not show a menu."));
    }

    [TestMethod]
    [DataRow(MouseButtons.Left)]
    [DataRow(MouseButtons.Right)]
    public void ShowContextMenu_RepeatedClick_RefreshesContentAndStateBeforeEachDisplay(MouseButtons button)
    {
        using var viewModel = new TestViewModel();
        var menu = viewModel.GetMenu(button);
        menu.ItemText = "Sleep";
        menu.ItemEnabled = true;
        var showCount = 0;

        ViewService.ShowContextMenu(viewModel, button, icon =>
        {
            showCount++;
            Assert.AreEqual("Sleep", icon.ContextMenuStrip!.Items[0].Text);
            Assert.IsTrue(icon.ContextMenuStrip.Items[0].Enabled);
        });

        menu.ItemText = "Сон";
        menu.ItemEnabled = false;

        ViewService.ShowContextMenu(viewModel, button, icon =>
        {
            showCount++;
            Assert.AreSame(menu, icon.ContextMenuStrip);
            Assert.AreEqual("Сон", icon.ContextMenuStrip!.Items[0].Text);
            Assert.IsFalse(icon.ContextMenuStrip.Items[0].Enabled);
        });

        Assert.AreEqual(2, showCount);
        Assert.AreEqual(2, menu.UpdateCount);
        Assert.IsNull(viewModel.NotifyIcon.ContextMenuStrip);
    }

    [TestMethod]
    [DataRow(MouseButtons.Left)]
    [DataRow(MouseButtons.Right)]
    public void ShowContextMenu_DisplayThrows_ClearsMenuAndPropagatesException(MouseButtons button)
    {
        using var viewModel = new TestViewModel();
        var expected = new InvalidOperationException("Display failed.");

        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ViewService.ShowContextMenu(viewModel, button, icon =>
            {
                Assert.AreSame(viewModel.GetMenu(button), icon.ContextMenuStrip);
                throw expected;
            }));

        Assert.AreSame(expected, actual);
        Assert.IsNull(viewModel.NotifyIcon.ContextMenuStrip);
    }

    [TestMethod]
    [DataRow(MouseButtons.Left)]
    [DataRow(MouseButtons.Right)]
    public void ShowContextMenu_UpdateThrows_DoesNotAttachOrShowMenu(MouseButtons button)
    {
        using var viewModel = new TestViewModel();
        var expected = new InvalidOperationException("Update failed.");
        viewModel.GetMenu(button).UpdateException = expected;

        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ViewService.ShowContextMenu(viewModel, button, _ => Assert.Fail("A failed update must not show a menu.")));

        Assert.AreSame(expected, actual);
        Assert.IsNull(viewModel.NotifyIcon.ContextMenuStrip);
    }

    [TestMethod]
    [DataRow(MouseButtons.Left)]
    [DataRow(MouseButtons.Right)]
    public void ShowContextMenu_AfterDisplayFailure_NextClickCanShowOtherMenu(MouseButtons button)
    {
        using var viewModel = new TestViewModel();
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ViewService.ShowContextMenu(viewModel, button, _ => throw new InvalidOperationException()));

        var nextButton = button == MouseButtons.Left ? MouseButtons.Right : MouseButtons.Left;
        var showCount = 0;

        ViewService.ShowContextMenu(viewModel, nextButton, icon =>
        {
            showCount++;
            Assert.AreSame(viewModel.GetMenu(nextButton), icon.ContextMenuStrip);
        });

        Assert.AreEqual(1, showCount);
        Assert.AreEqual(1, viewModel.GetMenu(button).UpdateCount);
        Assert.AreEqual(1, viewModel.GetMenu(nextButton).UpdateCount);
        Assert.IsNull(viewModel.NotifyIcon.ContextMenuStrip);
    }

    private sealed class TestViewModel : IViewModel
    {
        private readonly Container _components = new();

        public TestViewModel()
        {
            NotifyIcon = new NotifyIcon(_components);
            ContextLeftMenu = new TestMenu(_components);
            ContextRightMenu = new TestMenu(_components);
        }

        public NotifyIcon NotifyIcon { get; }
        public ContextMainMenu ContextLeftMenu { get; }
        public ContextMainMenu ContextRightMenu { get; }
        public IPowerSchemeService Power => throw new NotSupportedException();

        public TestMenu GetMenu(MouseButtons button) =>
            (TestMenu)(button == MouseButtons.Right ? ContextRightMenu : ContextLeftMenu);

        public void BuildAllMenu() => throw new NotSupportedException();
        public void ClearAllMenu() => throw new NotSupportedException();
        public void UpdateIcon(Icon icon) => throw new NotSupportedException();
        public void RemoveIcon() => throw new NotSupportedException();
        public void Dispose() => _components.Dispose();
    }

    private sealed class TestMenu(IContainer components) : ContextMainMenu(components, null!)
    {
        public int UpdateCount { get; private set; }
        public string ItemText { get; set; } = string.Empty;
        public bool ItemEnabled { get; set; } = true;
        public Exception? UpdateException { get; set; }

        internal override void UpdateMenu()
        {
            UpdateCount++;
            if (UpdateException != null)
            {
                throw UpdateException;
            }

            if (Items.Count == 0)
            {
                Items.Add(new ToolStripMenuItem());
            }

            Items[0].Text = ItemText;
            Items[0].Enabled = ItemEnabled;
        }

        internal override void ClearMenu() => throw new NotSupportedException();
        protected override void BuildContextMenu() => throw new NotSupportedException();
    }
}
