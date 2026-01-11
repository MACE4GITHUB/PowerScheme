using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static MessageForm.DialogResultLookup;

namespace MessageForm;

public partial class MainMessageBox : Form, IMainMessageBox
{
    private int _timeout;
    private readonly IWin32Window _owner;
    private Timer? _timer;
    private int _valueTimerTick;
    private Control? _controlForValueTimerTick;
    private string _initialControlValue = "";
    private DialogResult _defaultResultAfterTimeout;
    private DialogResult _returnResultAfterTimeout;
    private bool _hasTimer;

    public MainMessageBox()
    {
        InitializeComponent();
        _labelTimerTick.Text = "";
        var handle = Process.GetCurrentProcess().MainWindowHandle;
        _owner = new WindowWrapper(handle);
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
            FormClosed += (sender, args) => Environment.Exit(0);
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

    private void TimerOnTick(object sender, EventArgs e)
    {
        _valueTimerTick--;

        if (_valueTimerTick > 0)
        {
            _controlForValueTimerTick.Text =
                string.IsNullOrWhiteSpace(_initialControlValue)
                    ? $@"{_valueTimerTick}"
                    : $@"{_initialControlValue} ({_valueTimerTick})";
        }
        else
        {
            _timer.Stop();
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

    private void SetButtons(MessageBoxButtons? messageBoxButtons,
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

        _pictureBox.Width = _pictureBox.Image.Width;
        _pictureBox.Height = _pictureBox.Image.Height;
    }

    private void SetSizeMessageBox()
    {
        var message = _lblMessage.Text;
        if (message.Length < 60)
        {
            Size = new Size(350, 170);
            return;
        }
        var height = 125;

        SizeF size = TextRenderer.MeasureText("W", 
            new Font(new FontFamily("Microsoft Sans Serif"),
            8.25f,
            0,
            (GraphicsUnit)3));
        var sHeight = (int)(size.Height * 1.1);
        var sWidth = (int)(_lblMessage.Width / size.Width * 3);

        var groups = (from Match m in Regex.Matches(message, ".{1," + sWidth + "}") select m.Value).ToArray();
        var groupsInline = (from Match m in MyRegex().Matches(message) select m.Value).ToArray();
        var lines = groups.Length;
        var fHeight = sHeight * groupsInline.Length * 1;

        height += (sHeight * lines) + fHeight - (int)(groupsInline.Length * 1.5);

        Size = new Size(Width, height);
    }

    private void AddButton(DialogResult buttonDialogResult, bool isDefault = false)
    {
        isDefault = _defaultResultAfterTimeout == DialogResult.None
            ? isDefault
            : _defaultResultAfterTimeout == buttonDialogResult;

        var text = isDefault && _hasTimer
            ? $@"{DialogResultScheme[buttonDialogResult]} ({_timeout})"
            : DialogResultScheme[buttonDialogResult];

        var button = new Button
        {
            Text = text,
            Tag = buttonDialogResult,
            Font = isDefault
                ? new Font("Microsoft Sans Serif", 8.25F,
                    FontStyle.Bold, GraphicsUnit.Point, (byte)204)
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
        _initialControlValue = DialogResultScheme[buttonDialogResult];
    }

    private void ButtonClick(object sender, EventArgs e)
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

    [GeneratedRegex("\n\n{1,1}")]
    private static partial Regex MyRegex();
}
