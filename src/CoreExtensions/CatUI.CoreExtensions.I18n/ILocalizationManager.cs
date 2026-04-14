using System.Globalization;

namespace CatUI.CoreExtensions.I18n;

public interface ILocalizationManager
{
    /// <summary>
    /// Represents the language code of the current language (the one from <see cref="CultureInfo.Name"/>).
    /// </summary>
    string LanguageCode { get; }

    string? GetString(string key, params object[] args);

    string? GetStringPlural(string key, string pluralKey, long count, params object[] args);

    string? GetStringCtx(string key, string context, params object[] args);

    string? GetStringPluralCtx(string key, string pluralKey, string context, long count, params object[] args);
}
