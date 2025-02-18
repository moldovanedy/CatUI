using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Containers
{
    public class VBoxContainer : BoxContainer
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<VBoxContainer>? Ref
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

        private ObjectRef<VBoxContainer>? _ref;

        public override Orientation BoxOrientation => Orientation.Vertical;

        public override VBoxContainer Duplicate()
        {
            return new VBoxContainer
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
