using System.Collections.Generic;
using System.Globalization;

namespace Languages;

public abstract class Language
{
    private static readonly List<string> _ruLanguages = ["ru", "ru-ru", "be", "be-by"];

    public static Language Current { get; } = GetLanguage();

    public static Language GetLanguage() =>
        IsRuLocale(CultureInfo.CurrentCulture.Name)
            ? new LanguageRu()
            : new LanguageEn();

    public static bool IsRuLocale(string locale) =>
        !string.IsNullOrWhiteSpace(locale) &&
        _ruLanguages.Contains(locale.Trim().ToLowerInvariant());

    public abstract string UpdateApp {  get; }
    public abstract string UpdateAppToVersion {  get; }
    public abstract string Quit {  get; }
    public abstract string StartupOnWindows { get; }
    public abstract string RestoreDefaultPowerSchemesName { get; }
    public abstract string RestoreDefaultPowerSchemesDescription { get; }
    public abstract string Settings { get; }
    public abstract string HibernateName { get; }
    public abstract string HibernateDescription { get; }
    public abstract string SleepName { get; }
    public abstract string SleepDescription { get; }
    public abstract string PowerOptions { get; }
    public abstract string CreateTypicalSchemes { get; }
    public abstract string DeleteTypicalSchemes { get; }
    public abstract string CreateStableScheme { get; }
    public abstract string DeleteStableScheme { get; }
    public abstract string CreateMediaScheme { get; }
    public abstract string DeleteMediaScheme { get; }
    public abstract string CreateSimpleScheme { get; }
    public abstract string DeleteSimpleScheme { get; }
    public abstract string CreateExtremeScheme { get; }
    public abstract string DeleteExtremeScheme { get; }
    public abstract string MediaName { get; }
    public abstract string MediaDescription { get; }
    public abstract string StableName { get; }
    public abstract string StableDescription { get; }
    public abstract string SimpleName { get; }
    public abstract string SimpleDescription { get; }
    public abstract string ExtremeName { get; }
    public abstract string ExtremeDescription { get; }
    public abstract string ShutDown { get; }
    public abstract string DoNothing { get; }
    public abstract string WhenICloseTheLid { get; }
    public abstract string AlreadyRunning { get; }
    public abstract string Error { get; }
    public abstract string Information { get; }
    public abstract string CannotGetAdministratorRights { get; }
    public abstract string ApplicationLatter { get; }
    public abstract string FirstStartCaption { get; }
    public abstract string FirstStartDescription { get; }
    public abstract string None { get; }
    public abstract string Ok { get; }
    public abstract string Cancel { get; }
    public abstract string Abort { get; }
    public abstract string Retry { get; }
    public abstract string Ignore { get; }
    public abstract string Yes { get; }
    public abstract string No { get; }
    public abstract string RunAsAdministrator { get; }
}
