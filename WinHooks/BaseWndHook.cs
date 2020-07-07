using System;

namespace WinHooks
{
    public abstract class BaseWndHook
    {
        private bool _disposed;

        protected abstract void UnHookWindowsHook();

        /// <summary>
        /// Releases the win hook.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;
            if (disposing) GC.SuppressFinalize(this);

            UnHookWindowsHook();
        }

        ~BaseWndHook()
        {
            Dispose(false);
        }
    }
}
