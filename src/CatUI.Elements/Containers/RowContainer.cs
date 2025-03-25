using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Containers
{
    public class RowContainer : LinearContainer
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

        public override Orientation ContainerOrientation => Orientation.Horizontal;

        public override RowContainer Duplicate()
        {
            return new RowContainer
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
