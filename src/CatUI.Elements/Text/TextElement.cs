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
        public ObservableProperty<string> TextProperty { get; private set; } = new(string.Empty);

        /// <summary>
        /// Represents the size of the font to use when drawing the text. The default value is 12dp.
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

        private Dimension _fontSize = new(12);
        public ObservableProperty<Dimension> FontSizeProperty { get; private set; } = new(new Dimension(16));

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

        public ObservableProperty<TextOverflowMode> OverflowModeProperty { get; private set; } =
            new(TextOverflowMode.Ellipsis);

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

        public ObservableProperty<TextAlignmentType> TextAlignmentProperty { get; private set; }
            = new(TextAlignmentType.Left);

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
        public ObservableProperty<string> OverflowStringProperty { get; private set; } = new("\u2026");

        public TextElement()
        {
            InitPropertiesEvents();
        }

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
            InitPropertiesEvents();
        }

        ~TextElement()
        {
            TextProperty = null!;
            FontSizeProperty = null!;
            OverflowModeProperty = null!;
            TextAlignmentProperty = null!;
            OverflowStringProperty = null!;
        }

        private void InitPropertiesEvents()
        {
            TextProperty.ValueChangedEvent += SetText;
            FontSizeProperty.ValueChangedEvent += SetFontSize;
            OverflowModeProperty.ValueChangedEvent += SetOverflowMode;
            TextAlignmentProperty.ValueChangedEvent += SetTextAlignment;
            OverflowStringProperty.ValueChangedEvent += SetOverflowString;
        }

        private void SetText(string? value)
        {
            _text = value ?? string.Empty;
        }

        private void SetFontSize(Dimension value)
        {
            _fontSize = value;
        }

        private void SetOverflowMode(TextOverflowMode value)
        {
            _overflowMode = value;
        }

        private void SetTextAlignment(TextAlignmentType value)
        {
            _textAlignment = value;
        }

        private void SetOverflowString(string? value)
        {
            _overflowString = value ?? string.Empty;
        }
    }
}
