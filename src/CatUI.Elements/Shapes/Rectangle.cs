using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Utils;

namespace CatUI.Elements.Shapes
{
    /// <summary>
    /// Draws a rectangle, either filled, outlined or both. If the rectangle is both filled and outlined, the filled
    /// area will have the size of the element and the outline will exceed the element bounds by half of the outline width
    /// on each size. The outline will also overlap with the filled area by half of the outline width on each side.
    /// </summary>
    public class Rectangle : AbstractShapeElement
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Rectangle>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<Rectangle>? _ref;

        public Rectangle(IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
        }

        /// <summary>
        /// Creates a new rectangle from the given <see cref="Rect"/> object. The coordinates are fixed and are
        /// represented as <see cref="Element.Position"/> and for <see cref="Element.Layout"/> it sets
        /// <see cref="ElementLayout.SetFixedWidth"/> and <see cref="ElementLayout.SetFixedHeight"/> respectively.
        /// </summary>
        /// <param name="rectDescriptor">Serves as the basis upon which the element's position and size are set.</param>
        /// <param name="fillBrush">Sets <see cref="AbstractShapeElement.FillBrush"/>.</param>
        /// <param name="outlineBrush">Sets <see cref="AbstractShapeElement.OutlineBrush"/>.</param>
        public Rectangle(
            Rect rectDescriptor,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            Position = new Dimension2(rectDescriptor.X, rectDescriptor.Y);
            Layout.SetFixedWidth(Math.Abs(rectDescriptor.Width)).SetFixedHeight(Math.Abs(rectDescriptor.Height));
        }

        protected override void DrawBackground()
        {
            if (!Visible)
            {
                return;
            }

            if (FillBrush.IsSkippable)
            {
                return;
            }

            Document?.Renderer.DrawRect(Bounds, FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawRectOutline(Bounds, OutlineBrush, OutlineParameters);
        }

        public override Rectangle Duplicate()
        {
            return new Rectangle
            {
                FillBrush = FillBrush.Duplicate(),
                OutlineBrush = OutlineBrush.Duplicate(),
                OutlineParameters = OutlineParameters,
                //
                Position = Position,
                Background = Background.Duplicate(),
                CornerRadius = CornerRadius,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate()
            };
        }
    }
}
