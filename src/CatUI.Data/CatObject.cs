namespace CatUI.Data
{
    /// <summary>
    /// The base class for (almost) every class that CatUI uses. Provides basic functionality to all elements and data classes.
    /// </summary>
    public abstract class CatObject
    {
        /// <summary>
        /// Deep clones the object. When overriding this, ensure you always deep clone the object.
        /// For elements, this means that the clone won't belong to the document tree and won't have the original's event handlers.
        /// </summary>
        /// <returns>A new instance of the object, completely separated by the current object.</returns>
        public abstract CatObject Duplicate();
    }
}
