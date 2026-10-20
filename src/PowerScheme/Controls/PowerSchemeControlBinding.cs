using System;
using System.Diagnostics;
using System.Windows.Forms;
using Languages;
using PowerScheme.Model.Options;
using PowerScheme.Themes;

namespace PowerScheme.Controls;

internal static class PowerSchemeControlBinding
{
    internal static void BindButtons(
        Button okButton,
        Button applyButton,
        Button defaultButton,
        PowerSchemeOptions options,
        Action hide,
        Action applyValues,
        Action reviewApplyButton,
        Action resetDefaults)
    {
        okButton.Text = Language.Current.Ok;
        applyButton.Text = Language.Current.Apply;
        defaultButton.Text = Language.Current.Default;

        okButton.Click += (_, _) =>
        {
            hide();
            applyValues();
        };
        applyButton.Click += (_, _) =>
        {
            Debug.WriteLine($"Selected Power Scheme Id: {options.PowerSchemeId}");
            applyValues();
            reviewApplyButton();
        };
        defaultButton.Click += (_, _) => resetDefaults();

        defaultButton.Visible = false;
    }

    internal static void BindSchemes(ComboBox comboBox, Label label, PowerSchemeOptions options, Action reviewApplyButton)
    {
        options.PowerSchemeIdChanged += (_, _) => reviewApplyButton();
        label.Text = Language.Current.PowerSchemes;

        var binding = new BindingSource
        {
            DataSource = options.PowerSchemes
        };

        binding.ResetBindings(false);
        comboBox.DataSource = binding;
        comboBox.DisplayMember = "Value";
        comboBox.ValueMember = "Key";
        comboBox.DataBindings.Add(nameof(comboBox.SelectedValue), options, nameof(options.PowerSchemeId), true, DataSourceUpdateMode.OnPropertyChanged);
    }

    internal static void ReviewApplyButton(BaseButton button, ToolTip toolTip, bool acEqualsDc) =>
        ReviewApplyButton(button, toolTip, acEqualsDc, ThemeService.SetButtonTheme, ThemeService.SetAttentionButtonTheme);

    internal static void ReviewApplyButton(
        BaseButton button,
        ToolTip toolTip,
        bool acEqualsDc,
        Action<BaseButton> applyNormalTheme,
        Action<BaseButton> applyAttentionTheme)
    {
        if (acEqualsDc)
        {
            applyNormalTheme(button);
            toolTip.SetToolTip(button, null);
        }
        else
        {
            applyAttentionTheme(button);
            toolTip.SetToolTip(button, Language.Current.SyncAcDc);
        }
    }
}