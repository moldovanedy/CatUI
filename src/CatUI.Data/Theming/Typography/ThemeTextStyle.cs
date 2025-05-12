using CatUI.Data.Enums;

namespace CatUI.Data.Theming.Typography
{
    /// <summary>
    /// Contains the properties that can be used for typography (i.e. texts in the application).
    /// </summary>
    public class ThemeTextStyle
    {
        //TODO: replace this with an actual font resource
        public string FontName { get; } = "Arial";

        /// <summary>
        /// Specifies how thick (bold) the text is. Although an int, allowed values are multiples of 100 from 100 to 1000
        /// (100, 200, ..., 1000). Any other value will be rounded to the nearest valid value.
        /// </summary>
        public FontWeightPreset FontWeight { get; }

        /// <summary>
        /// Specifies the font size used. This is always in font units, which are similar to <see cref="Unit.Dp"/>,
        /// but further scaled by platform options where supported.
        /// </summary>
        public float FontSize { get; }

        /// <summary>
        /// Specifies the line height (spacing between the text baselines) in a dimensionless value that is multiplied with
        /// <see cref="FontSize"/> to get the final line height. Only positive values are allowed. All text elements
        /// will also add half of this value above the first line and below the last line.
        /// </summary>
        /// <example>
        /// 1 means the lines are tightly placed one after another, 2 means there is a space of <see cref="FontSize"/>
        /// between lines etc.
        /// </example>
        public float LineHeight { get; }

        public ThemeTextStyle(FontWeightPreset fontWeight, float fontSize, float lineHeight)
        {
            FontWeight = fontWeight;
            FontSize = fontSize;
            LineHeight = lineHeight;
        }
    }
}
