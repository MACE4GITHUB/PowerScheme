using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Controls;
using PowerScheme.Model.Options;
using PowerScheme.Tests.TestDoubles;

namespace PowerScheme.Tests;

[STATestClass]
[DoNotParallelize]
public sealed class PowerSchemeControlBindingTests
{
    [TestMethod]
    [DataRow("en")]
    [DataRow("ru")]
    public void BindButtons_SetsCurrentLabelsAndHidesDefaultsWithoutExecutingCommands(string language)
    {
        var originalLanguage = Language.Code;
        try
        {
            Language.SetLanguage(language);
            using var context = new ButtonContext();

            Assert.AreEqual(Language.Current.Ok, context.Ok.Text);
            Assert.AreEqual(Language.Current.Apply, context.Apply.Text);
            Assert.AreEqual(Language.Current.Default, context.Default.Text);
            Assert.IsFalse(context.Default.Visible);
            Assert.AreEqual(0, context.Calls.Count);
        }
        finally
        {
            Language.SetLanguage(originalLanguage);
        }
    }

    [TestMethod]
    [DataRow("ok", "hide,apply")]
    [DataRow("apply", "apply,review")]
    [DataRow("default", "reset")]
    public void ButtonClick_RunsOnlySelectedCommandsInOriginalOrder(string button, string expectedCalls)
    {
        using var context = new ButtonContext();

        context.Click(button);

        Assert.AreEqual(expectedCalls, string.Join(",", context.Calls));
    }

    [TestMethod]
    [DataRow("ok", "hide", "hide")]
    [DataRow("ok", "apply", "hide,apply")]
    [DataRow("apply", "apply", "apply")]
    [DataRow("apply", "review", "apply,review")]
    [DataRow("default", "reset", "reset")]
    public void ButtonClick_FailurePropagatesWithoutRunningLaterCommands(string button, string failingCommand, string expectedCalls)
    {
        using var context = new ButtonContext { FailingCommand = failingCommand };

        var actual = Assert.ThrowsExactly<InvalidOperationException>(() => context.Click(button));

        Assert.AreSame(context.Failure, actual);
        Assert.AreEqual(expectedCalls, string.Join(",", context.Calls));
    }

    [TestMethod]
    public void ButtonClick_AfterFailure_NextClickStillRunsCommandsOnce()
    {
        using var context = new ButtonContext { FailingCommand = "apply" };
        Assert.ThrowsExactly<InvalidOperationException>(() => context.Click("apply"));
        context.FailingCommand = null;
        context.Calls.Clear();

        context.Click("apply");
        context.Click("apply");

        Assert.AreEqual("apply,review,apply,review", string.Join(",", context.Calls));
    }

    [TestMethod]
    public void SeparateButtonBindings_KeepCallbacksIndependent()
    {
        using var first = new ButtonContext();
        using var second = new ButtonContext();

        first.Click("ok");
        second.Click("apply");

        Assert.AreEqual("hide,apply", string.Join(",", first.Calls));
        Assert.AreEqual("apply,review", string.Join(",", second.Calls));
    }

    [TestMethod]
    public void BindSchemes_ConfiguresListAndImmediateSelectionBinding()
    {
        using var context = new BindingContext();
        var binding = context.ComboBox.DataBindings[0];

        Assert.AreEqual(Language.Current.PowerSchemes, context.Label.Text);
        Assert.AreEqual("Value", context.ComboBox.DisplayMember);
        Assert.AreEqual("Key", context.ComboBox.ValueMember);
        Assert.AreEqual(3, ((BindingSource)context.ComboBox.DataSource).Count);
        Assert.AreSame(context.Options, binding.DataSource);
        Assert.AreEqual(nameof(ComboBox.SelectedValue), binding.PropertyName);
        Assert.AreEqual(nameof(PowerSchemeOptions.PowerSchemeId), binding.BindingMemberInfo.BindingMember);
        Assert.AreEqual(DataSourceUpdateMode.OnPropertyChanged, binding.DataSourceUpdateMode);
        Assert.IsTrue(binding.FormattingEnabled);
    }

    [TestMethod]
    public void SelectionChange_LoadsModelBeforeReviewingButton()
    {
        using var context = new BindingContext();
        context.ReviewedIds.Clear();

        context.ComboBox.SelectedValue = context.Other.Guid;

        Assert.AreEqual(context.Other.Guid, context.Options.PowerSchemeId);
        Assert.AreEqual(context.Other.Guid, context.Options.LoadedId);
        CollectionAssert.AreEqual(new[] { context.Other.Guid }, context.ReviewedIds);
    }

    [TestMethod]
    public void ModelChange_UpdatesSelectionAndReviewsButtonOnce()
    {
        using var context = new BindingContext();
        context.ReviewedIds.Clear();

        context.Options.PowerSchemeId = context.Other.Guid;

        Assert.AreEqual(context.Other.Guid, context.ComboBox.SelectedValue);
        CollectionAssert.AreEqual(new[] { context.Other.Guid }, context.ReviewedIds);
    }

    [TestMethod]
    public void AllSchemesSelection_UsesEmptyGuidAndSameSelectionDoesNotReviewAgain()
    {
        using var context = new BindingContext();
        context.ComboBox.SelectedValue = context.Other.Guid;
        context.ReviewedIds.Clear();

        context.ComboBox.SelectedValue = Guid.Empty;
        context.ComboBox.SelectedValue = Guid.Empty;

        Assert.AreEqual(Guid.Empty, context.Options.PowerSchemeId);
        CollectionAssert.AreEqual(new[] { Guid.Empty }, context.ReviewedIds);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ReviewApplyButton_SelectsOnlyExpectedThemeAndTooltip(bool acEqualsDc)
    {
        using var button = new BaseButton();
        using var toolTip = new ToolTip();
        toolTip.SetToolTip(button, "previous");
        var normalCalls = 0;
        var attentionCalls = 0;

        PowerSchemeControlBinding.ReviewApplyButton(button, toolTip, acEqualsDc,
            selected =>
            {
                Assert.AreSame(button, selected);
                normalCalls++;
            },
            selected =>
            {
                Assert.AreSame(button, selected);
                attentionCalls++;
            });

        Assert.AreEqual(acEqualsDc ? 1 : 0, normalCalls);
        Assert.AreEqual(acEqualsDc ? 0 : 1, attentionCalls);
        Assert.AreEqual(acEqualsDc ? string.Empty : Language.Current.SyncAcDc, toolTip.GetToolTip(button));
    }

    [TestMethod]
    public void ReviewApplyButton_ReturningToEqualValues_ClearsAttentionTooltip()
    {
        using var button = new BaseButton();
        using var toolTip = new ToolTip();
        var calls = new List<string>();

        PowerSchemeControlBinding.ReviewApplyButton(button, toolTip, false,
            _ => calls.Add("normal"), _ => calls.Add("attention"));
        PowerSchemeControlBinding.ReviewApplyButton(button, toolTip, true,
            _ => calls.Add("normal"), _ => calls.Add("attention"));

        CollectionAssert.AreEqual(new[] { "attention", "normal" }, calls);
        Assert.AreEqual(string.Empty, toolTip.GetToolTip(button));
    }

    [TestMethod]
    public void ReviewApplyButton_ReadsCurrentLanguageOnEachReview()
    {
        using var button = new BaseButton();
        using var toolTip = new ToolTip();
        var originalLanguage = Language.Code;
        try
        {
            Language.SetLanguage("en");
            PowerSchemeControlBinding.ReviewApplyButton(button, toolTip, false, _ => { }, _ => { });
            var english = toolTip.GetToolTip(button);
            Assert.AreEqual(Language.Current.SyncAcDc, english);

            Language.SetLanguage("ru");
            PowerSchemeControlBinding.ReviewApplyButton(button, toolTip, false, _ => { }, _ => { });

            Assert.AreEqual(Language.Current.SyncAcDc, toolTip.GetToolTip(button));
            Assert.AreNotEqual(english, toolTip.GetToolTip(button));
        }
        finally
        {
            Language.SetLanguage(originalLanguage);
        }
    }

    private sealed class ButtonContext : IDisposable
    {
        public TestButton Ok { get; } = new();
        public TestButton Apply { get; } = new();
        public TestButton Default { get; } = new();
        public List<string> Calls { get; } = [];
        public string? FailingCommand { get; set; }
        public InvalidOperationException Failure { get; } = new("Command failed.");

        public ButtonContext()
        {
            var options = new TestOptions(new TestPowerService(new TestScheme(Guid.Empty)));
            PowerSchemeControlBinding.BindButtons(Ok, Apply, Default, options,
                () => Record("hide"), () => Record("apply"), () => Record("review"), () => Record("reset"));
        }

        private void Record(string command)
        {
            Calls.Add(command);
            if (command == FailingCommand)
            {
                throw Failure;
            }
        }

        public void Click(string button)
        {
            var selected = button switch
            {
                "ok" => Ok,
                "apply" => Apply,
                "default" => Default,
                _ => throw new ArgumentOutOfRangeException(nameof(button))
            };
            selected.RaiseClick();
        }

        public void Dispose()
        {
            Ok.Dispose();
            Apply.Dispose();
            Default.Dispose();
        }
    }

    private sealed class TestButton : Button
    {
        public void RaiseClick() => OnClick(EventArgs.Empty);
    }

    private sealed class BindingContext : IDisposable
    {
        private readonly UserControl _parent = new() { BindingContext = new System.Windows.Forms.BindingContext() };
        public ComboBox ComboBox { get; } = new();
        public Label Label { get; } = new();
        public TestScheme Other { get; } = new(Guid.NewGuid()) { Name = "Other" };
        public TestOptions Options { get; }
        public List<Guid> ReviewedIds { get; } = [];

        public BindingContext()
        {
            var main = new TestScheme(Guid.NewGuid()) { Name = "Main" };
            var power = new TestPowerService(main) { Schemes = [main, Other] };
            Options = new TestOptions(power) { PowerSchemeId = main.Guid };
            _parent.Controls.Add(ComboBox);
            _parent.Controls.Add(Label);
            PowerSchemeControlBinding.BindSchemes(ComboBox, Label, Options, () =>
            {
                Assert.AreEqual(Options.PowerSchemeId, Options.LoadedId);
                ReviewedIds.Add(Options.PowerSchemeId);
            });
            _parent.CreateControl();
        }

        public void Dispose()
        {
            var source = (BindingSource)ComboBox.DataSource;
            _parent.Dispose();
            source.Dispose();
        }
    }

    private sealed class TestOptions(TestPowerService power) : PowerSchemeOptions(power)
    {
        public Guid LoadedId { get; private set; }

        protected override void LoadSelectedValues() => LoadedId = PowerSchemeId;
    }
}