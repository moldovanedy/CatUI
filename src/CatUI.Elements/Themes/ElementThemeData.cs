using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes
{
    public class ElementThemeData : CatObject
    {
        public const string STYLE_NORMAL = "normal";
        public const string STYLE_HOVER = "hover";

        public string ForState { get; internal set; }

        public IBrush? Background { get; set; }
        public CornerInset? CornerRadius { get; set; }

        public ElementThemeData()
        {
            ForState = STYLE_NORMAL;
        }

        public ElementThemeData(string forState)
        {
            ForState = forState;
        }

        public override ElementThemeData Duplicate()
        {
            return new ElementThemeData(ForState)
            {
                // ReSharper disable ArrangeThisQualifier
                Background = this.Background, CornerRadius = this.CornerRadius
                // ReSharper restore ArrangeThisQualifier
            };
        }

        public virtual List<string> GetAllBuiltInStates()
        {
            return new List<string> { STYLE_NORMAL, STYLE_HOVER };
        }

        /// <summary>
        /// Returns the default theme data for the given state. Every call returns a new object, so it's safe to modify it.
        /// </summary>
        /// <param name="state">The state for which you want to get the default data.</param>
        /// <returns></returns>
        public virtual ElementThemeData GetDefaultData(string state)
        {
            return new ElementThemeData(state) { Background = new ColorBrush(), CornerRadius = new CornerInset() };
        }

        /// <inheritdoc cref="GetDefaultData"/>
        public T GetDefaultData<T>(string state) where T : ElementThemeData, new()
        {
            return (T)GetDefaultData(state);
        }

        /// <summary>
        /// Sets the properties taken from the given object where those are non-null. Any null value on the given object
        /// is ignored.
        /// </summary>
        /// <remarks>
        /// If you override this method, make sure to always call the base method
        /// (<code>base.ApplyDataAdditively(theme)</code>) first, because otherwise this will result in unexpected issues.
        /// </remarks>
        /// <param name="themeData">The theme data object to consider.</param>
        public virtual void ApplyDataAdditively(ElementThemeData themeData)
        {
            Background = themeData.Background ?? Background;
            CornerRadius = themeData.CornerRadius ?? CornerRadius;
        }

        /// <summary>
        /// For any non-null field in the mask, it sets that field in the current object to null.
        /// See <see cref="ThemeDefinition{T}.ResetThemeDataForStateAdditively"/> for more info.
        /// </summary>
        /// <param name="mask">The mask to use.</param>
        public virtual void ResetDataAdditively(ElementThemeData mask)
        {
            if (mask.Background != null)
            {
                Background = null;
            }

            if (mask.CornerRadius != null)
            {
                CornerRadius = null;
            }
        }
    }
}
