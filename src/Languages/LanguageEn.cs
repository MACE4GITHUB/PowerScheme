namespace Languages;

public sealed class LanguageEn : Language
{
    public override string UpdateApp
        => "Update the app";

    public override string UpdateAppToVersion
        => "Update the app to version";

    public override string Quit
        => "Quit";

    public override string StartupOnWindows
        => "Run automatically at startup in Windows";

    public override string RestoreDefaultPowerSchemesName
        => "Restore default Windows PowerSchemes";

    public override string RestoreDefaultPowerSchemesDescription
        => "All other schemes will be deleted";

    public override string Settings
        => "Settings";

    public override string HibernateName
        => "Hibernate";

    public override string HibernateDescription
        => "Show in Power menu";

    public override string SleepName
        => "Sleep";

    public override string SleepDescription
        => "Show in Power menu";

    public override string PowerOptions
        => "Power Options";

    public override string CreateTypicalSchemes
        => "Create typical schemes";

    public override string DeleteTypicalSchemes
        => "Delete typical schemes";

    public override string CreateStableScheme
        => "Create stable scheme";

    public override string DeleteStableScheme
        => "Delete stable scheme";

    public override string CreateMediaScheme
        => "Create media scheme";

    public override string DeleteMediaScheme
        => "Delete media scheme";

    public override string CreateSimpleScheme
        => "Create simple scheme";

    public override string DeleteSimpleScheme
        => "Delete simple scheme";

    public override string CreateExtremeScheme
        => "Create extreme scheme";

    public override string DeleteExtremeScheme
        => "Delete extreme scheme";

    public override string MediaName
        => "Media";

    public override string MediaDescription
        => "Show media (video, etc.)";

    public override string StableName
        => "Stable";

    public override string StableDescription
        => "Constant CPU Speed";

    public override string SimpleName
        => "Simple";

    public override string SimpleDescription
        => "Power saver";

    public override string ExtremeName
        => "Extreme";

    public override string ExtremeDescription
        => "Ultimate CPU Speed";

    public override string ShutDown
        => "Shut down";

    public override string DoNothing
        => "Do nothing";

    public override string WhenICloseTheLid
        => "When I close the lid";

    public override string AlreadyRunning
        => "Another instance is already running.";

    public override string Error
        => "Error";

    public override string Information
        => "Information";

    public override string CannotGetAdministratorRights
        => "This application cannot get administrator rights. \n\nSee yours access policy.";

    public override string ApplicationLatter
        => "This application does not run on the current Windows version. \n\nWorks it on Vista and latter.";

    public override string FirstStartCaption
        => "First start";

    public override string FirstStartDescription
        => $"Create typical power schemes {StableName}, {MediaName}, {SimpleName}?";

    public override string None
        => "None";

    public override string Ok
        => "OK";

    public override string Cancel
        => "Cancel";

    public override string Abort
        => "Abort";

    public override string Retry
        => "Retry";

    public override string Ignore
        => "Ignore";

    public override string Yes
        => "Yes";

    public override string No
        => "No";
}
