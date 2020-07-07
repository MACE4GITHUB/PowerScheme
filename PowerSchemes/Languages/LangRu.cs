namespace PowerSchemes.Languages
{
    public sealed class LangRu : Lang
    {
        public override string Exit
            => "Выход";

        public override string StartupOnWindows
            => "Запускать вместе с Windows";

        public override string RestoreDefaultPowerSchemesName
            => "Восстановить схемы Windows по умолчанию";

        public override string RestoreDefaultPowerSchemesDescription
            => "Все типовые схемы будут удалены. Windows схемы не удаляются.";

        public override string Settings
            => "Настройки";

        public override string ShowHibernateOptionName
            => "Режим гибернации";

        public override string ShowHibernateOptionDescription
            => "Отображать в меню завершения работы";

        public override string ShowSleepOptionName
            => "Режим сна";

        public override string ShowSleepOptionDescription
            => "Отображать в меню завершения работы";

        public override string ShowCplName
            => "Панель Электропитания Windows";

        public override string CreateTypicalSchemes
            => "Создать типовые схемы";

        public override string DeleteTypicalSchemes
            => "Удалить типовые схемы";

        public override string CreateStableScheme
            => "Создать стабильную схему";

        public override string CreateMediaScheme
            => "Создать схему для просмотра фильмов";

        public override string CreateSimpleScheme
            => "Создать схему энергосбережения";

        public override string MediaName
            => "Медиа";

        public override string MediaDescription
            => "Просмотр фильмов";

        public override string StableName
            => "Стабильная";

        public override string StableDescription
            => "Постоянная скорость";

        public override string SimpleName
            => "Легкая";

        public override string SimpleDescription
            => "Энергосбережение";
    }
}
