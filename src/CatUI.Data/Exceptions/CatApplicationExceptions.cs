using System;

namespace CatUI.Data.Exceptions
{
    /// <summary>
    /// Occurs when you try to use any kind of CatUI specific API before initializing <see cref="CatApplication"/>
    /// properly.
    /// </summary>
    public class CatApplicationUninitializedException : Exception
    {
        public CatApplicationUninitializedException(string message) : base(message) { }

        public CatApplicationUninitializedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
