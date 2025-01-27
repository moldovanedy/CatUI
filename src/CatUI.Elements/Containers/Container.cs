using CatUI.Elements.Themes;

namespace CatUI.Elements.Containers
{
    public abstract class Container : Element
    {
        public Container(ThemeDefinition<ElementThemeData>? themeOverrides = null)
            : base(themeOverrides)
        {
        }
    }
}
