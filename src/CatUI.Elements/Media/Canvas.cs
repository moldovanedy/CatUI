using System;
using CatUI.Data;
using CatUI.RenderingEngine;
using CatUI.Utils;

namespace CatUI.Elements.Media;

public class Canvas : Element
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<Canvas>? Ref
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

    private ObjectRef<Canvas>? _ref;

    /// <summary>
    /// The function that draws on the canvas. It is only invoked when the Canvas is "dirty" (call
    /// <see cref="Element.MarkLayoutDirty"/> or set <see cref="CanvasPen.IsDirty"/> to true for manual control)
    /// and when the Canvas is inside a document.
    /// </summary>
    public Action<CanvasPen> DrawFunction
    {
        get => _drawFunction;
        set => DrawFunctionProperty.Value = value;
    }

    private Action<CanvasPen> _drawFunction = _ => { };

    public ObservableProperty<Action<CanvasPen>> DrawFunctionProperty
    {
        get => _drawFunctionProperty;
        set => _drawFunctionProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<Action<CanvasPen>> _drawFunctionProperty = new(_ => { });

    private void SetDrawFunction(Action<CanvasPen>? value)
    {
        value ??= _ => { };

        _drawFunction = value;
        SetLocalValue(nameof(DrawFunction), value);
        MarkLayoutDirty();
    }

    public Canvas()
    {
        Init();
    }

    public Canvas(Canvas other) : base(other)
    {
        Init();
    }

    public override Canvas Duplicate()
    {
        var el = new Canvas(this);
        DuplicateChildrenUtil(el);
        return el;
    }

    protected override void Draw(object sender)
    {
        base.Draw(sender);

        if (Document?.Renderer != null)
        {
            CanvasPen pen = new(Document.Renderer);
            _drawFunction.Invoke(pen);

            if (pen.IsDirty)
            {
                Document.RequestAnimationFrame(_ =>
                {
                    Document.MarkVisualDirty();
                });
            }
        }
    }

    private void Init()
    {
        DrawFunctionProperty.ValueChangedEvent += SetDrawFunction;
    }
}
