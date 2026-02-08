using System;
using Updater.Common;

namespace PowerScheme.EventArguments;

public class UpdateEventArgs(ReleaseInfo releaseInfo) : EventArgs
{
    public ReleaseInfo ReleaseInfo { get; } = releaseInfo;
}
