using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Enums;
using CatUI.RenderingEngine.GraphicsCaching;

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
                if (value != _text)
                {
                    TextProperty.Value = value;
                }
            }
        }

        private string _text = string.Empty;
        public ObservableProperty<string> TextProperty { get; } = new(string.Empty);

        private void SetText(string? value)
        {
            _text = value ?? string.Empty;
            SetLocalValue(nameof(Text), value);
            MarkLayoutDirty();
        }

        public FontAsset? Font
        {
            get => _font;
            set
            {
                if (value != _font)
                {
                    FontProperty.Value = value;
                }
            }
        }

        private FontAsset? _font;
        public ObservableProperty<FontAsset> FontProperty { get; } = new();

        private void SetFont(FontAsset? value)
        {
            _font = value;
            SetLocalValue(nameof(Font), value);
            MarkLayoutDirty();

            if (value != null)
            {
                TextMeasuringCache.UseFont(value);
            }
        }

        /// <summary>
        /// Represents the size of the font to use when drawing the text. The default value is 16dp.
        /// </summary>
        public Dimension FontSize
        {
            get => _fontSize;
            set
            {
                if (value != _fontSize)
                {
                    FontSizeProperty.Value = value;
                }
            }
        }

        private Dimension _fontSize = new(16);
        public ObservableProperty<Dimension> FontSizeProperty { get; } = new(new Dimension(16));

        private void SetFontSize(Dimension value)
        {
            _fontSize = value;
            SetLocalValue(nameof(FontSize), value);
            MarkLayoutDirty();
        }

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
                if (value != _overflowMode)
                {
                    OverflowModeProperty.Value = value;
                }
            }
        }

        private TextOverflowMode _overflowMode = TextOverflowMode.Ellipsis;

        public ObservableProperty<TextOverflowMode> OverflowModeProperty { get; } =
            new(TextOverflowMode.Ellipsis);

        private void SetOverflowMode(TextOverflowMode value)
        {
            _overflowMode = value;
            SetLocalValue(nameof(OverflowMode), value);
            MarkLayoutDirty();
        }

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
                if (value != _textAlignment)
                {
                    TextAlignmentProperty.Value = value;
                }
            }
        }

        private TextAlignmentType _textAlignment = TextAlignmentType.Left;

        public ObservableProperty<TextAlignmentType> TextAlignmentProperty { get; }
            = new(TextAlignmentType.Left);

        private void SetTextAlignment(TextAlignmentType value)
        {
            _textAlignment = value;
            SetLocalValue(nameof(TextAlignment), value);
            MarkLayoutDirty();
        }

        /// <summary>
        /// Specifies the string that will be appended at the end of a row if the text cannot be drawn completely
        /// (because it will overflow the element, for example).
        /// </summary>
        public string OverflowString
        {
            get => _overflowString;
            set
            {
                if (value != _overflowString)
                {
                    OverflowStringProperty.Value = value;
                }
            }
        }

        private string _overflowString = "\u2026";
        public ObservableProperty<string> OverflowStringProperty { get; } = new("\u2026");

        private void SetOverflowString(string? value)
        {
            _overflowString = value ?? string.Empty;
            SetLocalValue(nameof(OverflowString), value);
            MarkLayoutDirty();
        }

        public TextElement()
        {
            InitPropertiesEvents();
        }

        public TextElement(string text, TextAlignmentType textAlignment = TextAlignmentType.Left)
        {
            InitPropertiesEvents();
            Text = text;
            TextAlignment = textAlignment;
        }

        //~TextElement()
        //{
        //    TextProperty = null!;
        //    FontSizeProperty = null!;
        //    OverflowModeProperty = null!;
        //    TextAlignmentProperty = null!;
        //    OverflowStringProperty = null!;
        //}

        private void InitPropertiesEvents()
        {
            TextProperty.ValueChangedEvent += SetText;
            FontProperty.ValueChangedEvent += SetFont;
            FontSizeProperty.ValueChangedEvent += SetFontSize;
            OverflowModeProperty.ValueChangedEvent += SetOverflowMode;
            TextAlignmentProperty.ValueChangedEvent += SetTextAlignment;
            OverflowStringProperty.ValueChangedEvent += SetOverflowString;
        }
    }
}
