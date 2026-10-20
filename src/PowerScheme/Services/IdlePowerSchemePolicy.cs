using System;

namespace PowerScheme.Services;

internal static class IdlePowerSchemePolicy
{
    internal static Guid? GetMainPowerSchemeUpdate(
        Guid previousPowerSchemeId,
        Guid activePowerSchemeId,
        Guid idlePowerSchemeId,
        Guid mainPowerSchemeId)
    {
        if (idlePowerSchemeId == Guid.Empty)
        {
            return null;
        }

        var mainSchemeId = activePowerSchemeId == idlePowerSchemeId &&
                           previousPowerSchemeId != idlePowerSchemeId
            ? previousPowerSchemeId
            : activePowerSchemeId;

        return mainSchemeId != mainPowerSchemeId ? mainSchemeId : null;
    }
}
