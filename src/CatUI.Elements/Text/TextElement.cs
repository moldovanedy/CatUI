using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Enums;
using CatUI.RenderingEngine.GraphicsCaching;

namespace CatUI.Elements.Text;

/// <summary>
/// An abstract class that represents all elements that are composed just of text, like labels. Note that most elements
/// that do have text are generally composed of multiple elements, and the actual text element is just a subelement, so
/// those DON'T inherit this class.
/// </summary>
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
        set => TextProperty.Value = value;
    }

    private string _text = string.Empty;

    public ObservableProperty<string> TextProperty
    {
        get => _textProperty;
        set => _textProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<string> _textProperty = new(string.Empty);

    private void SetText(string? value)
    {
        _text = value ?? string.Empty;
        SetLocalValue(nameof(Text), value);
        MarkLayoutDirty();
    }

    public FontAsset? Font
    {
        get => _font;
        set => FontProperty.Value = value;
    }

    private FontAsset? _font;

    public ObservableProperty<FontAsset> FontProperty
    {
        get => _fontProperty;
        set => _fontProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<FontAsset> _fontProperty = new();

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
        set => FontSizeProperty.Value = value;
    }

    private Dimension _fontSize = new(16);

    public ObservableProperty<Dimension> FontSizeProperty
    {
        get => _fontSizeProperty;
        set => _fontSizeProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<Dimension> _fontSizeProperty = new(new Dimension(16));

    private void SetFontSize(Dimension value)
    {
        _fontSize = value;
        SetLocalValue(nameof(FontSize), value);
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
        set => TextAlignmentProperty.Value = value;
    }

    private TextAlignmentType _textAlignment = TextAlignmentType.Left;

    public ObservableProperty<TextAlignmentType> TextAlignmentProperty
    {
        get => _textAlignmentProperty;
        set => _textAlignmentProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<TextAlignmentType> _textAlignmentProperty = new(TextAlignmentType.Left);

    private void SetTextAlignment(TextAlignmentType value)
    {
        _textAlignment = value;
        SetLocalValue(nameof(TextAlignment), value);
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

    public TextElement(TextElement other) : base(other)
    {
        InitPropertiesEvents();
        Text = other.Text;
        FontSize = other.FontSize;
        TextAlignment = other.TextAlignment;
    }

    private void InitPropertiesEvents()
    {
        TextProperty.ValueChangedEvent += SetText;
        FontProperty.ValueChangedEvent += SetFont;
        FontSizeProperty.ValueChangedEvent += SetFontSize;
        TextAlignmentProperty.ValueChangedEvent += SetTextAlignment;
    }
}
