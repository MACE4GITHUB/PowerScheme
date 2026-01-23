using System.Globalization;

namespace Languages;

public class Language
{
    public static Language Current { get; } = GetLanguage();

    public static Language GetLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture.Name;

        if (currentCulture == "ru-RU")
        {
            return new LanguageRu();
        }

        return new Language();
    }

    public virtual string Exit
        => "Exit";

    public virtual string StartupOnWindows
        => "Run automatically at startup in Windows";

    public virtual string RestoreDefaultPowerSchemesName
        => "Restore default Windows PowerSchemes";

    public virtual string RestoreDefaultPowerSchemesDescription
        => "All other schemes will be deleted";

    public virtual string Settings
        => "Settings";

    public virtual string HibernateName
        => "Hibernate";

    public virtual string HibernateDescription
        => "Show in Power menu";

    public virtual string SleepName
        => "Sleep";

    public virtual string SleepDescription
        => "Show in Power menu";

    public virtual string PowerOptions
        => "Power Options";

    public virtual string CreateTypicalSchemes
        => "Create typical schemes";

    public virtual string DeleteTypicalSchemes
        => "Delete typical schemes";

    public virtual string CreateStableScheme
        => "Create stable scheme";

    public virtual string DeleteStableScheme
        => "Delete stable scheme";

    public virtual string CreateMediaScheme
        => "Create media scheme";

    public virtual string DeleteMediaScheme
        => "Delete media scheme";

    public virtual string CreateSimpleScheme
        => "Create simple scheme";

    public virtual string DeleteSimpleScheme
        => "Delete simple scheme";

    public virtual string CreateExtremeScheme
        => "Create extreme scheme";

    public virtual string DeleteExtremeScheme
        => "Delete extreme scheme";

    public virtual string MediaName
        => "Media";

    public virtual string MediaDescription
        => "Show media (video, etc.)";

    public virtual string StableName
        => "Stable";

    public virtual string StableDescription
        => "Constant CPU Speed";

    public virtual string SimpleName
        => "Simple";

    public virtual string SimpleDescription
        => "Power saver";

    public virtual string ExtremeName
        => "Extreme";

    public virtual string ExtremeDescription
        => "Ultimate CPU Speed";

    public virtual string ShutDown
        => "Shut down";

    public virtual string DoNothing
        => "Do nothing";

    public virtual string WhenICloseTheLid
        => "When I close the lid";

    public virtual string AlreadyRunning
        => "Another instance is already running.";

    public virtual string Error
        => "Error";

    public virtual string Information
        => "Information";

    public virtual string CannotGetAdministratorRights
        => "This application cannot get administrator rights. \n\nSee yours access policy.";

    public virtual string ApplicationLatter
        => "This application does not run on the current Windows version. \n\nWorks it on Vista and latter.";

    public virtual string FirstStartCaption
        => "First start";

    public virtual string FirstStartDescription
        => $"Create typical power schemes {StableName}, {MediaName}, {SimpleName}?";

    public virtual string None
        => "None";

    public virtual string Ok
        => "OK";

    public virtual string Cancel
        => "Cancel";

    public virtual string Abort
        => "Abort";

    public virtual string Retry
        => "Retry";

    public virtual string Ignore
        => "Ignore";

    public virtual string Yes
        => "Yes";

    public virtual string No
        => "No";
}
