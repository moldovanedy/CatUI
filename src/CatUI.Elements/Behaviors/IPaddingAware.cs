using CatUI.Data;
using CatUI.Data.ElementData;

namespace CatUI.Elements.Behaviors
{
    /// <summary>
    /// An element that implements this interface has a configurable content padding by default. Only use this on
    /// elements that will usually have padding (i.e. where not having a padding is uncommon) like buttons. 
    /// </summary>
    public interface IPaddingAware
    {
        /// <summary>
        /// Represents the padding of the element's content from the element's bounds, so basically the content padding.
        /// </summary>
        public EdgeInset Padding { get; set; }

        public ObservableProperty<EdgeInset> PaddingProperty { get; }
    }
}
