using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
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
            set
            {
                if (value != (VerticalAlignmentType)PreferredAlignment)
                {
                    VerticalAlignmentProperty.Value = value;
                }
            }
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

        //~RowContainer()
        //{
        //    VerticalAlignmentProperty = null!;
        //}

        public override RowContainer Duplicate()
        {
            RowContainer el = new()
            {
                Arrangement = Arrangement,
                VerticalAlignment = VerticalAlignment,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
