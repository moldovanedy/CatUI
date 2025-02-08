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

        public Rectangle(
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                fillBrush,
                outlineBrush,
                preferredWidth,
                preferredHeight)
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
                PreferredWidth = new Dimension(rectDescriptor.Width);
            }

            if (rectDescriptor.Height != 0)
            {
                PreferredHeight = new Dimension(rectDescriptor.Height);
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

            Rect rect = Bounds.GetContentBox();
            Document?.Renderer.DrawRect(rect, FillBrush);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                return;
            }

            Document?.Renderer.DrawRectOutline(rect, OutlineBrush, OutlineParameters);
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
                PreferredWidth = PreferredWidth,
                PreferredHeight = PreferredHeight,
                MinWidth = MinWidth,
                MinHeight = MinHeight,
                MaxWidth = MaxWidth,
                MaxHeight = MaxHeight,
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
