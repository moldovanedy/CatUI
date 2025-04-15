namespace CatUI.Data.Enums
{
    /// <summary>
    /// This can also be cast to <see cref="AlignmentType"/> if needed.
    /// </summary>
    public enum HorizontalAlignmentType
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    /// <summary>
    /// This can also be cast to <see cref="AlignmentType"/> if needed.
    /// </summary>
    public enum VerticalAlignmentType
    {
        Top = 0,
        Center = 1,
        Bottom = 2
    }

    /// <summary>
    /// This can also be cast to <see cref="HorizontalAlignmentType"/> or <see cref="VerticalAlignmentType"/> if needed.
    /// </summary>
    public enum AlignmentType
    {
        Start = 0,
        Center = 1,
        End = 2
    }

    /// <summary>
    /// The orientation of an element.
    /// </summary>
    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    }
}
