namespace CatUI.Data
{
    /// <summary>
    /// A set of offsets in each of the 4 rectangle corners: top-left, top-right, bottom-right, bottom-left.
    /// </summary>
    /// <remarks>
    /// This is generally used for corner radius.
    /// </remarks>
    public class CornerInset
    {
        public CornerInset() { }
        public CornerInset(Dimension dimension)
        {
            TopLeft = dimension;
            TopRight = dimension;
            BottomRight = dimension;
            BottomLeft = dimension;
        }
        public CornerInset(Dimension topLeft, Dimension topRight, Dimension bottomRight, Dimension bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }
        /// <summary>
        /// Creates a corner inset as an ellipse, where each corner has 2 dimensions: one for X axis and one for Y axis.
        /// </summary>
        /// <param name="topLeftEllipse"></param>
        /// <param name="topRightEllipse"></param>
        /// <param name="bottomRightEllipse"></param>
        /// <param name="bottomLeftEllipse"></param>
        public CornerInset(
            Dimension2 topLeftEllipse,
            Dimension2 topRightEllipse,
            Dimension2 bottomRightEllipse,
            Dimension2 bottomLeftEllipse)
        {
            TopLeftEllipse = topLeftEllipse;
            TopRightEllipse = topRightEllipse;
            BottomRightEllipse = bottomRightEllipse;
            BottomLeftEllipse = bottomLeftEllipse;
        }

        public Dimension TopLeft { get; set; } = new Dimension();
        public Dimension TopRight { get; set; } = new Dimension();
        public Dimension BottomRight { get; set; } = new Dimension();
        public Dimension BottomLeft { get; set; } = new Dimension();

        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value.
        /// Default is an invalid value as normally the more simple, circle corners are used. Setting this to other than invalid will override the simple value.
        /// </summary>
        public Dimension2 TopLeftEllipse { get; set; } = Dimension2.Unset;
        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value.
        /// Default is an invalid value as normally the more simple, circle corners are used. Setting this to other than invalid will override the simple value.
        /// </summary>
        public Dimension2 TopRightEllipse { get; set; } = Dimension2.Unset;
        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value.
        /// Default is an invalid value as normally the more simple, circle corners are used. Setting this to other than invalid will override the simple value.
        /// </summary>
        public Dimension2 BottomRightEllipse { get; set; } = Dimension2.Unset;
        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value.
        /// Default is an invalid value as normally the more simple, circle corners are used. Setting this to other than invalid will override the simple value.
        /// </summary>
        public Dimension2 BottomLeftEllipse { get; set; } = Dimension2.Unset;

        /// <summary>
        /// If true, it means that at least one of the corners has a value that is NOT 0.
        /// </summary>
        public bool HasNonTrivialValues
        {
            get
            {
                return
                    !(TopLeft.Value == 0 &&
                    TopRight.Value == 0 &&
                    BottomRight.Value == 0 &&
                    BottomLeft.Value == 0 &&

                    TopLeftEllipse.X.Value == 0 &&
                    TopLeftEllipse.Y.Value == 0 &&
                    TopRightEllipse.X.Value == 0 &&
                    TopRightEllipse.Y.Value == 0 &&
                    BottomRightEllipse.X.Value == 0 &&
                    BottomRightEllipse.Y.Value == 0 &&
                    BottomLeftEllipse.X.Value == 0 &&
                    BottomLeftEllipse.Y.Value == 0 &&

                    float.IsNaN(TopLeft.Value) &&
                    float.IsNaN(TopRight.Value) &&
                    float.IsNaN(BottomRight.Value) &&
                    float.IsNaN(BottomLeft.Value) &&

                    float.IsNaN(TopLeftEllipse.X.Value) &&
                    float.IsNaN(TopLeftEllipse.Y.Value) &&
                    float.IsNaN(TopRightEllipse.X.Value) &&
                    float.IsNaN(TopRightEllipse.Y.Value) &&
                    float.IsNaN(BottomRightEllipse.X.Value) &&
                    float.IsNaN(BottomRightEllipse.Y.Value) &&
                    float.IsNaN(BottomLeftEllipse.X.Value) &&
                    float.IsNaN(BottomLeftEllipse.Y.Value));
            }
        }

        public override string ToString()
        {
            //TODO: also add case for elliptic insets
            return $"({TopLeft}, {TopRight}, {BottomRight}, {BottomLeft})";
        }
    }
}
