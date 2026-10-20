using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Forms;

namespace PowerScheme.Tests;

[STATestClass]
public sealed class PopupFormBehaviorTests
{
    [TestMethod]
    public void Attach_EnablesKeyPreviewWithoutRunningCallbacks()
    {
        using var form = new TestForm();
        var calls = 0;

        PopupFormBehavior.Attach(form, () => calls++, () => calls++, _ => calls++);

        Assert.IsTrue(form.KeyPreview);
        Assert.AreEqual(0, calls);
        Assert.IsFalse(form.Visible);
    }

    [TestMethod]
    public void Activation_AppliesThemeBeforeRefreshingControls()
    {
        using var form = new TestForm();
        var calls = new List<string>();
        PopupFormBehavior.Attach(form,
            () => calls.Add("refresh"),
            () => Assert.Fail("Activation must not hide the form."),
            control =>
            {
                Assert.AreSame(form, control);
                calls.Add("theme");
            });

        form.RaiseActivated();

        CollectionAssert.AreEqual(new[] { "theme", "refresh" }, calls);
    }

    [TestMethod]
    public void RepeatedActivation_ReappliesCurrentThemeAndRefreshesEveryTime()
    {
        using var form = new TestForm();
        var theme = "light";
        var calls = new List<string>();
        PopupFormBehavior.Attach(form, () => calls.Add("refresh"), () => { }, _ => calls.Add(theme));

        form.RaiseActivated();
        theme = "dark";
        form.RaiseActivated();

        CollectionAssert.AreEqual(new[] { "light", "refresh", "dark", "refresh" }, calls);
    }

    [TestMethod]
    public void Deactivation_InvokesOnlyHideWithoutDisposingForm()
    {
        using var form = new TestForm();
        var hideCount = 0;
        PopupFormBehavior.Attach(form,
            () => Assert.Fail("Deactivation must not refresh controls."),
            () => hideCount++,
            _ => Assert.Fail("Deactivation must not apply a theme."));

        form.RaiseDeactivated();

        Assert.AreEqual(1, hideCount);
        Assert.IsFalse(form.IsDisposed);
    }

    [TestMethod]
    [DataRow(Keys.Escape)]
    [DataRow(Keys.Control | Keys.Escape)]
    public void Escape_InvokesHideWithoutDisposingForm(Keys key)
    {
        using var form = new TestForm();
        var hideCount = 0;
        PopupFormBehavior.Attach(form, () => { }, () => hideCount++, _ => { });

        form.RaiseKeyDown(key);

        Assert.AreEqual(1, hideCount);
        Assert.IsFalse(form.IsDisposed);
    }

    [TestMethod]
    [DataRow(Keys.Enter)]
    [DataRow(Keys.Tab)]
    [DataRow(Keys.A)]
    [DataRow(Keys.ControlKey)]
    public void OtherKeys_DoNotHideForm(Keys key)
    {
        using var form = new TestForm();
        PopupFormBehavior.Attach(form, () => { }, () => Assert.Fail("Only Escape should hide the form."), _ => { });

        form.RaiseKeyDown(key);
    }

    [TestMethod]
    public void SeparateForms_KeepTheirCallbacksIndependent()
    {
        using var first = new TestForm();
        using var second = new TestForm();
        var calls = new List<string>();
        PopupFormBehavior.Attach(first, () => calls.Add("first-refresh"), () => calls.Add("first-hide"), _ => { });
        PopupFormBehavior.Attach(second, () => calls.Add("second-refresh"), () => calls.Add("second-hide"), _ => { });

        first.RaiseActivated();
        second.RaiseKeyDown(Keys.Escape);
        first.RaiseDeactivated();

        CollectionAssert.AreEqual(new[] { "first-refresh", "second-hide", "first-hide" }, calls);
    }

    private sealed class TestForm : Form
    {
        public void RaiseActivated() => OnActivated(EventArgs.Empty);
        public void RaiseDeactivated() => OnDeactivate(EventArgs.Empty);
        public void RaiseKeyDown(Keys key) => OnKeyDown(new KeyEventArgs(key));
    }
}
