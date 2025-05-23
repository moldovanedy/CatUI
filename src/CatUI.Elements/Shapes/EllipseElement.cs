﻿using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Shapes
{
    /// <summary>
    /// Draws an ellipse (oval), either filled, outlined or both. If the ellipse is both filled and outlined, the filled
    /// area will have the size of the element and the outline will exceed the element bounds by half of the outline width
    /// on each size. The outline will also overlap with the filled area by half of the outline width on each side.
    /// </summary>
    public class EllipseElement : AbstractShapeElement
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<EllipseElement>? Ref
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

        private ObjectRef<EllipseElement>? _ref;

        public EllipseElement(IBrush? fillBrush = null, IBrush? outlineBrush = null)
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
        /// <param name="fillBrush">Sets <see cref="AbstractShapeElement.FillBrush"/>.</param>
        /// <param name="outlineBrush">Sets <see cref="AbstractShapeElement.OutlineBrush"/>.</param>
        public EllipseElement(
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
            Layout = new ElementLayout().SetFixedWidth(radiusX * 2f).SetFixedHeight(radiusY * 2f);
        }

        protected override void DrawBackground()
        {
            if (!IsCurrentlyVisible)
            {
                return;
            }

            //also draw the background
            base.DrawBackground();

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

        public override EllipseElement Duplicate()
        {
            EllipseElement el = new()
            {
                FillBrush = FillBrush.Duplicate(),
                OutlineBrush = OutlineBrush.Duplicate(),
                OutlineParameters = OutlineParameters,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                LocallyVisible = LocallyVisible,
                LocallyEnabled = LocallyEnabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
