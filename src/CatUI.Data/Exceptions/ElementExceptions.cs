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

    /// <summary>
    /// Thrown when you give the same ID to more than one element.
    /// </summary>
    public class DuplicateIdException : Exception
    {
        public DuplicateIdException(string message) : base(message) { }
        public DuplicateIdException(string message, Exception innerException) : base(message, innerException) { }
    }
}
