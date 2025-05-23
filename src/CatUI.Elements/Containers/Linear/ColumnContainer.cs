using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Linear
{
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

        public ObservableProperty<HorizontalAlignmentType> HorizontalAlignmentProperty { get; }
            = new(HorizontalAlignmentType.Left);

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

        //~ColumnContainer()
        //{
        //    HorizontalAlignmentProperty = null!;
        //}

        public override ColumnContainer Duplicate()
        {
            ColumnContainer el = new()
            {
                Arrangement = Arrangement,
                HorizontalAlignment = HorizontalAlignment,
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
