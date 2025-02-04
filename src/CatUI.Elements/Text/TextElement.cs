using CatUI.Data;
using CatUI.Data.Enums;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        /// <summary>
        /// The text of the element. Can use hyphenation with soft hyphens (U+00AD), but not all text elements support
        /// multiple lines; read the documentation on each one to find out if it supports multiple lines or not.
        /// The default value is an empty string.
        /// </summary>
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
        public ObservableProperty<string> TextProperty { get; } = new(string.Empty);

        /// <summary>
        /// If true, it makes the text wrap itself if the width is too small to fit an entire row. Beware that word wrapping
        /// involves quite a lot of calculations and elements without word wrapping are generally faster. Use only when
        /// needed. See the remarks on each element that implements this interface for more information.
        /// The default value is false.
        /// </summary>

        //TODO: implement this in an interface (like IWordWrappable)
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
        public ObservableProperty<bool> WordWrapProperty { get; } = new(false);

        /// <summary>
        /// Specifies the behavior of the text element when the text is too large to render in the given space.
        /// The actual behavior depends on <see cref="AllowsExpansion"/>. See <see cref="TextOverflowMode"/> for information
        /// about possible values. The default value is <see cref="TextOverflowMode.Ellipsis"/>.
        /// </summary>
        public TextOverflowMode OverflowMode
        {
            get => _overflowMode;
            set
            {
                _overflowMode = value;
                OverflowModeProperty.Value = value;
            }
        }

        private TextOverflowMode _overflowMode = TextOverflowMode.Ellipsis;

        public ObservableProperty<TextOverflowMode> OverflowModeProperty { get; } = new(TextOverflowMode.Ellipsis);

        /// <summary>
        /// The text alignment to use. All values except <see cref="TextAlignmentType.Justify"/> are generally supported
        /// by all text elements, for <see cref="TextAlignmentType.Justify"/> consult the documentation on each document
        /// to see if it specifies that it's not supported (if no mention of it, it means it's supported).
        /// The default value is <see cref="TextAlignmentType.Left"/>.
        /// </summary>
        public TextAlignmentType TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                TextAlignmentProperty.Value = value;
            }
        }

        private TextAlignmentType _textAlignment = TextAlignmentType.Left;
        public ObservableProperty<TextAlignmentType> TextAlignmentProperty { get; } = new(TextAlignmentType.Left);

        /// <summary>
        /// 
        /// </summary>
        public string OverflowString
        {
            get => _overflowString;
            set
            {
                _overflowString = value;
                OverflowStringProperty.Value = value;
            }
        }

        private string _overflowString = "\u2026";
        public ObservableProperty<string> OverflowStringProperty { get; } = new("\u2026");

        /// <summary>
        /// If true, the element will ignore <see cref="Element.PreferredWidth"/> and <see cref="Element.PreferredHeight"/>,
        /// only respecting the minimum and maximum width and height constraints (like <see cref="Element.MaxHeight"/>).
        /// It tries to occupy as little space as possible (in regard to the minimum size), but is free to expand until
        /// the maximum size is reached.
        /// </summary>
        /// <remarks>
        /// This expansion will generally happen:
        /// <list type="bullet">
        /// <item>when <see cref="WordWrap"/> is false if the element implements that interface</item>
        /// <item>when a word's width is larger than the element's width</item>
        /// <item>when the text is so large that it won't fit in the element's height (as it occupies more rows)</item>
        /// </list>
        /// <para>
        /// What exactly happens when this property is false and the text is larger than the element's size depends
        /// on each element that implements this interface. For example, <see cref="Label"/> will have the
        /// <see cref="OverflowString"/> at the end of the last row.
        /// </para>
        /// </remarks>

        //TODO: implement the stated behavior and also put it in some interface
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
        public ObservableProperty<bool> AllowsExpansionProperty { get; } = new(true);

        public TextElement(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
            Text = text;
            TextAlignment = textAlignment;
        }
    }
}
