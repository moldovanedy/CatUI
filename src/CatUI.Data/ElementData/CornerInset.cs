namespace CatUI.Data.ElementData
{
    /// <summary>
    /// A set of offsets in each of the 4 rectangle corners: top-left, top-right, bottom-right, bottom-left.
    /// </summary>
    /// <remarks>
    /// This is generally used for corner radius.
    /// </remarks>
    public class CornerInset
    {
        /// <summary>
        /// Represents the radius of the top-left corner. Default is <see cref="Dimension.Unset"/> or 0. Setting this
        /// to anything other than <see cref="Dimension.Unset"/> will overwrite the value of <see cref="TopLeftEllipse"/>
        /// to be <see cref="Dimension2.Unset"/>.
        /// </summary>
        public Dimension TopLeftRadius
        {
            get => _topLeftRadius;
            set
            {
                _topLeftRadius = value;
                if (value != Dimension.Unset)
                {
                    _topLeftEllipse = Dimension2.Unset;
                }
            }
        }

        private Dimension _topLeftRadius = Dimension.Unset;

        /// <summary>
        /// Represents the radius of the top-right corner. Default is <see cref="Dimension.Unset"/> or 0. Setting this
        /// to anything other than <see cref="Dimension.Unset"/> will overwrite the value of <see cref="TopRightEllipse"/>
        /// to be <see cref="Dimension2.Unset"/>.
        /// </summary>
        public Dimension TopRightRadius
        {
            get => _topRightRadius;
            set
            {
                _topRightRadius = value;
                if (value != Dimension.Unset)
                {
                    _topRightEllipse = Dimension2.Unset;
                }
            }
        }

        private Dimension _topRightRadius = Dimension.Unset;

        /// <summary>
        /// Represents the radius of the bottom-left corner. Default is <see cref="Dimension.Unset"/> or 0. Setting this
        /// to anything other than <see cref="Dimension.Unset"/> will overwrite the value of <see cref="BottomLeftEllipse"/>
        /// to be <see cref="Dimension2.Unset"/>.
        /// </summary>
        public Dimension BottomLeftRadius
        {
            get => _bottomLeftRadius;
            set
            {
                _bottomLeftRadius = value;
                if (value != Dimension.Unset)
                {
                    _bottomLeftEllipse = Dimension2.Unset;
                }
            }
        }

        private Dimension _bottomLeftRadius = Dimension.Unset;

        /// <summary>
        /// Represents the radius of the bottom-right corner. Default is <see cref="Dimension.Unset"/> or 0. Setting this
        /// to anything other than <see cref="Dimension.Unset"/> will overwrite the value of <see cref="BottomRightEllipse"/>
        /// to be <see cref="Dimension2.Unset"/>.
        /// </summary>
        public Dimension BottomRightRadius
        {
            get => _bottomRightRadius;
            set
            {
                _bottomRightRadius = value;
                if (value != Dimension.Unset)
                {
                    _bottomRightEllipse = Dimension2.Unset;
                }
            }
        }

        private Dimension _bottomRightRadius = Dimension.Unset;

        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value for
        /// the top-left corner. Default is <see cref="Dimension2.Unset"/> as normally the more simple, circle corners are used.
        /// Setting this anything other than <see cref="Dimension2.Unset"/> will override the simple value
        /// (<see cref="TopLeftRadius"/>).
        /// </summary>
        public Dimension2 TopLeftEllipse
        {
            get => _topLeftEllipse;
            set
            {
                _topLeftEllipse = value;
                if (value != Dimension2.Unset)
                {
                    _topLeftRadius = Dimension.Unset;
                }
            }
        }

        private Dimension2 _topLeftEllipse = Dimension2.Unset;

        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value for
        /// the top-right corner. Default is <see cref="Dimension2.Unset"/> as normally the more simple, circle corners are used.
        /// Setting this anything other than <see cref="Dimension2.Unset"/> will override the simple value
        /// (<see cref="TopRightRadius"/>).
        /// </summary>
        public Dimension2 TopRightEllipse
        {
            get => _topRightEllipse;
            set
            {
                _topRightEllipse = value;
                if (value != Dimension2.Unset)
                {
                    _topRightRadius = Dimension.Unset;
                }
            }
        }

        private Dimension2 _topRightEllipse = Dimension2.Unset;

        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value for
        /// the bottom-left corner. Default is <see cref="Dimension2.Unset"/> as normally the more simple, circle corners are used.
        /// Setting this anything other than <see cref="Dimension2.Unset"/> will override the simple value
        /// (<see cref="BottomLeftRadius"/>).
        /// </summary>
        public Dimension2 BottomLeftEllipse
        {
            get => _bottomLeftEllipse;
            set
            {
                _bottomLeftEllipse = value;
                if (value != Dimension2.Unset)
                {
                    _bottomLeftRadius = Dimension.Unset;
                }
            }
        }

        private Dimension2 _bottomLeftEllipse = Dimension2.Unset;

        /// <summary>
        /// The X is the corner's ellipse X axis half value and the Y is the corner's ellipse Y axis half value for
        /// the bottom-right corner. Default is <see cref="Dimension2.Unset"/> as normally the more simple, circle corners are used.
        /// Setting this anything other than <see cref="Dimension2.Unset"/> will override the simple value
        /// (<see cref="BottomRightRadius"/>).
        /// </summary>
        public Dimension2 BottomRightEllipse
        {
            get => _bottomRightEllipse;
            set
            {
                _bottomRightEllipse = value;
                if (value != Dimension2.Unset)
                {
                    _bottomRightRadius = Dimension.Unset;
                }
            }
        }

        private Dimension2 _bottomRightEllipse = Dimension2.Unset;

        /// <summary>
        /// If true, it means that at least one of the corners has a value that is NOT 0.
        /// </summary>
        public bool HasNonTrivialValues =>
            !(TopLeftRadius.Value == 0 &&
              TopRightRadius.Value == 0 &&
              BottomRightRadius.Value == 0 &&
              BottomLeftRadius.Value == 0 &&
              TopLeftEllipse.X.Value == 0 &&
              TopLeftEllipse.Y.Value == 0 &&
              TopRightEllipse.X.Value == 0 &&
              TopRightEllipse.Y.Value == 0 &&
              BottomRightEllipse.X.Value == 0 &&
              BottomRightEllipse.Y.Value == 0 &&
              BottomLeftEllipse.X.Value == 0 &&
              BottomLeftEllipse.Y.Value == 0 &&
              float.IsNaN(TopLeftRadius.Value) &&
              float.IsNaN(TopRightRadius.Value) &&
              float.IsNaN(BottomRightRadius.Value) &&
              float.IsNaN(BottomLeftRadius.Value) &&
              float.IsNaN(TopLeftEllipse.X.Value) &&
              float.IsNaN(TopLeftEllipse.Y.Value) &&
              float.IsNaN(TopRightEllipse.X.Value) &&
              float.IsNaN(TopRightEllipse.Y.Value) &&
              float.IsNaN(BottomRightEllipse.X.Value) &&
              float.IsNaN(BottomRightEllipse.Y.Value) &&
              float.IsNaN(BottomLeftEllipse.X.Value) &&
              float.IsNaN(BottomLeftEllipse.Y.Value));

        public CornerInset() { }

        /// <summary>
        /// Creates a CornerInset with a radius for each one of the 4 corners (circular corners).
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomRight"></param>
        /// <param name="bottomLeft"></param>
        public CornerInset(Dimension topLeft, Dimension topRight, Dimension bottomRight, Dimension bottomLeft)
        {
            TopLeftRadius = topLeft;
            TopRightRadius = topRight;
            BottomRightRadius = bottomRight;
            BottomLeftRadius = bottomLeft;
        }

        /// <summary>
        /// Creates a CornerInset with all corners having the given radius (circular corners).
        /// </summary>
        /// <param name="radius">The corner radius.</param>
        public CornerInset(Dimension radius)
        {
            TopLeftRadius = radius;
            TopRightRadius = radius;
            BottomRightRadius = radius;
            BottomLeftRadius = radius;
        }

        /// <summary>
        /// Creates a CornerInset with elliptical corners, where each corner has 2 dimensions: one for X axis and one
        /// for Y axis.
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

        /// <summary>
        /// Creates a CornerInset with all corners having the given elliptical radius.
        /// </summary>
        /// <param name="ellipticalRadius">The corner elliptical radius.</param>
        public CornerInset(Dimension2 ellipticalRadius)
        {
            TopLeftEllipse = ellipticalRadius;
            TopRightEllipse = ellipticalRadius;
            BottomRightEllipse = ellipticalRadius;
            BottomLeftEllipse = ellipticalRadius;
        }

        public override string ToString()
        {
            //TODO: also add case for elliptic insets
            return $"({TopLeftRadius}, {TopRightRadius}, {BottomRightRadius}, {BottomLeftRadius})";
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public CornerInset Duplicate()
        {
            return new CornerInset
            {
                TopLeftEllipse = TopLeftEllipse,
                TopRightEllipse = TopRightEllipse,
                BottomLeftEllipse = BottomLeftEllipse,
                BottomRightEllipse = BottomRightEllipse,
                TopLeftRadius = TopLeftRadius,
                TopRightRadius = TopRightRadius,
                BottomLeftRadius = BottomLeftRadius,
                BottomRightRadius = BottomRightRadius
            };
        }
    }
}
