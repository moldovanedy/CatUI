using CatUI.Data;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Linear;

public class ColumnContainer : LinearContainerBase
{
    /// <inheritdoc cref="Element.Ref"/>
    public new ObjectRef<ColumnContainer>? Ref
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

    private ObjectRef<ColumnContainer>? _ref;

    /// <summary>
    /// Indicates the horizontal alignment of the children. A child can override this by having a
    /// <see cref="ColumnContainerSizing"/> set as <see cref="Element.ElementContainerSizing"/> and setting
    /// <see cref="ColumnContainerSizing.HorizontalAlignment"/> to a different value. The default value is
    /// <see cref="HorizontalAlignmentType.Left"/>.
    /// </summary>
    public HorizontalAlignmentType HorizontalAlignment
    {
        get => (HorizontalAlignmentType)PreferredAlignment;
        set => HorizontalAlignmentProperty.Value = value;
    }

    public ObservableProperty<HorizontalAlignmentType> HorizontalAlignmentProperty
    {
        get => _horizontalAlignmentProperty;
        set => _horizontalAlignmentProperty.BindBidirectional(value);
    }

    private readonly ObservableProperty<HorizontalAlignmentType> _horizontalAlignmentProperty =
        new(HorizontalAlignmentType.Left);

    private void SetHorizontalAlignment(HorizontalAlignmentType value)
    {
        PreferredAlignment = (AlignmentType)value;
        SetLocalValue(nameof(HorizontalAlignment), value);
        MarkLayoutDirty();
    }

    public sealed override Orientation ContainerOrientation => Orientation.Vertical;

    public ColumnContainer()
    {
        HorizontalAlignmentProperty.ValueChangedEvent += SetHorizontalAlignment;
    }

    public ColumnContainer(ColumnContainer other) : base(other)
    {
        HorizontalAlignmentProperty.ValueChangedEvent += SetHorizontalAlignment;
        HorizontalAlignment = other.HorizontalAlignment;
    }

    public override ColumnContainer Duplicate()
    {
        var el = new ColumnContainer(this);
        DuplicateChildrenUtil(el);
        return el;
    }
}
