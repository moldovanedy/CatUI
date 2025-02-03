namespace CatUI.Data.Enums
{
    public enum TextAlignmentType
    {
        Justify = 0,
        Left = 1,
        Center = 2,
        Right = 3
    }

    /// <summary>
    /// Specifies the text breaking (i.e. rendering on multiple lines) behavior for text elements.
    /// </summary>
    public enum TextBreakMode
    {
        /// <summary>
        /// Only breaks the text at space or newline, meaning it might overflow if the word is larger than the width of the element.
        /// </summary>
        NoBreak = 0,

        /// <summary>
        /// Will break the text only at the \u00ad ("soft hyphen" or "shy") character or at space. If the portion 
        /// of the word between two "shy" or spaces is larger than the width of the element, the content will overflow.
        /// If there are no "shy" characters, it will behave as NoBreak.
        /// </summary>
        SoftBreak = 1,

        /// <summary>
        /// Will break the text wherever it is necessary, without respecting the "shy" character. Will generally not overflow.
        /// </summary>
        HardBreak = 2
    }

    public enum PaintMode
    {
        Fill = 0,
        Outline = 1,
        FillAndOutline = 2
    }

    public enum TextOverflowMode
    {
        /// <summary>
        /// When the text reaches the element's size limits, it will end the displayed text with an \u2026
        /// (horizontal ellipsis, similar to "...") or a custom string if the element allows it,
        /// even if that ellipsis will overflow (will only happen when the given space is too small, for example 15dp, which will
        /// only display the ellipsis instead of other text).
        /// </summary>
        Ellipsis = 0,

        /// <summary>
        /// The text will appear as if it continues to render beyond the element's size limit, but it is clipped and might show only portions
        /// of characters.
        /// </summary>
        Clip = 1,

        /// <summary>
        /// The text will not account for the element's size and overflow the element and possibly only be clipped by the surface's size.
        /// </summary>
        Overflow = 2
    }
}
