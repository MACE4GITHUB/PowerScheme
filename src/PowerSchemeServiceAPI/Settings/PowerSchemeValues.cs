using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeValues : PowerSchemeDCACValues
{
    public PowerSchemeValues(Setting setting, int dCSettings, int aCSettings) : 
        base(dCSettings, aCSettings)
    {
        Setting = setting;
    }

    public Setting Setting { get; }
}