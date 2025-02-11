using System;
using CatUI.PlatformExtension;

namespace CatUI.Data
{
    /// <summary>
    /// This represents the initializer for the platform on which the application runs. You should only use this if
    /// you want to make support for a new platform that is not yet covered by CatUI.
    /// </summary>
    /// <remarks>
    /// You should create a static property that returns a new instance of this class that sets all the necessary properties
    /// of the platform as the app developer should give that property as an argument to
    /// <see cref="CatApplication.AppBuilder.SetInitializer"/>.
    /// </remarks>
    public class CatApplicationInitializer
    {
        public Action? PostInitializationAction { get; }

        public DispatcherBase Dispatcher { get; }

        /// <summary>
        /// Creates a new <see cref="CatApplicationInitializer"/>.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use on this platform.</param>
        /// <param name="postInitializationAction">
        /// The action to perform after the initialization is done inside <see cref="CatApplication"/>, but before any
        /// UI code.
        /// </param>
        public CatApplicationInitializer(DispatcherBase dispatcher, Action? postInitializationAction = null)
        {
            Dispatcher = dispatcher;
            PostInitializationAction = postInitializationAction;
        }
    }
}
