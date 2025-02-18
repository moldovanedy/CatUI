using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Shapes
{
    public class Rectangle : AbstractShape
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

        public Rectangle(
            Rect rectDescriptor,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null)
        {
            if (fillBrush != null)
            {
                FillBrush = fillBrush;
            }

            if (outlineBrush != null)
            {
                OutlineBrush = outlineBrush;
            }

            if (rectDescriptor.X != 0)
            {
                Position = new Dimension2(rectDescriptor.X, Position.Y);
            }

            if (rectDescriptor.Y != 0)
            {
                Position = new Dimension2(Position.X, rectDescriptor.Y);
            }

            if (rectDescriptor.Width != 0)
            {
                Layout.SetFixedWidth(new Dimension(rectDescriptor.Width));
            }

            if (rectDescriptor.Height != 0)
            {
                Layout.SetFixedHeight(new Dimension(rectDescriptor.Height));
            }
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
