using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Utils;

namespace CatUI.Elements.Shapes
{
    /// <summary>
    /// Draws an ellipse (oval), either filled, outlined or both. If the ellipse is both filled and outlined, the filled
    /// area will have the size of the element and the outline will exceed the element bounds by half of the outline width
    /// on each size. The outline will also overlap with the filled area by half of the outline width on each side.
    /// </summary>
    public class Ellipse : AbstractShape
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Ellipse>? Ref
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

        private ObjectRef<Ellipse>? _ref;

        public Ellipse(IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
        }

        /// <summary>
        /// Creates an ellipse element with the given coordinates of the center, the radius on X axis and the radius on
        /// Y axis. The coordinates are fixed and are represented as <see cref="Element.Position"/> and for
        /// <see cref="Element.Layout"/> it sets <see cref="ElementLayout.SetFixedWidth"/> and
        /// <see cref="ElementLayout.SetFixedHeight"/> respectively.
        /// </summary>
        /// <param name="centerPoint">The coordinates of the center point of the ellipse.</param>
        /// <param name="radiusX">The radius on the X axis (horizontal). This will represent half of the width.</param>
        /// <param name="radiusY">The radius on the Y axis (vertical). This will represent half of the height.</param>
        /// <param name="fillBrush">Sets <see cref="AbstractShape.FillBrush"/>.</param>
        /// <param name="outlineBrush">Sets <see cref="AbstractShape.OutlineBrush"/>.</param>
        public Ellipse(
            Point2D centerPoint,
            float radiusX,
            float radiusY,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            radiusX = Math.Abs(radiusX);
            radiusY = Math.Abs(radiusY);

            Position = new Dimension2(centerPoint.X - radiusX, centerPoint.Y - radiusY);
            Layout.SetFixedWidth(radiusX * 2f).SetFixedHeight(radiusY * 2f);
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

            Document?.Renderer.DrawEllipse(
                new Point2D(Bounds.CenterX, Bounds.CenterY),
                Bounds.Width / 2f,
                Bounds.Height / 2f,
                FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawEllipseOutline(
                new Point2D(Bounds.CenterX, Bounds.CenterY),
                Bounds.Width / 2f,
                Bounds.Height / 2f,
                OutlineBrush,
                OutlineParameters);
        }

        public override Ellipse Duplicate()
        {
            return new Ellipse
            {
                FillBrush = FillBrush.Duplicate(),
                OutlineBrush = OutlineBrush.Duplicate(),
                OutlineParameters = OutlineParameters,
                //
                Position = Position,
                Margin = Margin,
                Background = Background.Duplicate(),
                CornerRadius = CornerRadius,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate()
            };
        }
    }
}
