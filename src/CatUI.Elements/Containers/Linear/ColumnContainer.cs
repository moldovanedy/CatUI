using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Linear
{
    public class ColumnContainer : LinearContainer
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
            set
            {
                SetHorizontalAlignment(value);
                HorizontalAlignmentProperty.Value = value;
            }
        }

        public ObservableProperty<HorizontalAlignmentType> HorizontalAlignmentProperty { get; private set; }
            = new(HorizontalAlignmentType.Left);

        private void SetHorizontalAlignment(HorizontalAlignmentType value)
        {
            PreferredAlignment = (AlignmentType)value;
            MarkLayoutDirty();
        }

        public sealed override Orientation ContainerOrientation => Orientation.Vertical;

        public ColumnContainer()
        {
            HorizontalAlignmentProperty.ValueChangedEvent += SetHorizontalAlignment;
        }

        ~ColumnContainer()
        {
            HorizontalAlignmentProperty = null!;
        }

        public override ColumnContainer Duplicate()
        {
            return new ColumnContainer
            {
                Arrangement = Arrangement,
                HorizontalAlignment = HorizontalAlignment,
                //
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };
        }
    }
}
