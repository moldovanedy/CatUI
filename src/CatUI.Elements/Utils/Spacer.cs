using CatUI.Data;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
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

        public Spacer(Dimension space, Orientation orientation)
        {
            Layout =
                orientation == Orientation.Horizontal
                    ? new ElementLayout().SetFixedWidth(space)
                    : new ElementLayout().SetFixedHeight(space);
        }

        public Spacer(Spacer other) : base(other)
        {
        }

        public override Spacer Duplicate()
        {
            var el = new Spacer(this);
            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
