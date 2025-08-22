using CatUI.Data;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Linear
{
    public class RowContainer : LinearContainerBase
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<RowContainer>? Ref
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

        private ObjectRef<RowContainer>? _ref;

        /// <summary>
        /// Indicates the vertical alignment of the children. A child can override this by having a
        /// <see cref="RowContainerSizing"/> set as <see cref="Element.ElementContainerSizing"/> and setting
        /// <see cref="RowContainerSizing.VerticalAlignment"/> to a different value. The default value is
        /// <see cref="VerticalAlignmentType.Top"/>.
        /// </summary>
        public VerticalAlignmentType VerticalAlignment
        {
            get => (VerticalAlignmentType)PreferredAlignment;
            set => VerticalAlignmentProperty.Value = value;
        }

        public ObservableProperty<VerticalAlignmentType> VerticalAlignmentProperty { get; }
            = new(VerticalAlignmentType.Top);

        private void SetVerticalAlignment(VerticalAlignmentType value)
        {
            PreferredAlignment = (AlignmentType)value;
            SetLocalValue(nameof(VerticalAlignment), value);
            MarkLayoutDirty();
        }

        public sealed override Orientation ContainerOrientation => Orientation.Horizontal;

        public RowContainer()
        {
            VerticalAlignmentProperty.ValueChangedEvent += SetVerticalAlignment;
        }

        public RowContainer(RowContainer other) : base(other)
        {
            VerticalAlignmentProperty.ValueChangedEvent += SetVerticalAlignment;
            VerticalAlignment = other.VerticalAlignment;
        }

        public override RowContainer Duplicate()
        {
            var el = new RowContainer(this);
            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
