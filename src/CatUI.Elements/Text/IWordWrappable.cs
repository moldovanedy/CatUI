using CatUI.Data;

namespace CatUI.Elements.Text
{
    /// <summary>
    /// Elements that implement this interface are able to wrap lines of text that are too long to fit to the next rows.
    /// </summary>
    public interface IWordWrappable
    {
        /// <summary>
        /// If true, it makes the text wrap itself if the width is too small to fit an entire row. Beware that word wrapping
        /// involves quite a lot of calculations and elements without word wrapping are generally faster. Use only when
        /// needed. See the remarks on each element that implements this interface for more information.
        /// The default value is false.
        /// </summary>
        public bool WordWrap { get; set; }

        public ObservableProperty<bool> WordWrapProperty { get; }
    }
}
