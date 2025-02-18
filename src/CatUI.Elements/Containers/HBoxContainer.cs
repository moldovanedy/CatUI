using CatUI.Data.Containers;
using CatUI.Utils;

namespace CatUI.Elements.Containers
{
    public class HBoxContainer : BoxContainer
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<HBoxContainer>? Ref
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

        private ObjectRef<HBoxContainer>? _ref;

        public override Orientation BoxOrientation => Orientation.Horizontal;

        public override HBoxContainer Duplicate()
        {
            return new HBoxContainer
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
