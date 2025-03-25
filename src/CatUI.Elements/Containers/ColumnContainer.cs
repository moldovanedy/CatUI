using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Containers
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

        public override Orientation ContainerOrientation => Orientation.Vertical;

        public override ColumnContainer Duplicate()
        {
            return new ColumnContainer
            {
                Spacing = Spacing,
                //
                Position = Position,
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
