using CatUI.Data.Shapes;

namespace CatUI.Data.Theming.ClipShapes
{
    /// <summary>
    /// A clip shape utility for your application where you can use different rounded corner values. All the properties
    /// will always return a new shape, so a duplicate of the one stored internally. This means that changing a value
    /// from here will not affect the rest of the app.
    /// </summary>
    public class CatThemeClipShapes
    {
        public CatThemeClipShapes()
        {
            SmallRoundingProperty.ValueChangedEvent += SetSmallRounding;
            MediumRoundingProperty.ValueChangedEvent += SetMediumRounding;
            LargeRoundingProperty.ValueChangedEvent += SetLargeRounding;
            XlRoundingProperty.ValueChangedEvent += SetXlRounding;
        }

        ~CatThemeClipShapes()
        {
            SmallRoundingProperty = null!;
            MediumRoundingProperty = null!;
            LargeRoundingProperty = null!;
            XlRoundingProperty = null!;
        }

#pragma warning disable CA1822
        /// <summary>
        /// Returns a new rectangle clip shape with no rounding on corners. The returned value is a duplicate of the
        /// internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        /// <remarks>
        /// This will always return the same value and cannot be changed. Therefore, it might be used by some elements.
        /// </remarks>
        public RectangleClipShape NoRounding => new();

        /// <summary>
        /// Returns a new rounded rectangle clip shape with all corners fully rounded. The returned value is a duplicate
        /// of the internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        /// <remarks>
        /// This will always return the same value and cannot be changed. Therefore, it might be used by some elements.
        /// </remarks>
        public RoundedRectangleClipShape FullRounding => new((Dimension)"50%");
#pragma warning restore CA1822

        /// <summary>
        /// Returns a new rounded rectangle clip shape with a small rounding on corners. The returned value is a
        /// duplicate of the internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        public RoundedRectangleClipShape SmallRounding
        {
            get => _smallRounding.Duplicate();
            internal set
            {
                SetSmallRounding(value);
                SmallRoundingProperty.Value = value;
            }
        }

        private RoundedRectangleClipShape _smallRounding = new(4);

        public ObservableProperty<RoundedRectangleClipShape> SmallRoundingProperty { get; private set; }
            = new(new RoundedRectangleClipShape(4));

        private void SetSmallRounding(RoundedRectangleClipShape? value)
        {
            value ??= new RoundedRectangleClipShape(4);
            _smallRounding = value;
        }

        /// <summary>
        /// Returns a new rounded rectangle clip shape with a medium rounding on corners. The returned value is a
        /// duplicate of the internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        public RoundedRectangleClipShape MediumRounding
        {
            get => _mediumRounding.Duplicate();
            internal set
            {
                SetMediumRounding(value);
                MediumRoundingProperty.Value = value;
            }
        }

        private RoundedRectangleClipShape _mediumRounding = new(8);

        public ObservableProperty<RoundedRectangleClipShape> MediumRoundingProperty { get; private set; }
            = new(new RoundedRectangleClipShape(8));

        private void SetMediumRounding(RoundedRectangleClipShape? value)
        {
            value ??= new RoundedRectangleClipShape(8);
            _mediumRounding = value;
        }

        /// <summary>
        /// Returns a new rounded rectangle clip shape with a large rounding on corners. The returned value is a
        /// duplicate of the internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        public RoundedRectangleClipShape LargeRounding
        {
            get => _largeRounding.Duplicate();
            internal set
            {
                SetLargeRounding(value);
                LargeRoundingProperty.Value = value;
            }
        }

        private RoundedRectangleClipShape _largeRounding = new(16);

        public ObservableProperty<RoundedRectangleClipShape> LargeRoundingProperty { get; private set; }
            = new(new RoundedRectangleClipShape(16));

        private void SetLargeRounding(RoundedRectangleClipShape? value)
        {
            value ??= new RoundedRectangleClipShape(16);
            _largeRounding = value;
        }

        /// <summary>
        /// Returns a new rounded rectangle clip shape with a very large rounding on corners. The returned value is a
        /// duplicate of the internal one, so modifying it won't affect the other usages of this property.
        /// </summary>
        public RoundedRectangleClipShape XlRounding
        {
            get => _xlRounding.Duplicate();
            internal set
            {
                SetXlRounding(value);
                XlRoundingProperty.Value = value;
            }
        }

        private RoundedRectangleClipShape _xlRounding = new(28);

        public ObservableProperty<RoundedRectangleClipShape> XlRoundingProperty { get; private set; }
            = new(new RoundedRectangleClipShape(28));

        private void SetXlRounding(RoundedRectangleClipShape? value)
        {
            value ??= new RoundedRectangleClipShape(28);
            _xlRounding = value;
        }


        /// <summary>
        /// Given a symmetrically rounded shape, it will create a new shape that will only have 2 rounded corners in the
        /// given direction, the other 2 will not be rounded.
        /// </summary>
        /// <param name="originalShape">
        /// The symmetrically rounded shape. Giving an arbitrary shape that has for example elliptic corners will create
        /// unexpected results. You should always give a shape from the properties of this class.
        /// </param>
        /// <param name="direction">The direction in which to round the shapes.</param>
        /// <returns>A new, asymmetrically rounded shape.</returns>
        public static RoundedRectangleClipShape GenerateWithAsymmetry(
            RoundedRectangleClipShape originalShape,
            AsymmetryDirection direction)
        {
            Dimension radius = originalShape.TopLeftRadius;

            switch (direction)
            {
                default:
                case AsymmetryDirection.Top:
                    return new RoundedRectangleClipShape(radius, radius, 0, 0);
                case AsymmetryDirection.Right:
                    return new RoundedRectangleClipShape(0, radius, radius, 0);
                case AsymmetryDirection.Bottom:
                    return new RoundedRectangleClipShape(0, 0, radius, radius);
                case AsymmetryDirection.Left:
                    return new RoundedRectangleClipShape(radius, 0, 0, radius);
            }
        }
    }

    public enum AsymmetryDirection
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }
}
