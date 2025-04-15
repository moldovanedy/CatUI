using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Utils
{
    /// <summary>
    /// A utility element that is generally used in containers to add space between elements. This is just a regular
    /// element that modifies the <see cref="Element.Layout"/>, so is you modify <see cref="Element.Layout"/> after
    /// creation, this spacer will no longer respect the given dimension and orientation.
    /// </summary>
    public class Spacer : Element
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<Spacer>? Ref
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

        private ObjectRef<Spacer>? _ref;

        private Spacer() { }

        public Spacer(Dimension space, Orientation orientation)
        {
            Layout =
                orientation == Orientation.Horizontal
                    ? new ElementLayout().SetFixedWidth(space)
                    : new ElementLayout().SetFixedHeight(space);
        }

        public override Spacer Duplicate()
        {
            return new Spacer
            {
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
