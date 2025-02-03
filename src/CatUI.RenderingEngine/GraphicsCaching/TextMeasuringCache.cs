using System;
using System.Collections.Generic;
using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.RenderingEngine.GraphicsCaching
{
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

        /// <summary>
        /// The key is the text, the value is an array where the index is the font index and each element is another
        /// dictionary where the key is the font size (rounded to 2 decimals) and the value is yet another dictionary,
        /// with the key being the font weight and the value being finally the width of the text.
        /// </summary>
        private static readonly Dictionary<ReadOnlyMemory<char>, List<Dictionary<float, Dictionary<int, float>>>>
            _cache =
                new(500);

        /// <summary>
        /// Given the values, will search for the text's width and will return it if it's found. Otherwise, it is
        /// calculated using <see cref="Calculate"/>.
        /// </summary>
        /// <param name="text">The text to search for. For empty will return 0.</param>
        /// <param name="fontSize">The font size to search for. It will be rounded to 2 decimals.</param>
        /// <param name="fontWeight">
        /// The font weight to use. 400 is regular, 700 is bold. It's between 100 and 1000 inclusive, going in
        /// increments of 100 (100, 200, 300 etc.)
        /// </param>
        /// <param name="fontIndex">The index in the font table. 0 for platform default.</param>
        /// <returns>The text's width in pixels.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the font index or the font size are negative. Also when the font weight is not between 100 and 1000
        /// and the modulus to 100 is not 0 (x % 100 != 0).
        /// </exception>
        public static float GetValueOrCalculate(ReadOnlyMemory<char> text, float fontSize, int fontWeight = 400,
            int fontIndex = 0)
        {
            if (fontIndex < 0)
            {
                throw new ArgumentException("Negative font index is not allowed", nameof(fontIndex));
            }

            if (fontSize < 0)
            {
                throw new ArgumentException("Negative font size is not allowed", nameof(fontSize));
            }

            if (fontWeight < 100 || fontWeight > 1000 || fontWeight % 100 != 0)
            {
                throw new ArgumentException("Invalid font weight value", nameof(fontWeight));
            }

            if (text.Length == 0)
            {
                return 0;
            }

            fontSize = MathF.Round(fontSize, 2);

            //if the text isn't in the cache 
            if (!_cache.TryGetValue(text, out List<Dictionary<float, Dictionary<int, float>>>? records))
            {
                records = new List<Dictionary<float, Dictionary<int, float>>>(fontIndex + 1);
                _cache[text] = records;
            }

            //if the text entry doesn't have space for all the fonts
            if (fontIndex >= records.Count)
            {
                Dictionary<float, Dictionary<int, float>>[] newRecords =
                    new Dictionary<float, Dictionary<int, float>>[fontIndex - records.Count + 1];
                for (int i = 0; i < newRecords.Length; i++)
                {
                    newRecords[i] = new Dictionary<float, Dictionary<int, float>>();
                }

                _cache[text].AddRange(newRecords);
            }

            //if the given size doesn't exist
            if (!records[fontIndex].TryGetValue(fontSize, out Dictionary<int, float>? weightDictionary))
            {
                weightDictionary = new Dictionary<int, float>();
                records[fontIndex][fontSize] = weightDictionary;
            }

            //if the given weight doesn't exist
            if (!weightDictionary.TryGetValue(fontWeight, out float value))
            {
                value = Calculate(text.Span, fontSize, fontWeight, fontIndex);
                weightDictionary[fontWeight] = value;
                NumberOfEntries++;
            }

            return value;
        }

        /// <summary>
        /// Calculates the text's width using <see cref="SKPaint.MeasureText(string)"/>.
        /// </summary>
        /// <param name="text">See <see cref="GetValueOrCalculate"/>.</param>
        /// <param name="fontSize">See <see cref="GetValueOrCalculate"/>.</param>
        /// <param name="fontWeight">See <see cref="GetValueOrCalculate"/>.</param>
        /// <param name="fontIndex">See <see cref="GetValueOrCalculate"/>.</param>
        /// <returns>The text's width in pixels.</returns>
        public static float Calculate(ReadOnlySpan<char> text, float fontSize, int fontWeight = 400, int fontIndex = 0)
        {
            SKPaint paint = PaintManager.GetPaint(fontSize: fontSize);
            return paint.MeasureText(text);
        }

        /// <summary>
        /// Clears the values from the cache. It might perform slowly because if also updates <see cref="NumberOfEntries"/>,
        /// so it has to count a lot of values. The parameters control what to delete: if no parameters are given,
        /// the entire cache will be deleted (very fast); if both are given, only the values for the given text
        /// and the given font will be removed (very fast). Consult the notes for the parameters below for more information
        /// for the situation when only one parameter is set to a value other than default.
        /// </summary>
        /// <param name="forText">
        /// If this is given without the font index forFontIndex == -1, it will remove
        /// the entry for that text (faster).
        /// </param>
        /// <param name="forFontIndex">
        /// If this is given without text forText == null, it will remove the entries
        /// for the font index on all texts (slow).
        /// </param>
        public static void PurgeCache(Memory<char>? forText = null, int forFontIndex = -1)
        {
            //for all texts
            if (forText == null)
            {
                //clear completely
                if (forFontIndex < 0)
                {
                    _cache.Clear();
                    //reset
                    NumberOfEntries = 0;
                }
                //only for a font
                else
                {
                    foreach (ReadOnlyMemory<char> key in _cache.Keys)
                    {
                        Dictionary<float, Dictionary<int, float>> toDelete = _cache[key][forFontIndex];
                        foreach (KeyValuePair<float, Dictionary<int, float>> pair in toDelete)
                        {
                            NumberOfEntries -= (uint)pair.Value.Count;
                        }

                        _cache[key][forFontIndex].Clear();
                    }
                }
            }
            //a specific text, all fonts
            else if (forFontIndex < 0)
            {
                Memory<char> text = (Memory<char>)forText;

                if (_cache.TryGetValue(text, out List<Dictionary<float, Dictionary<int, float>>>? records))
                {
                    foreach (Dictionary<float, Dictionary<int, float>> record in records)
                    {
                        foreach (KeyValuePair<float, Dictionary<int, float>> pair in record)
                        {
                            NumberOfEntries -= (uint)pair.Value.Count;
                        }
                    }
                }

                _cache.Remove(text);
            }
            //a specific text and a specific font
            else
            {
                Memory<char> text = (Memory<char>)forText;

                Dictionary<float, Dictionary<int, float>> toDelete = _cache[text][forFontIndex];
                foreach (KeyValuePair<float, Dictionary<int, float>> pair in toDelete)
                {
                    NumberOfEntries -= (uint)pair.Value.Count;
                }

                toDelete.Clear();
            }
        }
    }
}
