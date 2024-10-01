namespace CatUI.Data.Enums
{
    public enum TextBreakMode
    {
        /// <summary>
        /// Doesn't break the text, meaning it might overflow if the word is larger than the width of the element.
        /// </summary>
        NoBreak = 0,
        /// <summary>
        /// Will break the text only at the \u00ad ("soft hyphen" or "shy") character. If the portion of the word between two "shy" or spaces
        /// is larger than the width of the element, the content will overflow.
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
        Stroke = 1,
        FillAndStroke = 2,
    }
}
