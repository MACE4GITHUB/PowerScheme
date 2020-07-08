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

        public override string Hibernate
            => "Режим гибернации";

        public override string HibernateDescription
            => "Отображать в меню завершения работы";

        public override string Sleep
            => "Режим сна";

        public override string SleepDescription
            => "Отображать в меню завершения работы";

        public override string PowerOptions
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

        public override string ShutDown
            => "Выключить компьютер";

        public override string DoNothing
            => "Действие не требуется";

        public override string WhenICloseTheLid
            => "Действие при закрытии крышки";
    }
}
