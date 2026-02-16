using System;
using System.Windows.Forms;

namespace PowerScheme.Timers;

internal abstract class BaseTimer : IDisposable
{
    private readonly Timer _timer;
    private bool _disposedValue;
    private bool _success = false;

    protected BaseTimer(int intervalInMilliseconds)
    {
        _timer = new Timer
        {
            Interval = intervalInMilliseconds
        };
        _timer.Tick += TimerOnTick;
    }

    public int IntervalInMilliseconds
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public void Start() =>
        _timer.Start();

    public void Stop() =>
        _timer.Stop();

    public bool IsRunning => _timer.Enabled;

    public virtual void BeforeAction() { }

    public abstract void Action();

    public virtual void AfterAction() { }

    public void SetStopAfterTick()
    {
        _success = true;
    }

    private void TimerOnTick(object sender, EventArgs e)
    {
        if (_success)
        {
            _timer.Stop();
            return;
        }

        BeforeAction();

        Action();

        AfterAction();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _timer.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
