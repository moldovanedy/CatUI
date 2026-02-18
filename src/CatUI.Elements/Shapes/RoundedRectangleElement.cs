using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Shapes;
using CatUI.RenderingEngine;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements.Shapes;

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
    public ObservableProperty<CornerInset> RoundCornersDescriptorProperty
    {
        get => _roundCornersDescriptorProperty;
        set => _roundCornersDescriptorProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<CornerInset> _roundCornersDescriptorProperty = new(new CornerInset());

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
        Init();
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
        Init();
        _clipShape = new RoundedRectangleClipShape();

        Position = new Dimension2(rectDescriptor.X, rectDescriptor.Y);
        Layout =
            new ElementLayout()
                .SetFixedWidth(Math.Abs(rectDescriptor.Width))
                .SetFixedHeight(Math.Abs(rectDescriptor.Height));
    }

    public RoundedRectangleElement(RoundedRectangleElement other) : base(other)
    {
        Init();
        RoundCornersDescriptor = other.RoundCornersDescriptor.Duplicate();
        _clipShape = new RoundedRectangleClipShape();
    }

    private void Init()
    {
        RoundCornersDescriptorProperty.ValueChangedEvent += SetRoundCornersDescriptor;
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
            Document?.FramebufferSize ?? new Size());

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
        var el = new RoundedRectangleElement(this);
        DuplicateChildrenUtil(el);
        return el;
    }
}
