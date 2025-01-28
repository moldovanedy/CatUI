using CatUI.Data;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Containers
{
    public abstract class BoxContainer : Container
    {
        /// <summary>
        /// Specifies the dimension of the space left between each of the elements in the container. By default, it's 0.
        /// </summary>
        public Dimension Spacing { get; set; } = Dimension.Unset;

        /// <summary>
        /// Specifies the orientation of this BoxContainer. Can be vertical or horizontal.
        /// </summary>
        public abstract Orientation BoxOrientation { get; }

        public BoxContainer(
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                themeOverrides,
                preferredWidth,
                preferredHeight)
        {
        }

        public enum Orientation
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}
