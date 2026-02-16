namespace Languages;

public sealed class LanguageRu : Language
{
    public override string UpdateApp
        => "Обновить приложение";

    public override string UpdateAppToVersion
        => "Обновить приложение до версии";

    public override string Quit
        => "Выход";

    public override string StartupOnWindows
        => "Запускать вместе с Windows";

    public override string RestoreDefaultPowerSchemesName
        => "Восстановить схемы Windows по умолчанию";

    public override string RestoreDefaultPowerSchemesDescription
        => "Все типовые схемы будут удалены. Windows схемы не удаляются.";

    public override string Settings
        => "Настройки";

    public override string HibernateName
        => "Режим гибернации";

    public override string HibernateDescription
        => "Отображать в меню завершения работы";

    public override string SleepName
        => "Режим сна";

    public override string SleepDescription
        => "Отображать в меню завершения работы";

    public override string PowerOptions
        => "Панель электропитания Windows";

    public override string CreateTypicalSchemes
        => "Создать типовые схемы";

    public override string DeleteTypicalSchemes
        => "Удалить типовые схемы";

    public override string CreateStableScheme
        => "Создать стабильную схему";

    public override string DeleteStableScheme
        => "Удалить стабильную схему";

    public override string CreateMediaScheme
        => "Создать схему для просмотра фильмов";

    public override string DeleteMediaScheme
        => "Удалить схему для просмотра фильмов";

    public override string CreateSimpleScheme
        => "Создать схему энергосбережения";

    public override string DeleteSimpleScheme
        => "Удалить схему энергосбережения";

    public override string CreateExtremeScheme
        => "Создать схему экстремальной производительности";

    public override string DeleteExtremeScheme
        => "Удалить схему экстремальной производительности";

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

    public override string ExtremeName
        => "Экстремальная";

    public override string ExtremeDescription
        => "Максимальная производительность";

    public override string ShutDown
        => "Выключить компьютер";

    public override string DoNothing
        => "Действие не требуется";

    public override string WhenICloseTheLid
        => "Действие при закрытии крышки";


    public override string AlreadyRunning
        => "Приложение уже было запущено.";

    public override string Error
        => "Ошибка";

    public override string Information
        => "Информация";

    public override string CannotGetAdministratorRights
        => "Приложению не удается получить права администратора. \n\nСм. свою политику доступа.";

    public override string ApplicationLatter
        => "Приложение не может работать в текущей версии Windows. " +
           "\n\nТребуется ОС Windows Vista/7/10.";

    public override string FirstStartCaption
        => "Первый запуск";

    public override string FirstStartDescription
        => $"Создать типовые схемы электропитания ({StableName}, {MediaName}, {SimpleName})? " +
           $"\n\n {StableName} - схема на основе Высокая производительность.\n Частота постоянная. \n Для повседневных не критических задач." +
           $"\n\n {MediaName} - схема на основе Сбалансированная. \n Частота переменная. \n Для просмотра видео, прослушивания аудио." +
           $"\n\n {SimpleName} - схема на основе Экономия энергии. \n Частота переменная, преимущественно низкая. \n Для просмотра интернет." +
           $"\n\n При нажатии \"Нет\" типовые схемы можно будет создать позже из меню \"Настройки\".";

    public override string None
        => "Ничего";

    public override string Ok
        => "ОК";

    public override string Cancel
        => "Отмена";

    public override string Abort
        => "Прервать";

    public override string Retry
        => "Повторить";

    public override string Ignore
        => "Игнорировать";

    public override string Yes
        => "Да";

    public override string No
        => "Нет";

    public override string RunAsAdministrator
        => "Нет разрешений для управления настройками электропитания. \n\nЗапустите приложение один раз как администратор для установки разрешений.";

    public override string SwitchPowerSchemeWhenIdle
        => "Действие при простое";
}
