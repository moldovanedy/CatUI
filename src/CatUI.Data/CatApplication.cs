using System;
using System.Collections.Generic;
using System.Diagnostics;
using CatUI.Platform.Essentials;

namespace CatUI.Data
{
    /// <summary>
    /// Represents the global application object, responsible for initializations and other features
    /// like logging behavior and application name.
    /// </summary>
    public class CatApplication
    {
        /// <summary>
        /// The global application object. Accessing this before creating the object with <see cref="NewBuilder"/>
        /// will create a global instance with all the default parameters set, meaning it is unmodifiable after this point.
        /// You should always create a new instance with <see cref="NewBuilder"/>, then calling
        /// <see cref="AppBuilder.Build"/> before any CatUI-specific calls, as that will initialize this object properly.
        /// </summary>
        public static CatApplication Instance
        {
            get
            {
                _instance ??= new CatApplication();
                return _instance;
            }
        }

        private static CatApplication? _instance;

        /// <summary>
        /// Represents the version of CatUI used by your application.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static Version? CatUIVersion { get; private set; }

        /// <summary>
        /// The application name. It is not used yet.
        /// </summary>
        public string AppName { get; private set; } = "";

        /// <summary>
        /// The minimum logging level when DEBUG is defined. Defaults to <see cref="Debug"/>.
        /// </summary>
        public CatLogger.LogLevel DebugLogLevel { get; set; } = CatLogger.LogLevel.Debug;

        /// <summary>
        /// The minimum logging level when TRACE is defined and DEBUG is not. Defaults to
        /// <see cref="CatLogger.LogLevel.None"/>, so no logging in release by default.
        /// </summary>
        public CatLogger.LogLevel ReleaseLogLevel { get; set; } = CatLogger.LogLevel.None;

        /// <summary>
        /// The platform dispatcher. See <see cref="DispatcherBase"/> for more info.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Thrown if the dispatcher wasn't available because you didn't set the <see cref="AppInitializer"/> in the
        /// builder (using <see cref="AppBuilder.SetInitializer"/>).
        /// </exception>
        public DispatcherBase Dispatcher =>
            AppInitializer?.Dispatcher ?? throw new NotImplementedException(
                "Dispatcher is not available. Did you forgot to set the initializer?");

        public CatApplicationInitializer? AppInitializer { get; private set; }

        private CatApplication()
        {
#if DEBUG
            if (DebugLogLevel > CatLogger.LogLevel.Debug)
            {
                return;
            }

            Debug.WriteLine(
                "Initializing CatApplication. This message will only appear in debug mode if DebugLogLevel" +
                " is LogLevel.Debug or lower. To configure debugging, use SetMinimumDebugLogLevel and " +
                "SetMinimumReleaseLogLevel.");
#endif

            CatUIVersion = typeof(CatApplication).Assembly.GetName().Version;

            AppInitializer?.PostInitializationAction?.Invoke();
        }

        /// <summary>
        /// Creates a new builder that can set up the <see cref="CatApplication"/>'s parameters, like logging level
        /// or the application name.
        /// </summary>
        /// <remarks>
        /// This should be called before any CatUI-specific methods, then calling <see cref="AppBuilder.Build"/>.
        /// </remarks>
        /// <returns>A new AppBuilder on which you can call the specific methods to set the parameters.</returns>
        public static AppBuilder NewBuilder()
        {
            return new AppBuilder();
        }

        /// <summary>
        /// This is responsible for setting up the global <see cref="CatApplication"/> object.
        /// </summary>
        public class AppBuilder
        {
            private string _appName = "";
            private CatLogger.LogLevel _debugLogLevel = CatLogger.LogLevel.Debug;
            private CatLogger.LogLevel _releaseLogLevel = CatLogger.LogLevel.Warning;
            private CatApplicationInitializer? _initializer;

            /// <summary>
            /// Sets the application name.
            /// </summary>
            /// <param name="appName"></param>
            /// <returns>This builder.</returns>
            public AppBuilder SetAppName(string appName)
            {
                _appName = appName;
                return this;
            }

            /// <summary>
            /// Sets <see cref="CatApplication.DebugLogLevel"/>. This is only used when DEBUG is set.
            /// </summary>
            /// <param name="debugLogLevel"></param>
            /// <returns>This builder.</returns>
            public AppBuilder SetMinimumDebugLogLevel(CatLogger.LogLevel debugLogLevel)
            {
                _debugLogLevel = debugLogLevel;
                return this;
            }

            /// <summary>
            /// Sets <see cref="CatApplication.ReleaseLogLevel"/>. This is only used when TRACE is set and DEBUG is not.
            /// </summary>
            /// <param name="releaseLogLevel"></param>
            /// <returns>This builder.</returns>
            public AppBuilder SetMinimumReleaseLogLevel(CatLogger.LogLevel releaseLogLevel)
            {
                _releaseLogLevel = releaseLogLevel;
                return this;
            }

            /// <summary>
            /// Sets the platform-specific app initializer (this should be already given as a property for each
            /// platform, like IPlatformInfo.PlatformInitializer). 
            /// </summary>
            /// <param name="initializer">
            /// The app initializer. Without it, you will not be able to use critical functionality like
            /// <see cref="CatApplication.Dispatcher"/>.
            /// </param>
            /// <returns>This builder.</returns>
            public AppBuilder SetInitializer(CatApplicationInitializer initializer)
            {
                _initializer = initializer;
                return this;
            }

            /// <summary>
            /// Simply sets <see cref="Trace.Listeners"/>, this is only a convenience function because its functionality
            /// can be easily achieved by directly manipulating <see cref="Trace.Listeners"/>.
            /// </summary>
            /// <remarks>It clears existing Trace listeners.</remarks>
            /// <param name="listeners">The listeners that will respond to any log by performing a certain action.</param>
            /// <returns>This builder.</returns>
            public AppBuilder SetReleaseLoggingListeners(List<TraceListener> listeners)
            {
                Trace.Listeners.Clear();
                foreach (TraceListener listener in listeners)
                {
                    Trace.Listeners.Add(listener);
                }

                return this;
            }

            /// <summary>
            /// Sets up the <see cref="CatApplication"/> object using the given parameters or their default value.
            /// </summary>
            /// <returns><see cref="CatApplication.Instance"/>.</returns>
            /// <exception cref="InvalidOperationException">
            /// Thrown if the <see cref="CatApplication"/> global object has already been set up.
            /// </exception>
            public CatApplication Build()
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException("A CatApplication has already been instantiated.");
                }

                //using Instance here is OK, as this has private access to CatApplication
                Instance.AppName = _appName;
                Instance.DebugLogLevel = _debugLogLevel;
                Instance.ReleaseLogLevel = _releaseLogLevel;
                Instance.AppInitializer = _initializer;
                return Instance;
            }
        }
    }
}
