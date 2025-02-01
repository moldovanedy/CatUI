using System;

namespace CatUI.Data.Exceptions
{
    /// <summary>
    /// Thrown when you add a child that already exists to the children of an element.
    /// </summary>
    public class DuplicateElementException : Exception
    {
        public DuplicateElementException(string message) : base(message) { }
        public DuplicateElementException(string message, Exception innerException) : base(message, innerException) { }
    }
}
