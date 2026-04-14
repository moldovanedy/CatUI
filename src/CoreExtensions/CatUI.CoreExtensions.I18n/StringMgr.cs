using System;
using System.Collections.Generic;
using CatUI.Data;

namespace CatUI.CoreExtensions.I18n;

/// <summary>
/// A manager for localized strings, having the ability to store properties that will be updated when the language
/// changes. You always need to call <see cref="SetLocalizationManager"/> when the language changes and at the start
/// of the application as well.
/// </summary>
public class StringMgr
{
    /// <summary>
    /// Represents the default string manager. You should use this for most cases instead of creating a new instance
    /// with <see cref="CreateNewProfile"/>.
    /// </summary>
    public static StringMgr Instance { get; } = new();

    private ILocalizationManager? _localizationManager;

    private readonly Dictionary<string, List<StringProperty>> _storedProperties = [];
    private readonly Dictionary<string, List<StringPropertyPluralized>> _storedPropertiesPluralized = [];
    private readonly Dictionary<string, List<StringPropertyWithContext>> _storedPropertiesWithContext = [];

    private readonly Dictionary<string, List<StringPropertyWithContextPluralized>>
        _storedPropertiesWithContextPluralized = [];

    private StringMgr()
    {
    }

    /// <summary>
    /// Creates a new string manager, completely independent of the default one (<see cref="Instance"/>). This is
    /// only for specialized use cases, as in general you should use <see cref="Instance"/>.
    /// </summary>
    /// <returns></returns>
    public static StringMgr CreateNewProfile()
    {
        return new StringMgr();
    }

    /// <summary>
    /// Sets the localization manager to use. This represents that actual implementation that will be used to get
    /// the strings.
    /// </summary>
    /// <param name="localizationManager"></param>
    public void SetLocalizationManager(ILocalizationManager? localizationManager)
    {
        _localizationManager = localizationManager;
        Refresh();
    }

    /// <summary>
    /// Refreshes all the properties that are currently stored. This should only ever be called when the language
    /// changes (i.e., when you also call <see cref="SetLocalizationManager"/>), as it is a slow operation
    /// (O(m*n), where m is the number of properties and n is the number of strings).
    /// </summary>
    /// <remarks>This is automatically called by <see cref="SetLocalizationManager"/>.</remarks>
    public void Refresh()
    {
        foreach (KeyValuePair<string, List<StringProperty>> propertyListKvPair in _storedProperties)
        {
            for (int i = 0; i < propertyListKvPair.Value.Count; i++)
            {
                StringProperty property = propertyListKvPair.Value[i];
                if (property.PropReference.TryGetTarget(out ObservableProperty<string>? observableProperty))
                {
                    observableProperty.Value = GetString(propertyListKvPair.Key, property.Args);
                }
                else
                {
                    propertyListKvPair.Value.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (KeyValuePair<string, List<StringPropertyPluralized>> propertyListKvPair in _storedPropertiesPluralized)
        {
            for (int i = 0; i < propertyListKvPair.Value.Count; i++)
            {
                StringPropertyPluralized property = propertyListKvPair.Value[i];
                if (property.PropReference.TryGetTarget(out ObservableProperty<string>? observableProperty))
                {
                    observableProperty.Value = GetStringPlural(
                        propertyListKvPair.Key,
                        property.PluralKey,
                        property.Count,
                        property.Args);
                }
                else
                {
                    propertyListKvPair.Value.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (KeyValuePair<string, List<StringPropertyWithContext>> propertyListKvPair in
                 _storedPropertiesWithContext)
        {
            for (int i = 0; i < propertyListKvPair.Value.Count; i++)
            {
                StringPropertyWithContext property = propertyListKvPair.Value[i];
                if (property.PropReference.TryGetTarget(out ObservableProperty<string>? observableProperty))
                {
                    observableProperty.Value = GetStringCtx(propertyListKvPair.Key, property.Context, property.Args);
                }
                else
                {
                    propertyListKvPair.Value.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (KeyValuePair<string, List<StringPropertyWithContextPluralized>> propertyListKvPair in
                 _storedPropertiesWithContextPluralized)
        {
            for (int i = 0; i < propertyListKvPair.Value.Count; i++)
            {
                StringPropertyWithContextPluralized property = propertyListKvPair.Value[i];
                if (property.PropReference.TryGetTarget(out ObservableProperty<string>? observableProperty))
                {
                    observableProperty.Value = GetStringPluralCtx(
                        propertyListKvPair.Key,
                        property.PluralKey,
                        property.Context,
                        property.Count,
                        property.Args);
                }
                else
                {
                    propertyListKvPair.Value.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void RemovePropertiesWithKey(string key)
    {
        bool isRemoved = _storedProperties.Remove(key);
        if (isRemoved)
        {
            return;
        }

        isRemoved = _storedPropertiesPluralized.Remove(key);
        if (isRemoved)
        {
            return;
        }

        isRemoved = _storedPropertiesWithContext.Remove(key);
        if (isRemoved)
        {
            return;
        }

        _storedPropertiesWithContextPluralized.Remove(key);
    }

    public ObservableProperty<string> GetProperty(string key, params object[] args)
    {
        ObservableProperty<string> property = new(GetString(key, args));
        WeakReference<ObservableProperty<string>> weakReference = new(property);
        var stringProperty = new StringProperty(weakReference, args);

        if (_storedProperties.TryGetValue(key, out List<StringProperty>? storedProperties))
        {
            storedProperties.Add(stringProperty);
        }
        else
        {
            _storedProperties[key] = [stringProperty];
        }

        return property;
    }

    public ObservableProperty<string> GetPropertyPlural(string key, string pluralKey, long count, params object[] args)
    {
        ObservableProperty<string> property = new(GetStringPlural(key, pluralKey, count, args));
        WeakReference<ObservableProperty<string>> weakReference = new(property);
        var stringProperty = new StringPropertyPluralized(weakReference, pluralKey, count, args);

        if (_storedPropertiesPluralized.TryGetValue(key, out List<StringPropertyPluralized>? storedProperties))
        {
            storedProperties.Add(stringProperty);
        }
        else
        {
            _storedPropertiesPluralized[key] = [stringProperty];
        }

        return property;
    }

    public ObservableProperty<string> GetPropertyCtx(string key, string context, params object[] args)
    {
        ObservableProperty<string> property = new(GetStringCtx(key, context, args));
        WeakReference<ObservableProperty<string>> weakReference = new(property);
        var stringProperty = new StringPropertyWithContext(weakReference, context, args);

        if (_storedPropertiesWithContext.TryGetValue(key, out List<StringPropertyWithContext>? storedProperties))
        {
            storedProperties.Add(stringProperty);
        }
        else
        {
            _storedPropertiesWithContext[key] = [stringProperty];
        }

        return property;
    }

    public ObservableProperty<string> GetPropertyPluralCtx(
        string key, string pluralKey, string context, long count, params object[] args)
    {
        ObservableProperty<string> property = new(GetStringPluralCtx(key, pluralKey, context, count, args));
        WeakReference<ObservableProperty<string>> weakReference = new(property);
        var stringProperty = new StringPropertyWithContextPluralized(weakReference, pluralKey, context, count, args);

        if (_storedPropertiesWithContextPluralized.TryGetValue(
                key,
                out List<StringPropertyWithContextPluralized>? storedProperties))
        {
            storedProperties.Add(stringProperty);
        }
        else
        {
            _storedPropertiesWithContextPluralized[key] = [stringProperty];
        }

        return property;
    }

    public string GetString(string key, params object[] args)
    {
        return _localizationManager?.GetString(key, args) ?? key;
    }

    public string GetStringPlural(string key, string pluralKey, long count, params object[] args)
    {
        return _localizationManager?.GetStringPlural(key, pluralKey, count, args) ?? key;
    }

    public string GetStringCtx(string key, string context, params object[] args)
    {
        return _localizationManager?.GetStringCtx(key, context, args) ?? key;
    }

    public string GetStringPluralCtx(string key, string pluralKey, string context, long count, params object[] args)
    {
        return _localizationManager?.GetStringPluralCtx(key, pluralKey, context, count, args) ?? key;
    }
}
