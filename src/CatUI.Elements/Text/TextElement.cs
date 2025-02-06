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
        /// Represents the size of the font to use when drawing the text. The default value is 16dp.
        /// </summary>
        public Dimension FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                FontSizeProperty.Value = value;
            }
        }

        private Dimension _fontSize = new(16);
        public ObservableProperty<Dimension> FontSizeProperty { get; } = new(new Dimension(16));

        /// <summary>
        /// Specifies the behavior of the text element when the text is too large to render in the given space.
        /// The actual behavior depends on each element. See <see cref="TextOverflowMode"/> for information
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
        /// Specifies the string that will be appended at the end of a row if the text cannot be drawn completely
        /// (because it will overflow the element, for example).
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

        public TextElement() { }

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
