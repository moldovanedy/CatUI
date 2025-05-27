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

    /// <summary>
    /// Represents an error that occurred on an internal platform level, which is generally outside app control
    /// (e.g. hardware graphics context failed to initialize, failed to create a window).
    /// </summary>
    public class InternalPlatformException : Exception
    {
        public InternalPlatformException(string message) : base(message) { }
        public InternalPlatformException(string message, Exception innerException) : base(message, innerException) { }
    }
}
