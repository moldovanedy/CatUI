using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.RenderingEngine;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements.Shapes
{
    public class RoundedRectangleElement : AbstractShapeElement
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<RoundedRectangleElement>? Ref
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

        private ObjectRef<RoundedRectangleElement>? _ref;

        public override ClipShape CorrespondingClipShape => _clipShape;

        private readonly ClipShape _clipShape;

        public CornerInset RoundCornersDescriptor
        {
            get => _roundCornersDescriptor;
            set => SetRoundCornersDescriptor(value);
        }

        private CornerInset _roundCornersDescriptor = new();
        public ObservableProperty<CornerInset> RoundCornersDescriptorProperty { get; } = new(new CornerInset());

        private void SetRoundCornersDescriptor(CornerInset? value)
        {
            if (value != null)
            {
                _roundCornersDescriptor = value;
                if (_clipShape is RoundedRectangleClipShape clipShape)
                {
                    clipShape.RoundCornersDescriptor = _roundCornersDescriptor;
                }

                RequestRedraw();
            }
        }

        public RoundedRectangleElement(IBrush? fillBrush = null, IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            RoundCornersDescriptorProperty.ValueChangedEvent += SetRoundCornersDescriptor;
            _clipShape = new RoundedRectangleClipShape();
        }

        /// <summary>
        /// Constructs a rounded rectangle given a Rect descriptor that has the X, Y, Width, and Height, but not the corner
        /// radii (those need to be set separately using the available properties).
        /// </summary>
        /// <param name="rectDescriptor"></param>
        /// <param name="fillBrush"></param>
        /// <param name="outlineBrush"></param>
        public RoundedRectangleElement(
            Rect rectDescriptor,
            IBrush? fillBrush = null,
            IBrush? outlineBrush = null)
            : base(fillBrush, outlineBrush)
        {
            RoundCornersDescriptorProperty.ValueChangedEvent += SetRoundCornersDescriptor;
            _clipShape = new RoundedRectangleClipShape();

            Position = new Dimension2(rectDescriptor.X, rectDescriptor.Y);
            Layout =
                new ElementLayout()
                    .SetFixedWidth(Math.Abs(rectDescriptor.Width))
                    .SetFixedHeight(Math.Abs(rectDescriptor.Height));
        }

        protected override void DrawBackground()
        {
            if (!IsCurrentlyVisible)
            {
                return;
            }

            Renderer? renderer = Document?.Renderer;
            if (renderer == null)
            {
                return;
            }

            SKPath clipPath = _clipShape.GetSkiaClipPath(
                Bounds,
                Document?.ContentScale ?? 1f,
                Document?.ViewportSize ?? new Size());

            int saveCount = renderer.SaveCanvasState();
            renderer.SetClipPath(clipPath);
            renderer.DrawRect(Bounds, FillBrush, RoundCornersDescriptor);

            if (OutlineBrush.IsSkippable || OutlineParameters.OutlineWidth == 0)
            {
                renderer.RestoreCanvasState(saveCount);
                return;
            }

            Document?.Renderer.DrawRectOutline(Bounds, OutlineBrush, OutlineParameters, RoundCornersDescriptor);
            renderer.RestoreCanvasState(saveCount);
        }

        public override RoundedRectangleElement Duplicate()
        {
            RoundedRectangleElement el = new()
            {
                RoundCornersDescriptor = RoundCornersDescriptor.Duplicate(),
                //AbstractShapeElement
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
