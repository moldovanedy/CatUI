using System;

namespace CatUI.Data
{
    /// <summary>
    /// Represents the bounds of an element in device pixels, including the margins and padding.
    /// </summary>
    /// <remarks>
    /// It's not axis-aligned, meaning that the actual display might be differrent if the element is a child of a TransformControl.
    /// </remarks>
    public class ElementBounds : ICloneable
    {
        public ElementBounds() { }
        public ElementBounds(Point2D startPoint, float width, float height, float[] paddings, float[] margins)
        {
            this.StartPoint = startPoint;
            this.Width = width;
            this.Height = height;
            this.Paddings = paddings;
            this.Margins = margins;
        }

        /// <summary>
        /// The top left corner of the element's content, without margins and padding.
        /// </summary>
        public Point2D StartPoint { get; private set; } = Point2D.Zero;
        /// <summary>
        /// The width of the element's content, without margins and padding.
        /// </summary>
        public float Width { get; private set; } = 0;
        /// <summary>
        /// The height of the element's content, without margins and padding.
        /// </summary>
        public float Height { get; private set; } = 0;
        /// <summary>
        /// An array with 4 elements that represent the padding in the cardinal directions 
        /// in the following order: top, right, bottom, left.
        /// </summary>
        public float[] Paddings { get; private set; } = new float[4];
        /// <summary>
        /// An array with 4 elements that represent the margins in the cardinal directions 
        /// in the following order: top, right, bottom, left.
        /// </summary>
        public float[] Margins { get; private set; } = new float[4];

        /// <summary>
        /// Will deep clone the given element. This will also clone the margins and paddings.
        /// </summary>
        public object Clone()
        {
            ElementBounds newObject = new ElementBounds
            {
                StartPoint = this.StartPoint,
                Width = this.Width,
                Height = this.Height
            };

            float[] paddingsClone = new float[4];
            for (int i = 0; i < 5; i++)
            {
                paddingsClone[i] = this.Paddings[i];
            }
            newObject.Paddings = paddingsClone;

            float[] marginsClone = new float[4];
            for (int i = 0; i < 5; i++)
            {
                marginsClone[i] = this.Margins[i];
            }
            newObject.Margins = marginsClone;

            return newObject;
        }

        public Rect GetElementBox()
        {
            Rect rect = new Rect
            {
                X = this.StartPoint.X - this.Paddings[3] - this.Margins[3],
                Y = this.StartPoint.Y - this.Paddings[0] - this.Margins[0],
                Width = this.Width + this.Paddings[1] + this.Margins[1],
                Height = this.Height + this.Paddings[2] + this.Margins[2]
            };
            return rect;
        }

        public Rect GetPaddingBox()
        {
            Rect rect = new Rect
            {
                X = this.StartPoint.X - this.Paddings[3],
                Y = this.StartPoint.Y - this.Paddings[0],
                Width = this.Width + this.Paddings[1],
                Height = this.Height + this.Paddings[2]
            };
            return rect;
        }

        public Rect GetContentBox()
        {
            Rect rect = new Rect
            {
                X = this.StartPoint.X,
                Y = this.StartPoint.Y,
                Width = this.Width,
                Height = this.Height
            };
            return rect;
        }
    }
}
