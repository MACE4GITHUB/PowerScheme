using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static MessageForm.DialogResultScheme;

namespace MessageForm;

public partial class MainMessageBox : Form, IMainMessageBox
{
    private int _timeout;
    private int _valueTimerTick;
    private string _initialControlValue = "";
    private bool _hasTimer;
    private Timer? _timer;
    private Control? _controlForValueTimerTick;
    private DialogResult _defaultResultAfterTimeout;
    private DialogResult _returnResultAfterTimeout;

    private readonly IWin32Window _owner;

    public MainMessageBox()
    {
        InitializeComponent();

        _labelTimerTick.Text = "";

        _owner = new WindowWrapper(Handle);

        Font = new Font(new FontFamily("Microsoft Sans Serif"),
            8.25f,
            FontStyle.Regular,
            GraphicsUnit.Point);
    }

    public DialogResult Show(
        string message,
        string? title = null,
        MessageBoxButtons? buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.Information,
        bool isApplicationExit = false,
        int timeout = 0,
        DialogResult defaultResultAfterTimeout = DialogResult.None)
    {

        SetTimeout(timeout);

        Text = title;
        _lblMessage.Text = message;
        SetButtons(buttons, defaultResultAfterTimeout);
        SetIcon(icon);
        SetSizeMessageBox();

        if (isApplicationExit)
        {
            FormClosed += OnFormClosed;
        }

        if (_hasTimer)
        {
            LoadTimer();
        }

        var resultDialog = ShowDialog(_owner);
        var result = resultDialog == DialogResult.Cancel
            ? _returnResultAfterTimeout
            : resultDialog;

        return result;
    }

    private static void OnFormClosed(object? sender, FormClosedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void SetTimeout(int timeout)
    {
        if (timeout < 0)
        {
            timeout = 0;
        }

        if (timeout > 1440)
        {
            timeout = 1440;
        }

        _timeout = timeout;
        if (_timeout > 0)
        {
            _hasTimer = true;
        }
    }

    private void LoadTimer()
    {
        _valueTimerTick = _timeout;
        _timer = new Timer { Interval = 1000, Enabled = true };
        _timer.Start();
        _timer.Tick += TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _valueTimerTick--;

        if (_valueTimerTick > 0)
        {
            _controlForValueTimerTick?.Text =
                string.IsNullOrWhiteSpace(_initialControlValue)
                    ? $@"{_valueTimerTick}"
                    : $@"{_initialControlValue} ({_valueTimerTick})";
        }
        else
        {
            _timer?.Stop();
            Dispose();
        }
    }

    private void SetDialogResult(DialogResult buttonDialogResult)
    {
        _returnResultAfterTimeout = _hasTimer
            ? _defaultResultAfterTimeout == DialogResult.None
                ? buttonDialogResult
                : _defaultResultAfterTimeout
            : DialogResult.None;

        DialogResult = _returnResultAfterTimeout;
    }

    private void SetButtons(
        MessageBoxButtons? messageBoxButtons,
        DialogResult defaultResultAfterTimeout)
    {
        _controlForValueTimerTick = _labelTimerTick;
        _defaultResultAfterTimeout = defaultResultAfterTimeout;

        if (!messageBoxButtons.HasValue)
        {
            return;
        }

        switch (messageBoxButtons)
        {
            case MessageBoxButtons.AbortRetryIgnore:
                AddButton(DialogResult.Abort, true);
                AddButton(DialogResult.Retry);
                AddButton(DialogResult.Ignore);
                SetDialogResult(DialogResult.Abort);
                break;

            case MessageBoxButtons.OK:
                AddButton(DialogResult.OK, true);
                SetDialogResult(DialogResult.OK);
                break;

            case MessageBoxButtons.OKCancel:
                AddButton(DialogResult.OK, true);
                AddButton(DialogResult.Cancel);
                SetDialogResult(DialogResult.OK);
                break;

            case MessageBoxButtons.RetryCancel:
                AddButton(DialogResult.Cancel, true);
                AddButton(DialogResult.Retry);
                SetDialogResult(DialogResult.Cancel);
                break;

            case MessageBoxButtons.YesNo:
                AddButton(DialogResult.Yes, true);
                AddButton(DialogResult.No);
                SetDialogResult(DialogResult.Yes);
                break;

            case MessageBoxButtons.YesNoCancel:
                AddButton(DialogResult.Yes, true);
                AddButton(DialogResult.No);
                AddButton(DialogResult.Cancel);
                SetDialogResult(DialogResult.Yes);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(messageBoxButtons), messageBoxButtons, null);
        }
    }

    private void SetIcon(MessageBoxIcon icon)
    {
        switch (icon)
        {
            case MessageBoxIcon.None:
                _pictureBox.Hide();
                break;
            case MessageBoxIcon.Exclamation:
                _pictureBox.Image = SystemIcons.Exclamation.ToBitmap();
                break;

            case MessageBoxIcon.Error:
                _pictureBox.Image = SystemIcons.Error.ToBitmap();
                break;

            case MessageBoxIcon.Information:
                _pictureBox.Image = SystemIcons.Information.ToBitmap();
                break;

            case MessageBoxIcon.Question:
                _pictureBox.Image = SystemIcons.Question.ToBitmap();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(icon), icon, null);
        }

        if (_pictureBox.Image == null)
        {
            return;
        }

        _pictureBox.Width = _pictureBox.Image.Width;
        _pictureBox.Height = _pictureBox.Image.Height;
    }

    private void SetSizeMessageBox()
    {
        const int MAX_STRING_LENGTH = 40;
        const int MIN_LINE_COUNT = 10;

        var message = _lblMessage.Text;

        var messageLength = _lblMessage.Text.Length > MAX_STRING_LENGTH
            ? MAX_STRING_LENGTH
            : _lblMessage.Text.Length;

        var measureSize = TextRenderer.MeasureText("W", Font);
        var sHeight = measureSize.Height;
        var width = messageLength * sHeight;

        var countLines = CountSplitByWidth(message, messageLength);
        var countEmptyLines = CountEmptyLines(message);
        var height = sHeight * (MIN_LINE_COUNT + countLines + countEmptyLines);

        Size = new Size(width, height);
    }

    private static int CountSplitByWidth(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        var result = new List<string>((text.Length / width) + 1);

        for (var i = 0; i < text.Length; i += width)
        {
            var len = Math.Min(width, text.Length - i);
            result.Add(text.Substring(i, len));
        }

        return result.Count;
    }

    private static int CountEmptyLines(string text)
    {
        var count = 0;

        for (var i = 0; i < text.Length - 1; i++)
        {
            if (text[i] == '\n' && text[i + 1] == '\n')
                count++;
        }

        return count;
    }

    private void AddButton(DialogResult buttonDialogResult, bool isDefault = false)
    {
        isDefault = _defaultResultAfterTimeout == DialogResult.None
            ? isDefault
            : _defaultResultAfterTimeout == buttonDialogResult;

        var text = isDefault && _hasTimer
            ? $@"{DialogResultButtonName[buttonDialogResult]} ({_timeout})"
            : DialogResultButtonName[buttonDialogResult];

        var button = new Button
        {
            Text = text,
            Tag = buttonDialogResult,
            Font = isDefault
                ? new Font("Microsoft Sans Serif", 8.25F,
                    FontStyle.Bold, GraphicsUnit.Point, 204)
                : new Font("Microsoft Sans Serif", 8.25F),
            BackColor = Color.FromArgb(225, 225, 225),
            Padding = new Padding(2),
            AutoSize = true,
            Height = 30
        };
        button.Click += ButtonClick;
        _flpButtons.Controls.Add(button);

        if (!isDefault && _hasTimer)
        {
            return;
        }

        _controlForValueTimerTick = button;
        _initialControlValue = DialogResultButtonName[buttonDialogResult];
    }

    private void ButtonClick(object? sender, EventArgs e)
    {
        if (sender is not Button btn)
        {
            return;
        }

        if (btn.Tag is not DialogResult dialogResult)
        {
            return;
        }

        DialogResult = dialogResult;

        Close();
    }

    private void UnsubscribeAllEvents()
    {
        foreach (Control flpButtonsControl in _flpButtons.Controls)
        {
            if (flpButtonsControl is not Button button)
            {
                return;
            }

            button.Click -= ButtonClick;
        }

        _timer?.Tick -= TimerOnTick;
        _timer?.Dispose();
    }
}
