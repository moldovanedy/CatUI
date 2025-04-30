using System;

namespace CatUI.Windowing.Common
{
    /// <summary>
    /// Represents an error that occured on an internal platform level, which is generally outside of app control
    /// (e.g. hardware graphics context failed to initialize, failed to create a window).
    /// </summary>
    public class InternalPlatformException : Exception
    {
        public InternalPlatformException(string message) : base(message) { }
        public InternalPlatformException(string message, Exception innerException) : base(message, innerException) { }
    }
}
