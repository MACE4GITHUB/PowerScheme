using System;
using System.Windows.Forms;
using static System.Threading.Timeout;
using static MessageForm.NativeMethods;

namespace MessageForm;

/// <summary>
/// Represents Auto-close form
/// </summary>
public sealed class MessageBoxAutoClose : IMainMessageBox
{
    private System.Threading.Timer _timeoutTimer;
    private const int WM_CLOSE = 0x0010;
    private string _text;
    private string _title;
    private int _timeout;
    private MessageBoxIcon _messageBoxIcon;
    private MessageBoxButtons? _messageBoxButtons;

    public event EventHandler Closed;

    public MessageBoxAutoClose()
    {
        Closed += OnClosed;
    }

    public DialogResult Show(
        string message,
        string title = null,
        MessageBoxButtons? buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.Asterisk,
        bool isApplicationExit = false,
        int timeout = 0,
        DialogResult defaultResultAfterTimeout = DialogResult.None)
    {
        var currentProcessId = Environment.ProcessId;
        _text = message;
        _title = $"{title} [Id {currentProcessId}]";
        _timeout = timeout == 0 ? 1000 : timeout;
        _messageBoxIcon = icon;
        _messageBoxButtons = buttons;

        _timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, _timeout * 1000, Infinite);

        var result = MessageBox.Show(_text, _title, _messageBoxButtons ?? MessageBoxButtons.OK, _messageBoxIcon);

        if (isApplicationExit)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    private void OnTimerElapsed(object state)
    {
        var mbWnd = FindWindow(null, _title);
        if (mbWnd == IntPtr.Zero)
        {
            return;
        }

        SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    }

    private static void OnClosed(object sender, EventArgs args)
    {
        Environment.Exit(-1);
    }

    public void Dispose()
    {
        Closed -= OnClosed;
        _timeoutTimer?.Dispose();
    }
}
