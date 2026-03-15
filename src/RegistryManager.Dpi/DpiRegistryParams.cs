using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using RegistryManager.Common;
using static RegistryManager.Api.Registry;

namespace RegistryManager.Dpi;

public static class DpiRegistryParams
{
    private const string SCALE_FACTORS = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers\ScaleFactors";
    private const string DPI_VALUE = "DpiValue";

    public static RegistryParam GetScaleFactors =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = SCALE_FACTORS
        };

    public static List<RegistryParam> GetMonitorRegistryParams() => [..GetMonitorIds()
        .Select(x => new RegistryParam {
            RegistryHive = RegistryHive.LocalMachine,
            Path = SCALE_FACTORS,
            Section = x,
            Name = DPI_VALUE
        })];

    private static ICollection<string> GetMonitorIds() =>
        GetSubKeys(GetScaleFactors);
}
