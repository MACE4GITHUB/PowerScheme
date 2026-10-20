# PowerScheme

This app allows you to quickly select power plans. If necessary, you can restore the default settings for power schemes.

[Latest release](https://github.com/MACE4GITHUB/PowerScheme/releases/)

## Translations

Translations are plain XML files in the `lang` directory beside the executable.
To add a language, copy an existing XML file, translate its string values and change
the language element under `resources`, for example:

```xml
<language code="de">Deutsch</language>
```

The code must be unique and stable. The element text is the name shown in the language
menu; the filename can be anything ending in `.xml`. New files appear the next time
the language submenu opens, without restarting the application. Select the language
to load its strings. Missing strings fall back to the English translation, which
should remain available. Reselect a language or restart to reload edited strings.

All languages, including English and Russian, use the same loader and store their
string code in the `LanguageId` registry value (`REG_SZ`). Old numeric selections are
not migrated; select the language again to save its code. Without a saved selection,
the app matches the Windows culture against available languages, then falls back to
English or the first available translation. Invalid files are skipped or rejected
without replacing the current language; details are written to the error log.

## Startup Errors

Keep the `lang` directory beside the executable. For an unmerged build, keep its DLLs
as well; copying only the executable is not sufficient. Startup failures are recorded
in `%LOCALAPPDATA%\PowerScheme\errors.log`, including inner exceptions. The log is
limited to 1 MiB. A startup failure exits with code 1.
