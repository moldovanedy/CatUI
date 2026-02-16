using System;
using System.Collections.Generic;
using CatUI.Data.Assets;
using SkiaSharp;

namespace CatUI.RenderingEngine.GraphicsCaching;

/// <summary>
/// Controls the text measurements for the entire application. A value is introduced here, and it is used
/// for direct access instead of calling SkiaSharp's specific font measuring methods, which are much slower.
/// </summary>
public static class TextMeasuringCache
{
    /// <summary>
    /// The total number of entries.
    /// </summary>
    public static uint NumberOfEntries { get; private set; }

    private static readonly List<FontAsset> _loadedFonts = [];

    /// <summary>
    /// The key is the text, the value is a dictionary where the key is the font, and each value is another
    /// dictionary where the key is the font size (rounded to 2 decimals), and the value is finally the width of the
    /// text.
    /// </summary>
    private static readonly Dictionary<string, Dictionary<FontAsset, Dictionary<float, float>>>
        _cache = new(500);

    /// <summary>
    /// Given the values, will search for the text's width and will return it if it's found. Otherwise, it is
    /// calculated using <see cref="Calculate"/>.
    /// </summary>
    /// <param name="text">The text to search for. For empty will return 0.</param>
    /// <param name="fontSize">The font size to search for. It will be rounded to 2 decimals.</param>
    /// <param name="fontAsset">The index in the font table. 0 for platform default.</param>
    /// <returns>The text's width in pixels.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the font weight is not between 100 and 1000 and the modulus to 100 is not 0 (x % 100 != 0).
    /// Also when the fontSize is negative.
    /// </exception>
    public static float GetValueOrCalculate(
        string text,
        float fontSize,
        FontAsset? fontAsset = null)
    {
        if (fontSize < 0)
        {
            throw new ArgumentException("Negative font size is not allowed", nameof(fontSize));
        }

        if (text.Length == 0)
        {
            return 0;
        }

        fontAsset ??= FontAsset.Default;
        fontSize = MathF.Round(fontSize, 2);

        //if the text isn't in the cache 
        if (!_cache.TryGetValue(text, out Dictionary<FontAsset, Dictionary<float, float>>? records))
        {
            records = new Dictionary<FontAsset, Dictionary<float, float>>();
            _cache[text] = records;
        }

        //if the text entry doesn't have this font
        if (!_cache[text].TryGetValue(fontAsset, out Dictionary<float, float>? fontEntry))
        {
            fontEntry = new Dictionary<float, float>();
            _cache[text][fontAsset] = fontEntry;
        }

        //if the given size doesn't exist
        if (!fontEntry.TryGetValue(fontSize, out float value))
        {
            value = Calculate(text.AsSpan(), fontAsset, fontSize);
            fontEntry[fontSize] = value;
            NumberOfEntries++;
        }

        return value;
    }

    /// <summary>
    /// Calculates the text's width using <see cref="SKFont.MeasureText(string, SKPaint)"/> with the given paint.
    /// </summary>
    /// <param name="text">See <see cref="GetValueOrCalculate"/>.</param>
    /// <param name="font">See <see cref="GetValueOrCalculate"/>.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The text's width in pixels.</returns>
    public static float Calculate(ReadOnlySpan<char> text, FontAsset font, float fontSize)
    {
        return font.GetSkiaFont(fontSize).MeasureText(text);
    }

    /// <summary>
    /// Clears the values from the cache. It might perform slowly because if also updates <see cref="NumberOfEntries"/>,
    /// so it has to count a lot of values. The parameters control what to delete: if no parameters are given,
    /// the entire cache will be deleted (very fast); if both are given, only the values for the given text
    /// and the given font will be removed (very fast). Consult the notes for the parameters below for more information
    /// on the situation when only one parameter is set to a value other than default.
    /// </summary>
    /// <remarks>
    /// The font references will only be removed when the entire cache is purged. Fonts might still remain in memory
    /// if they are still in use by an Element.
    /// </remarks>
    /// <param name="forText">
    /// If this is given without the font forFontAsset == null, it will remove the entry for that text (faster).
    /// </param>
    /// <param name="forFontAsset">
    /// If this is given without text forText == null, it will remove the entries for the font asset on all texts (slow).
    /// </param>
    public static void PurgeCache(Memory<char>? forText = null, FontAsset? forFontAsset = null)
    {
        //for all texts
        if (forText == null)
        {
            //clear completely
            if (forFontAsset == null)
            {
                _cache.Clear();
                _loadedFonts.Clear();
                //reset
                NumberOfEntries = 0;
            }
            //only for a font
            else
            {
                foreach (string key in _cache.Keys)
                {
                    Dictionary<float, float> toDelete = _cache[key][forFontAsset];
                    NumberOfEntries -= (uint)toDelete.Count;
                    toDelete.Clear();
                }
            }
        }
        //a specific text, all fonts
        else if (forFontAsset == null)
        {
            string text = forText.ToString()!;

            if (_cache.TryGetValue(text, out Dictionary<FontAsset, Dictionary<float, float>>? records))
            {
                foreach (Dictionary<float, float> record in records.Values)
                {
                    NumberOfEntries -= (uint)record.Count;
                }
            }

            _cache.Remove(text);
        }
        //a specific text and a specific font
        else
        {
            string text = forText.ToString()!;

            Dictionary<float, float> toDelete = _cache[text][forFontAsset];
            NumberOfEntries -= (uint)toDelete.Count;
            toDelete.Clear();
        }
    }

    /// <summary>
    /// Will keep a reference to this font and use it for text measurements when needed. If it already exists, it
    /// won't do anything.
    /// </summary>
    /// <param name="fontAsset">The font that is in use by the application.</param>
    public static void UseFont(FontAsset fontAsset)
    {
        foreach (FontAsset existingFont in _loadedFonts)
        {
            if (existingFont.Equals(fontAsset))
            {
                return;
            }

            if (existingFont.FontFamily == fontAsset.FontFamily &&
                existingFont.FontWeight == fontAsset.FontWeight &&
                existingFont.FontWidth == fontAsset.FontWidth &&
                existingFont.FontSlant == fontAsset.FontSlant)
            {
                return;
            }
        }

        _loadedFonts.Add(fontAsset);
    }
}
