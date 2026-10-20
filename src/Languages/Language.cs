using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Languages;

public class Language
{
    private static readonly List<string> _ruLanguages = ["ru", "ru-ru", "be", "be-by"];
    private readonly string _code;
    private readonly Lazy<IReadOnlyDictionary<string, string>> _strings;

    private Language(LanguageDefinition definition)
    {
        _code = definition.Code;
        _strings = definition.Strings;
    }

    private string GetString(string name) =>
        _strings.Value.TryGetValue(name, out var value) ? value : LanguageResources.GetDefault(name);

    public static Language Current { get; private set; } = GetLanguage();

    public static string Code => Current._code;

    public static IReadOnlyList<LanguageDefinition> GetAvailableLanguages(string? baseDirectory = null, Action<string>? reportError = null) =>
        LanguageResources.Discover(baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory, reportError);

    public static void SetLanguage(string code, string? baseDirectory = null)
    {
        var definition = GetAvailableLanguages(baseDirectory).FirstOrDefault(
            language => string.Equals(language.Code, code, StringComparison.OrdinalIgnoreCase));
        if (definition == null)
        {
            throw new ArgumentException($"Translation not found: {code}", nameof(code));
        }
        SetLanguage(definition);
    }

    public static void SetLanguage(LanguageDefinition definition)
    {
        _ = definition.Strings.Value;
        Current = new Language(definition);
    }

    public static Language GetLanguage(string? locale = null, string? baseDirectory = null)
    {
        locale ??= CultureInfo.CurrentCulture.Name;
        var languages = GetAvailableLanguages(baseDirectory);
        var definition = languages.FirstOrDefault(language => string.Equals(language.Code, locale, StringComparison.OrdinalIgnoreCase))
            ?? languages.FirstOrDefault(language => string.Equals(language.Code, locale.Split('-')[0], StringComparison.OrdinalIgnoreCase))
            ?? (IsRuLocale(locale) ? languages.FirstOrDefault(language => language.Code == "ru") : null)
            ?? languages.FirstOrDefault(language => language.Code == "en")
            ?? languages.FirstOrDefault()
            ?? throw new InvalidOperationException("No valid translations found in the lang directory.");
        return new Language(definition);
    }

    public static bool IsRuLocale(string locale) =>
        !string.IsNullOrWhiteSpace(locale) &&
        _ruLanguages.Contains(locale.Trim().ToLowerInvariant());

    public virtual string UpdateApp => GetString(nameof(UpdateApp));
    public virtual string NewVersionIsAvailable => GetString(nameof(NewVersionIsAvailable));
    public virtual string UpdateAppToVersion => GetString(nameof(UpdateAppToVersion));
    public virtual string Version => GetString(nameof(Version));
    public virtual string Quit => GetString(nameof(Quit));
    public virtual string StartupOnWindows => GetString(nameof(StartupOnWindows));
    public virtual string RestoreDefaultPowerSchemesName => GetString(nameof(RestoreDefaultPowerSchemesName));
    public virtual string RestoreDefaultPowerSchemesDescription => GetString(nameof(RestoreDefaultPowerSchemesDescription));
    public virtual string Settings => GetString(nameof(Settings));
    public virtual string HibernateName => GetString(nameof(HibernateName));
    public virtual string HibernateDescription => GetString(nameof(HibernateDescription));
    public virtual string SleepName => GetString(nameof(SleepName));
    public virtual string SleepDescription => GetString(nameof(SleepDescription));
    public virtual string PowerOptions => GetString(nameof(PowerOptions));
    public virtual string CreateTypicalSchemes => GetString(nameof(CreateTypicalSchemes));
    public virtual string DeleteTypicalSchemes => GetString(nameof(DeleteTypicalSchemes));
    public virtual string CreateStableScheme => GetString(nameof(CreateStableScheme));
    public virtual string DeleteStableScheme => GetString(nameof(DeleteStableScheme));
    public virtual string CreateMediaScheme => GetString(nameof(CreateMediaScheme));
    public virtual string DeleteMediaScheme => GetString(nameof(DeleteMediaScheme));
    public virtual string CreateSimpleScheme => GetString(nameof(CreateSimpleScheme));
    public virtual string DeleteSimpleScheme => GetString(nameof(DeleteSimpleScheme));
    public virtual string CreateExtremeScheme => GetString(nameof(CreateExtremeScheme));
    public virtual string DeleteExtremeScheme => GetString(nameof(DeleteExtremeScheme));
    public virtual string AllPowerSchemes => GetString(nameof(AllPowerSchemes));
    public virtual string MediaName => GetString(nameof(MediaName));
    public virtual string MediaDescription => GetString(nameof(MediaDescription));
    public virtual string StableName => GetString(nameof(StableName));
    public virtual string StableDescription => GetString(nameof(StableDescription));
    public virtual string SimpleName => GetString(nameof(SimpleName));
    public virtual string SimpleDescription => GetString(nameof(SimpleDescription));
    public virtual string ExtremeName => GetString(nameof(ExtremeName));
    public virtual string ExtremeDescription => GetString(nameof(ExtremeDescription));
    public virtual string HighName => GetString(nameof(HighName));
    public virtual string HighDescription => GetString(nameof(HighDescription));
    public virtual string BalanceName => GetString(nameof(BalanceName));
    public virtual string BalanceDescription => GetString(nameof(BalanceDescription));
    public virtual string LowName => GetString(nameof(LowName));
    public virtual string LowDescription => GetString(nameof(LowDescription));
    public virtual string ShutDown => GetString(nameof(ShutDown));
    public virtual string DoNothing => GetString(nameof(DoNothing));
    public virtual string WhenICloseTheLid => GetString(nameof(WhenICloseTheLid));
    public virtual string AlreadyRunning => GetString(nameof(AlreadyRunning));
    public virtual string Error => GetString(nameof(Error));
    public virtual string Information => GetString(nameof(Information));
    public virtual string CannotGetAdministratorRights => GetString(nameof(CannotGetAdministratorRights));
    public virtual string ApplicationLatter => GetString(nameof(ApplicationLatter));
    public virtual string FirstStartCaption => GetString(nameof(FirstStartCaption));
    public virtual string FirstStartDescription => GetString(nameof(FirstStartDescription));
    public virtual string Apply => GetString(nameof(Apply));
    public virtual string Default => GetString(nameof(Default));
    public virtual string None => GetString(nameof(None));
    public virtual string Ok => GetString(nameof(Ok));
    public virtual string Cancel => GetString(nameof(Cancel));
    public virtual string Abort => GetString(nameof(Abort));
    public virtual string Retry => GetString(nameof(Retry));
    public virtual string Ignore => GetString(nameof(Ignore));
    public virtual string Yes => GetString(nameof(Yes));
    public virtual string No => GetString(nameof(No));
    public virtual string RunAsAdministrator => GetString(nameof(RunAsAdministrator));
    public virtual string SwitchPowerSchemeWhenIdle => GetString(nameof(SwitchPowerSchemeWhenIdle));
    public virtual string PowerSchemes => GetString(nameof(PowerSchemes));
    public virtual string KeepBrightness => GetString(nameof(KeepBrightness));
    public virtual string KeepBrightnessDescription => GetString(nameof(KeepBrightnessDescription));
    public virtual string DeleteCustomScheme => GetString(nameof(DeleteCustomScheme));
    public virtual string IdleSleepOptions => GetString(nameof(IdleSleepOptions));
    public virtual string IdleDisplayOptions => GetString(nameof(IdleDisplayOptions));
    public virtual string TurnOffDisplay => GetString(nameof(TurnOffDisplay));
    public virtual string TurnOffLockedDisplay => GetString(nameof(TurnOffLockedDisplay));
    public virtual string SyncAcDc => GetString(nameof(SyncAcDc));
    public virtual string IdleOptions => GetString(nameof(IdleOptions));
    public virtual string IdleThreshold => GetString(nameof(IdleThreshold));
    public virtual string PollingActiveTime => GetString(nameof(PollingActiveTime));
    public virtual string PollingIdleTime => GetString(nameof(PollingIdleTime));
    public virtual string Hours => GetString(nameof(Hours));
    public virtual string Minutes => GetString(nameof(Minutes));
    public virtual string Seconds => GetString(nameof(Seconds));
    public virtual string TheUserIdleTimeAfterWhichThePowerPlanWillChange => GetString(nameof(TheUserIdleTimeAfterWhichThePowerPlanWillChange));
    public virtual string MonitoringFrequencyWhenTheUserIsActive => GetString(nameof(MonitoringFrequencyWhenTheUserIsActive));
    public virtual string MonitoringFrequencyDuringUserInactivity => GetString(nameof(MonitoringFrequencyDuringUserInactivity));
    public virtual string Themes => GetString(nameof(Themes));
    public virtual string Dark => GetString(nameof(Dark));
    public virtual string Light => GetString(nameof(Light));
    public virtual string Blue => GetString(nameof(Blue));
    public virtual string Green => GetString(nameof(Green));
    public virtual string SelectLanguage => GetString(nameof(SelectLanguage));
}
