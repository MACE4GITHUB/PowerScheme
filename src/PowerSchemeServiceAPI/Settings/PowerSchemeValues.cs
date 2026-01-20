using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeValues(
    Setting setting,
    int dCSettings,
    int aCSettings) :
    PowerSchemeDcAcValues(dCSettings, aCSettings)
{
    public Setting Setting { get; } = setting;
}
