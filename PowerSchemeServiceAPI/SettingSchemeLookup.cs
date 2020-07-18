using Common;
using PowerSchemeServiceAPI.Model;
using System;
using System.Collections.Generic;

namespace PowerSchemeServiceAPI
{
    internal static class SettingSchemeLookup
    {
        internal static readonly Dictionary<SettingScheme, IPowerScheme> SettingSchemes
            = new Dictionary<SettingScheme, IPowerScheme>
            {
                { SettingScheme.High, new PowerScheme(new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"),true,ImageItem.High) },
                { SettingScheme.Balance, new PowerScheme(new Guid("381b4222-f694-41f0-9685-ff5bb260df2e"),true,ImageItem.Balance ) },
                { SettingScheme.Low, new PowerScheme(new Guid("a1841308-3541-4fab-bc81-f71556f20b4a"),true,ImageItem.Low) },
                { SettingScheme.Ultimate, new PowerScheme(new Guid("e9a42b02-d5df-448d-aa00-03f14749eb61"), true, ImageItem.Ultimate, false, true) },
                { SettingScheme.Stable, new PowerScheme(new Guid("fa0cd8f1-1300-4710-820b-00e8e75f31f8"), false, ImageItem.Stable) },
                { SettingScheme.Media, new PowerScheme(new Guid("fcab38a3-7e4c-4a75-8483-f522befb9c58"), false, ImageItem.Media) },
                { SettingScheme.Simple, new PowerScheme(new Guid("fa8d915c-65de-4bba-9569-3c2e77ea68b6"), false, ImageItem.Simple) },
                { SettingScheme.Extreme, new PowerScheme(new Guid("f384acfa-ed71-4607-bf8e-747d56402f0c"), false, ImageItem.Extreme, true, true) }
            };

        internal enum SettingScheme
        {
            High,
            Balance,
            Low,
            Ultimate,
            Stable,
            Media,
            Simple,
            Extreme
        }
    }
}
