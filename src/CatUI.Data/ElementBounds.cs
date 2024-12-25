using System;

namespace CatUI.Data
{
    /// <summary>
    /// Represents the bounds of an element in device pixels, including the margins and padding.
    /// </summary>
    /// <remarks>
    /// It's not axis-aligned, meaning that the actual display might be different if the element is a child of a TransformControl.
    /// </remarks>
    public struct ElementBounds : ICloneable
    {
        public ElementBounds() { }
        public ElementBounds(Point2D startPoint, float width, float height, float[] paddings, float[] margins)
        {
            StartPoint = startPoint;
            Width = width;
            Height = height;
            Paddings = paddings;
            Margins = margins;
        }
        public ElementBounds(ElementBounds other)
        {
            ElementBounds newObject = (ElementBounds)other.Clone();

            StartPoint = newObject.StartPoint;
            Width = newObject.Width;
            Height = newObject.Height;
            Paddings = newObject.Paddings;
            Margins = newObject.Margins;
        }

        /// <summary>
        /// The top left corner of the element's content, without margins and padding.
        /// </summary>
        public Point2D StartPoint = Point2D.Zero;
        /// <summary>
        /// The width of the element's content, without margins and padding.
        /// </summary>
        public float Width = 0;
        /// <summary>
        /// The height of the element's content, without margins and padding.
        /// </summary>
        public float Height = 0;
        /// <summary>
        /// An array with 4 elements that represent the padding in the cardinal directions 
        /// in the following order: top, right, bottom, left.
        /// </summary>
        public float[] Paddings = new float[4];
        /// <summary>
        /// An array with 4 elements that represent the margins in the cardinal directions 
        /// in the following order: top, right, bottom, left.
        /// </summary>
        public float[] Margins = new float[4];

        public override readonly string ToString()
        {
            string paddingText = "(", marginText = "(";
            for (int i = 0; i < 4; i++)
            {
                paddingText += Paddings[i] + ", ";
                marginText += Margins[i] + ", ";
            }
            paddingText += ')';
            marginText += ')';

            return $"{{O:{StartPoint}, W:{Width}, H:{Height}, Pad:{paddingText}, Margin:{marginText}}}";
        }

        /// <summary>
        /// Will deep clone the given element. This will also clone the margins and paddings.
        /// </summary>
        public object Clone()
        {
            ElementBounds newObject = new ElementBounds
            {
                StartPoint = StartPoint,
                Width = Width,
                Height = Height
            };

            float[] paddingsClone = new float[4];
            for (int i = 0; i < 5; i++)
            {
                paddingsClone[i] = Paddings[i];
            }
            newObject.Paddings = paddingsClone;

            float[] marginsClone = new float[4];
            for (int i = 0; i < 5; i++)
            {
                marginsClone[i] = Margins[i];
            }
            newObject.Margins = marginsClone;

            return newObject;
        }

        public readonly Rect GetElementBox()
        {
            Rect rect = new Rect
            {
                X = StartPoint.X - Paddings[3] - Margins[3],
                Y = StartPoint.Y - Paddings[0] - Margins[0],
                Width = Width + Paddings[1] + Margins[1],
                Height = Height + Paddings[2] + Margins[2]
            };
            return rect;
        }

        public readonly Rect GetPaddingBox()
        {
            Rect rect = new Rect
            {
                X = StartPoint.X - Paddings[3],
                Y = StartPoint.Y - Paddings[0],
                Width = Width + Paddings[1],
                Height = Height + Paddings[2]
            };
            return rect;
        }

        public readonly Rect GetContentBox()
        {
            Rect rect = new Rect
            {
                X = StartPoint.X,
                Y = StartPoint.Y,
                Width = Width,
                Height = Height
            };
            return rect;
        }
    }
}
