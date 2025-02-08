using CatUI.Data;
using CatUI.Elements.Behaviors;

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

        /// <summary>
        /// Recalculates the children's bounds. It shouldn't be called from outside code, as it's internally called when
        /// needed. If you implement this, you have to manually take care of all the relevant children, especially
        /// those that implement <see cref="IExpandable"/>, as they can expand depending on the content.
        /// </summary>
        public abstract void RecalculateContainerChildren();
    }
}
