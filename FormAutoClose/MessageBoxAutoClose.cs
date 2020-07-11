namespace FormAutoClose
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using static NativeMethods;

    /// <summary>
    /// Represents Auto-close form
    /// </summary>
    public sealed class MessageBoxAutoClose : IDisposable, IFormAutoClose
    {
        private System.Threading.Timer _timeoutTimer;
        private const int WM_CLOSE = 0x0010;
        private readonly string _text;
        private readonly string _caption;
        private readonly int _timeout;

        /// <summary>
        /// Creates Auto-close form
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="timeout">Timeout in seconds</param>
        public MessageBoxAutoClose(string text, string caption, int timeout)
        {
            var currentProcessId = Process.GetCurrentProcess().Id;
            _text = text;
            _caption = $"{caption} [Id {currentProcessId}]";
            _timeout = timeout;
        }

        /// <summary>
        /// Shows Auto-close form
        /// </summary>
        public void Show()
        {
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, _timeout * 1000, System.Threading.Timeout.Infinite);
            MessageBox.Show(_text, _caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnTimerElapsed(object state)
        {
            var mbWnd = FindWindow(null, _caption);
            if (mbWnd == IntPtr.Zero) return;

            SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _timeoutTimer.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}
