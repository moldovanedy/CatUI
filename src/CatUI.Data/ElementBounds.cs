using System.Numerics;

namespace CatUI.Data
{
    /// <summary>
    /// Represents the bounds of an element in device pixels, including the margins.
    /// </summary>
    /// <remarks>
    /// It's not axis-aligned, meaning that the actual display might be different if the element is a child of a TransformControl.
    /// </remarks>
    public readonly struct ElementBounds
    {
        /// <summary>
        /// The top left corner of the element's content, without margins and padding.
        /// </summary>
        public Rect BoundingRect { get; } = new();

        /// <summary>
        /// A <see cref="Vector4"/> that represents the margins in the cardinal directions in the following order:
        /// top (X), right (Y), bottom (Z), left (W).
        /// </summary>
        public Vector4 Margins { get; } = new();

        public ElementBounds() { }

        public ElementBounds(Rect boundingRect, Vector4 margins)
        {
            BoundingRect = boundingRect;
            Margins = margins;
        }

        public ElementBounds(ElementBounds other)
        {
            BoundingRect = other.BoundingRect;
            Margins = other.Margins;
        }

        public override string ToString()
        {
            string marginText = $"({Margins.X}, {Margins.Y}, {Margins.Z}, {Margins.W})";

            return $"{{Rect:{BoundingRect}, Margin:{marginText}}}";
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public ElementBounds Duplicate()
        {
            return new ElementBounds(BoundingRect, Margins);
        }

        public Rect GetElementBox()
        {
            var rect = new Rect(
                BoundingRect.X - Margins.W,
                BoundingRect.Y - Margins.X,
                BoundingRect.Width + Margins.Y,
                BoundingRect.Height + Margins.Z
            );
            return rect;
        }

        public Rect GetContentBox()
        {
            return BoundingRect;
        }
    }
}
