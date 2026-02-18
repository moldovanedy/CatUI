using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Elements.Containers.Linear;
using CatUI.Utils;

namespace CatUI.Elements.Utils;

/// <inheritdoc cref="Divider"/>
/// <remarks>
/// This will display a horizontal line, so it's generally used inside layouts that work like a column
/// (like <see cref="ColumnContainer"/>).
/// </remarks>
public class HorizontalDivider : Divider
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<HorizontalDivider>? Ref
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

    private ObjectRef<HorizontalDivider>? _ref;

    /// <summary>
    /// Represents the spacing between the line and the element on the top. The value is in <see cref="Unit.Dp"/>.
    /// The default value is 0.
    /// </summary>
    public float TopSpacing
    {
        get => Spacing.Item1;
        set
        {
            SetTopSpacing(value);
            TopSpacingProperty.Value = value;
        }
    }

    public ObservableProperty<float> TopSpacingProperty
    {
        get => _topSpacingProperty;
        set => _topSpacingProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<float> _topSpacingProperty = new(0);

    private void SetTopSpacing(float value)
    {
        //will mark the layout dirty automatically
        Spacing = (value, Spacing.Item2);
    }

    /// <summary>
    /// Represents the spacing between the line and the element on the bottom. The value is in <see cref="Unit.Dp"/>.
    /// The default value is 0.
    /// </summary>
    public float BottomSpacing
    {
        get => Spacing.Item2;
        set
        {
            SetBottomSpacing(value);
            BottomSpacingProperty.Value = value;
        }
    }

    public ObservableProperty<float> BottomSpacingProperty
    {
        get => _bottomSpacingProperty;
        set => _bottomSpacingProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<float> _bottomSpacingProperty = new(0);

    private void SetBottomSpacing(float value)
    {
        //will mark the layout dirty automatically
        Spacing = (Spacing.Item1, value);
    }

    /// <summary>
    /// Represents the padding (or space) between the left end of the line (line cap) and the element bounds.
    /// The percentage will represent the width of the element. The default value is 0.
    /// </summary>
    public Dimension LeftLinePadding
    {
        get => LinePadding.Item1;
        set
        {
            SetLeftLinePadding(value);
            LeftLinePaddingProperty.Value = value;
        }
    }

    public ObservableProperty<Dimension> LeftLinePaddingProperty
    {
        get => _leftLinePaddingProperty;
        set => _leftLinePaddingProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<Dimension> _leftLinePaddingProperty = new(0);

    private void SetLeftLinePadding(Dimension value)
    {
        //will mark the document dirty automatically
        LinePadding = (value, LinePadding.Item2);
    }

    /// <summary>
    /// Represents the padding (or space) between the right end of the line (line cap) and the element bounds.
    /// The percentage will represent the width of the element. The default value is 0.
    /// </summary>
    public Dimension RightLinePadding
    {
        get => LinePadding.Item1;
        set
        {
            SetRightLinePadding(value);
            RightLinePaddingProperty.Value = value;
        }
    }

    public ObservableProperty<Dimension> RightLinePaddingProperty
    {
        get => _rightLinePaddingProperty;
        set => _rightLinePaddingProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<Dimension> _rightLinePaddingProperty = new(0);

    private void SetRightLinePadding(Dimension value)
    {
        //will mark the document dirty automatically
        LinePadding = (LinePadding.Item1, value);
    }

    public HorizontalDivider(float thickness = 2, IBrush? brush = null) : base(Orientation.Horizontal)
    {
        Init(thickness, brush);
    }

    public HorizontalDivider(HorizontalDivider other) : base(other)
    {
        Init(other.LineThickness, other.LineBrush);

        TopSpacing = other.TopSpacing;
        BottomSpacing = other.BottomSpacing;
        LeftLinePadding = other.LeftLinePadding;
        RightLinePadding = other.RightLinePadding;
    }

    private void Init(float thickness = 2, IBrush? brush = null)
    {
        TopSpacingProperty.ValueChangedEvent += SetTopSpacing;
        BottomSpacingProperty.ValueChangedEvent += SetBottomSpacing;
        LeftLinePaddingProperty.ValueChangedEvent += SetLeftLinePadding;
        RightLinePaddingProperty.ValueChangedEvent += SetRightLinePadding;

        LineThickness = thickness;
        if (brush != null)
        {
            LineBrush = brush;
        }
    }

    public override HorizontalDivider Duplicate()
    {
        var el = new HorizontalDivider(this);
        DuplicateChildrenUtil(el);
        return el;
    }
}
