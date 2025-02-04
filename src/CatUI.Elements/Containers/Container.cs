using CatUI.Data;

namespace CatUI.Elements.Containers
{
    public abstract class Container : Element
    {
        public Container(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
        }
    }
}
