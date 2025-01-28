using CatUI.Data;
using CatUI.Elements.Themes;

namespace CatUI.Elements.Containers
{
    public abstract class Container : Element
    {
        public Container(
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                themeOverrides,
                preferredWidth,
                preferredHeight)
        {
        }
    }
}
