using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                TextProperty.Value = value;
            }
        }

        private string _text = string.Empty;
        public ObservableProperty<string> TextProperty { get; } = new();

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                WordWrapProperty.Value = value;
            }
        }

        private bool _wordWrap;
        public ObservableProperty<bool> WordWrapProperty { get; } = new();

        public TextOverflowMode TextOverflowMode
        {
            get => _textOverflowMode;
            set
            {
                _textOverflowMode = value;
                TextOverflowModeProperty.Value = value;
            }
        }

        private TextOverflowMode _textOverflowMode = TextOverflowMode.Ellipsis;

        public ObservableProperty<TextOverflowMode> TextOverflowModeProperty { get; }
            = new();

        public TextAlignmentType TextAlignment
        {
            get => _textAlignmentType;
            set
            {
                _textAlignmentType = value;
                TextAlignmentProperty.Value = value;
            }
        }

        private TextAlignmentType _textAlignmentType;
        public ObservableProperty<TextAlignmentType> TextAlignmentProperty { get; } = new();

        public string EllipsisString
        {
            get => _ellipsisString;
            set
            {
                _ellipsisString = value;
                EllipsisStringProperty.Value = value;
            }
        }

        private string _ellipsisString = "\u2026";
        public ObservableProperty<string> EllipsisStringProperty { get; } = new();

        /// <summary>
        /// If true, the element will expand beyond the set <see cref="Element.PreferredWidth"/> and <see cref="Element.PreferredHeight"/> without
        /// respecting the maximum width and height constraints (like <see cref="Element.MaxHeight"/>).
        /// </summary>
        /// <remarks>
        /// This expansion will generally happen:
        /// <list type="bullet">
        /// <item>when <see cref="WordWrap"/> is false</item>
        /// <item>when a word's width is larger than the element's width</item>
        /// <item>when the text is so large that it won't fit in the element's height (as it occupies more rows)</item>
        /// </list>
        /// <para>
        /// What exactly happens when this property is false and the text is larger than the element's size depends 
        /// on each element that derives from this class. For example, <see cref="Label"/> will have the 
        /// <see cref="EllipsisString"/> at the end of the last row.
        /// </para>
        /// </remarks>
        public bool AllowsExpansion
        {
            get => _allowsExpansion;
            set
            {
                _allowsExpansion = value;
                AllowsExpansionProperty.Value = value;
            }
        }

        private bool _allowsExpansion = true;
        public ObservableProperty<bool> AllowsExpansionProperty { get; } = new();

        public TextElement(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            ThemeDefinition<TextElementThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight)
        {
            Text = text;
            TextAlignment = textAlignment;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
        }
    }
}
