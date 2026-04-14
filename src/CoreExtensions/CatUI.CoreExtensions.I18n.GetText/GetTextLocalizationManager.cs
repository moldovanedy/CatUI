using System;
using System.Globalization;
using CatUI.Data.Assets;
using GetText;

namespace CatUI.CoreExtensions.I18n.GetText;

public class GetTextLocalizationManager : ILocalizationManager
{
    public string LanguageCode { get; }

    private readonly Catalog _strCatalog;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="languageCode"></param>
    /// <param name="i18Asset">
    /// The internationalization asset. It must have a non-null <see cref="I18NStreamAsset.MemStream"/> property.
    /// </param>
    public GetTextLocalizationManager(string languageCode, I18NStreamAsset i18Asset)
    {
        LanguageCode = languageCode;
        if (i18Asset.MemStream == null)
        {
            throw new ArgumentException(
                "The internationalization asset must have a non-null MemStream property.",
                nameof(i18Asset));
        }

        _strCatalog = new Catalog(i18Asset.MemStream, new CultureInfo(languageCode));
    }

    public string? GetString(string key, params object[] args)
    {
        return _strCatalog.GetString(key, args);
    }

    public string? GetStringPlural(string key, string pluralKey, long count, params object[] args)
    {
        return _strCatalog.GetPluralString(key, pluralKey, count, [count, ..args]);
    }

    public string? GetStringCtx(string key, string context, params object[] args)
    {
        return _strCatalog.GetParticularString(context, key, args);
    }

    public string? GetStringPluralCtx(string key, string pluralKey, string context, long count, params object[] args)
    {
        return _strCatalog.GetParticularPluralString(context, key, pluralKey, count, [count, ..args]);
    }
}
