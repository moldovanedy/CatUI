using CatUI.Data;

namespace CatUI.Elements.Containers
{
    public abstract class BoxContainer : Container
    {
        /// <summary>
        /// Specifies the dimension of the space left between each of the elements in the container. By default, it's 0.
        /// </summary>
        public Dimension Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                SpacingProperty.Value = _spacing;
            }
        }

        private Dimension _spacing = new(0);
        public ObservableProperty<Dimension> SpacingProperty { get; } = new(new Dimension(0));

        /// <summary>
        /// Specifies the orientation of this BoxContainer. Can be vertical or horizontal.
        /// </summary>
        public abstract Orientation BoxOrientation { get; }

        public BoxContainer(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
            SpacingProperty.ValueChangedEvent += SetSpacing;
        }

        ~BoxContainer()
        {
            SpacingProperty.ValueChangedEvent -= SetSpacing;
        }

        private void SetSpacing(Dimension value)
        {
            _spacing = value;
        }

        public enum Orientation
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}
