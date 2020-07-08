using System.Globalization;

namespace PowerSchemes.Languages
{
    public class Lang
    {
        public static Lang SetLanguage()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            if (currentCulture == "ru-RU")
            {
                return new LangRu();
            }

            return new Lang();
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

        public virtual string Hibernate
            => "Hibernate";

        public virtual string HibernateDescription
            => "Show in Power menu";

        public virtual string Sleep
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

        public virtual string CreateMediaScheme
            => "Create media scheme";

        public virtual string CreateSimpleScheme
            => "Create simple scheme";

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

        public virtual string ShutDown
            => "Shut down";

        public virtual string DoNothing
            => "Do nothing";

        public virtual string WhenICloseTheLid
            => "When I close the lid";
    }
}
