namespace MessageForm;

using System;
using System.Windows.Forms;

public interface IMainMessageBox: IDisposable
{
    /// <summary>
    /// Shows message form.
    /// <para>timeout = 0 is infinite</para>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="buttons"></param>
    /// <param name="icon"></param>
    /// <param name="isApplicationExit"></param>
    /// <param name="timeout"></param>
    /// <param name="defaultResultAfterTimeout"></param>
    /// <returns></returns>
    DialogResult Show(string message, 
        string? title = null,
        MessageBoxButtons? buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.Information,
        bool isApplicationExit = false,
        int timeout = 0,
        DialogResult defaultResultAfterTimeout = DialogResult.None);
}
