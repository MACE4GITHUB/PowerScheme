using System;
using PowerScheme.EventArguments;
using PowerScheme.Services;

namespace PowerScheme.Timers;

internal sealed class UpdateTimer(
    IUpdateService updateService) :
    BaseTimer((int)TimeSpan.FromSeconds(5).TotalMilliseconds)
{
    private readonly IUpdateService _updateService = updateService;

    public override async void Action()
    {
        var releaseInfo = await _updateService.GetReleaseInfoAsync();

        if (releaseInfo.NewVersionAvailable)
        {
            OnNotifyUpdate(new UpdateEventArgs(releaseInfo));
        }
    }

    public override void AfterAction()
    {
        IntervalInMilliseconds = (int)TimeSpan.FromHours(3).TotalMilliseconds;
    }

    public event EventHandler<UpdateEventArgs>? NotifyUpdate;

    private void OnNotifyUpdate(UpdateEventArgs e)
    {
        NotifyUpdate?.Invoke(this, e);
    }
}
