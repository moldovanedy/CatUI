using System;

namespace CatUI.Data.Shapes
{
    public class RoundedRectangleClipShape : ClipShape
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

        public RoundedRectangleClipShape() { }

        /// <summary>
        /// Creates a rounded rectangle with a radius for each of the 4 corners as a normal, circular corner.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="bottomRight"></param>
        public RoundedRectangleClipShape(
            Dimension topLeft,
            Dimension topRight,
            Dimension bottomLeft,
            Dimension bottomRight)
        {
            TopLeftRadius = topLeft;
            TopRightRadius = topRight;
            BottomLeftRadius = bottomLeft;
            BottomRightRadius = bottomRight;
        }

        /// <summary>
        /// Creates a rounded rectangle with a single radius given to all the corners (circular corners).
        /// </summary>
        /// <param name="cornerRadius">The radius for all corners.</param>
        public RoundedRectangleClipShape(Dimension cornerRadius)
        {
            TopLeftRadius = cornerRadius;
            TopRightRadius = cornerRadius;
            BottomLeftRadius = cornerRadius;
            BottomRightRadius = cornerRadius;
        }

        /// <summary>
        /// Creates a rounded rectangle with a radius for each of the 4 corners as an elliptical corner.
        /// </summary>
        /// <param name="topLeftEllipse"></param>
        /// <param name="topRightEllipse"></param>
        /// <param name="bottomLeftEllipse"></param>
        /// <param name="bottomRightEllipse"></param>
        public RoundedRectangleClipShape(
            Dimension2 topLeftEllipse,
            Dimension2 topRightEllipse,
            Dimension2 bottomLeftEllipse,
            Dimension2 bottomRightEllipse)
        {
            TopLeftEllipse = topLeftEllipse;
            TopRightEllipse = topRightEllipse;
            BottomLeftEllipse = bottomLeftEllipse;
            BottomRightEllipse = bottomRightEllipse;
        }

        /// <summary>
        /// Creates a rounded rectangle with a single radius given to all the corners (elliptical corners).
        /// </summary>
        /// <param name="ellipticalCornerRadius">The radius for all corners.</param>
        public RoundedRectangleClipShape(Dimension2 ellipticalCornerRadius)
        {
            TopLeftEllipse = ellipticalCornerRadius;
            TopRightEllipse = ellipticalCornerRadius;
            BottomLeftEllipse = ellipticalCornerRadius;
            BottomRightEllipse = ellipticalCornerRadius;
        }

        /// <inheritdoc cref="ClipShape.IsPointInside"/>
        /// <remarks>
        /// This calculation is pretty fast, but slower that <see cref="RectangleClipShape"/>. However, the difference
        /// is negligible unless you have thousands of these shapes.
        /// </remarks>
        public override bool IsPointInside(Point2D point, Rect bounds, float contentScale, Size viewportSize)
        {
            //firstly, check as if it's a normal rectangle
            float endX = bounds.X + bounds.Width;
            float endY = bounds.Y + bounds.Height;

            if (point.X >= endX || point.X <= bounds.X)
            {
                return false;
            }

            if (point.Y >= endY || point.Y <= bounds.Y)
            {
                return false;
            }

            //secondly, check for each rectangle in the corners of the big rectangle (this rectangle is either a square of
            // radius r (e.g. TopLeftRadius) or a rectangle of width rx (TopLeftEllipse.X) and height ry (TopLeftEllipse.Y))
            for (int i = 0; i < 4; i++)
            {
                bool isElliptical = false;

                float width = 0;
                switch (i)
                {
                    case 0:
                        isElliptical = TopLeftRadius.IsUnset();
                        width = GetSmallWidth(TopLeftRadius, TopLeftEllipse, bounds, contentScale, viewportSize);
                        break;
                    case 1:
                        isElliptical = TopRightRadius.IsUnset();
                        width = GetSmallWidth(TopRightRadius, TopRightEllipse, bounds, contentScale, viewportSize);
                        break;
                    case 2:
                        isElliptical = BottomRightRadius.IsUnset();
                        width = GetSmallWidth(BottomRightRadius, BottomRightEllipse, bounds, contentScale,
                            viewportSize);
                        break;
                    case 3:
                        isElliptical = BottomLeftRadius.IsUnset();
                        width = GetSmallWidth(BottomLeftRadius, BottomLeftEllipse, bounds, contentScale, viewportSize);
                        break;
                }

                width = Math.Abs(width);

                float height = 0;
                switch (i)
                {
                    case 0:
                        height = GetSmallHeight(TopLeftRadius, TopLeftEllipse, bounds, contentScale, viewportSize);
                        break;
                    case 1:
                        height = GetSmallHeight(TopRightRadius, TopRightEllipse, bounds, contentScale, viewportSize);
                        break;
                    case 2:
                        height = GetSmallHeight(BottomRightRadius, BottomRightEllipse, bounds, contentScale,
                            viewportSize);
                        break;
                    case 3:
                        height = GetSmallHeight(BottomLeftRadius, BottomLeftEllipse, bounds, contentScale,
                            viewportSize);
                        break;
                }

                height = Math.Abs(height);

                float x = 0;
                switch (i)
                {
                    case 0:
                    case 3:
                        x = bounds.X;
                        break;
                    case 1:
                    case 2:
                        x = bounds.X + (bounds.Width - width);
                        break;
                }

                float y = 0;
                switch (i)
                {
                    case 0:
                    case 1:
                        y = bounds.Y;
                        break;
                    case 2:
                    case 3:
                        y = bounds.Y + (bounds.Height - height);
                        break;
                }

                //clamp the radii to reasonable values
                if (width > bounds.Width / 2f)
                {
                    if (isElliptical)
                    {
                        //keep the ellipse parameters
                        height *= bounds.Width / 2f / width;
                        width = bounds.Width / 2f;
                    }
                    else
                    {
                        //we clamp the height to the width if larger because the corner must be circular
                        width = bounds.Width / 2f;
                        height = Math.Min(height, width);
                    }
                }

                if (height > bounds.Height / 2f)
                {
                    if (isElliptical)
                    {
                        width *= bounds.Height / 2f / height;
                        height = bounds.Height / 2f;
                    }
                    else
                    {
                        height = bounds.Height / 2f;
                        width = Math.Min(width, height);
                    }
                }

                Rect cornerRect = new(x, y, width, height);
                if (Rect.IsPointInside(ref cornerRect, point))
                {
                    //no need to continue, as the problem of being inside is only valid for this rectangle if we entered
                    // this if
                    return IsInsideCornerRectangle(cornerRect, point, i);
                }
            }

            //if the point wasn't in any of the 4 corner rectangles, then it must be inside the rounded rectangle
            return true;
        }

        private static float GetSmallWidth(
            Dimension circleRadius,
            Dimension2 ellipseRadius,
            Rect bounds,
            float contentScale,
            Size viewportSize)
        {
            if (circleRadius.IsUnset())
            {
                if (ellipseRadius.IsUnset())
                {
                    return 0;
                }

                return ellipseRadius.X.CalculateDimension(bounds.Width, contentScale, viewportSize);
            }

            return circleRadius.CalculateDimension(bounds.Width, contentScale, viewportSize);
        }

        private static float GetSmallHeight(
            Dimension circleRadius,
            Dimension2 ellipseRadius,
            Rect bounds,
            float contentScale,
            Size viewportSize)
        {
            if (circleRadius.IsUnset())
            {
                if (ellipseRadius.IsUnset())
                {
                    return 0;
                }

                return ellipseRadius.Y.CalculateDimension(bounds.Height, contentScale, viewportSize);
            }

            return circleRadius.CalculateDimension(bounds.Height, contentScale, viewportSize);
        }

        private static bool IsInsideCornerRectangle(Rect rect, Point2D pointToCheck, int outerCorner)
        {
            //the corner of interest is the opposite of the outer corner
            int cornerOfInterest = (outerCorner + 2) % 4;
            float xOffset = 0, yOffset = 0;

            switch (cornerOfInterest)
            {
                case 0:
                    xOffset = rect.X - pointToCheck.X;
                    yOffset = rect.Y - pointToCheck.Y;
                    break;
                case 1:
                    xOffset = rect.X + rect.Width - pointToCheck.X;
                    yOffset = rect.Y - pointToCheck.Y;
                    break;
                case 2:
                    xOffset = rect.X + rect.Width - pointToCheck.X;
                    yOffset = rect.Y + rect.Height - pointToCheck.Y;
                    break;
                case 3:
                    xOffset = rect.X - pointToCheck.X;
                    yOffset = rect.Y + rect.Height - pointToCheck.Y;
                    break;
            }

            //if it's a square, calculate using the circle formula, otherwise calculate using the ellipse formula;
            //the radii are the rect's width and height because we only have a quarter of a circle/ellipse here, so no
            //need to divide by 2
            if (Math.Abs(rect.Width - rect.Height) < 0.01)
            {
                return (xOffset * xOffset) + (yOffset * yOffset) <= rect.Height * rect.Height;
            }

            return
                (xOffset * xOffset / (rect.Width * rect.Width)) +
                (yOffset * yOffset / (rect.Height * rect.Height)) <= 1;
        }

        public override RoundedRectangleClipShape Duplicate()
        {
            return new RoundedRectangleClipShape
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
