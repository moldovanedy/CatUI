using System;
using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes.Text
{
    public class TextElementThemeData : ElementThemeData
    {
        public IBrush? FillBrush { get; set; }
        public IBrush? OutlineBrush { get; set; }
        public Dimension? FontSize { get; set; }

        /// <summary>
        /// A dimensionless value that will be multiplied with <see cref="FontSize"/> to get the element's line height.
        /// Default is 1.2.
        /// </summary>
        public float? LineHeight { get; set; }

        public TextElementThemeData()
        {
        }

        public TextElementThemeData(string forState) : base(forState)
        {
        }

        public override TextElementThemeData GetDefaultData(string state)
        {
            var data = new TextElementThemeData();
            ElementThemeData baseDefaults = base.GetDefaultData(state);
            data.Background = baseDefaults.Background;
            data.CornerRadius = baseDefaults.CornerRadius;

            data.FillBrush = new ColorBrush(new Color(0x00_00_00));
            data.OutlineBrush = new ColorBrush();
            data.FontSize = "16dp";
            data.LineHeight = 1.2f;

            return data;
        }

        public override void ApplyDataAdditively(ElementThemeData themeData)
        {
            base.ApplyDataAdditively(themeData);

            if (themeData is not TextElementThemeData textElementThemeData)
            {
                throw new InvalidOperationException(
                    $"The {nameof(textElementThemeData)} must be of type {nameof(TextElementThemeData)}");
            }

            FillBrush = textElementThemeData.FillBrush ?? FillBrush;
            OutlineBrush = textElementThemeData.OutlineBrush ?? OutlineBrush;
            FontSize = textElementThemeData.FontSize ?? FontSize;
            LineHeight = textElementThemeData.LineHeight ?? LineHeight;
        }

        public override void ResetDataAdditively(ElementThemeData mask)
        {
            base.ResetDataAdditively(mask);

            if (mask is not TextElementThemeData textElementThemeData)
            {
                throw new InvalidOperationException(
                    $"The {nameof(textElementThemeData)} must be of type {nameof(TextElementThemeData)}");
            }

            if (textElementThemeData.FillBrush != null)
            {
                textElementThemeData.FillBrush = null;
            }

            if (textElementThemeData.OutlineBrush != null)
            {
                textElementThemeData.OutlineBrush = null;
            }

            if (textElementThemeData.FontSize != null)
            {
                textElementThemeData.FontSize = null;
            }

            if (textElementThemeData.LineHeight != null)
            {
                textElementThemeData.LineHeight = null;
            }
        }
    }
}
