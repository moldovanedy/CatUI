using System;
using CatUI.Data.Theming;
using CatUI.Platform.Essentials;

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
        private bool _initializationGuard;

        internal Action? PostInitializationAction { get; }

        internal DispatcherBase Dispatcher { get; }

        internal PlatformUiOptionsBase PlatformUiOptions { get; }

        /// <summary>
        /// Creates a new <see cref="CatApplicationInitializer"/>.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use on this platform.</param>
        /// <param name="platformUiOptions">The platform-controlled UI options implementation for this platform.</param>
        /// <param name="postInitializationAction">
        /// The action to perform after the initialization is done inside <see cref="CatApplication"/>, but before any
        /// UI code.
        /// </param>
        public CatApplicationInitializer(
            DispatcherBase dispatcher,
            PlatformUiOptionsBase platformUiOptions,
            Action? postInitializationAction = null)
        {
            Dispatcher = dispatcher;
            PlatformUiOptions = platformUiOptions;
            PostInitializationAction = postInitializationAction;
        }

        /// <summary>
        /// Global initialization.
        /// </summary>
        internal void Initialize()
        {
            if (_initializationGuard)
            {
                return;
            }

            _initializationGuard = true;
            PostInitializationAction?.Invoke();
            CatTheme.Initialize();
        }
    }
}
