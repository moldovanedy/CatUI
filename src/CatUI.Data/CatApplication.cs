using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using CatUI.Data.Enums;
using CatUI.Data.Exceptions;
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
        /// will throw a <see cref="CatApplicationUninitializedException"/>. You should always create a new instance
        /// with <see cref="NewBuilder"/>, then calling <see cref="AppBuilder.Build"/> before any CatUI-specific calls,
        /// as that will initialize this object properly.
        /// </summary>
        public static CatApplication Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new CatApplicationUninitializedException(
                        "CatApplication is uninitialized. Did you forget to initialize it through CatApplicationBuilder?");
                }

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
        /// If true, will log all messages in the application's stdout buffer. See
        /// <see cref="AppBuilder.SetUseReleaseStdoutLogging"/> for more info. The default value is true.
        /// </summary>
        public bool UseReleaseStdoutLogging { get; private set; } = true;

        /// <summary>
        /// Specifies the order in which graphics APIs are tried at window creation. If an API is not available, the
        /// next one will be tried, so always the first API found will be used. This is currently only relevant on
        /// desktop platforms. Setting this after a window was created has no effect, but it will have effect on the
        /// next windows that you will open.
        /// </summary>
        /// <remarks>
        /// <para>
        /// While it's generally wise to include several APIs, don't include all of them, since some of them are
        /// platform-specific (e.g. Metal is only for Apple devices, while Vulkan is available on most non-Apple
        /// (and modern enough) platforms). Also, some of them are known to offer lower performance compared to others,
        /// like ANGLE, for example.
        /// </para>
        /// <para>
        /// <see cref="GraphicsApi.OpenGlCore"/> and <see cref="GraphicsApi.OpenGlCompatibility"/> is the same here, as
        /// the OpenGL driver always selects the highest available version.
        /// </para>
        /// </remarks>
        public ImmutableArray<GraphicsApi> GraphicsApisTryingOrder { get; set; } =
            [GraphicsApi.OpenGlCore, GraphicsApi.Software];

        /// <summary>
        /// The platform dispatcher. See <see cref="DispatcherBase"/> for more info.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Thrown if the dispatcher wasn't available because you didn't set the <see cref="PlatformInformation"/> in the
        /// builder (using <see cref="AppBuilder.SetPlatformInfo"/>).
        /// </exception>
        public DispatcherBase Dispatcher =>
            PlatformInformation?.AppInitializer.Dispatcher ?? throw new NotImplementedException(
                "Dispatcher is not available. Did you forgot to set the initializer?");

        public PlatformUiOptionsBase PlatformUiOptions =>
            PlatformInformation?.AppInitializer.PlatformUiOptions ?? throw new NotImplementedException(
                "Platform UI options are not available. Did you forgot to set the initializer?");

        public PlatformInfo? PlatformInformation { get; private set; }

        private CatApplication()
        {
#if DEBUG
            if (DebugLogLevel <= CatLogger.LogLevel.Debug)
            {
                Console.WriteLine(
                    "Initializing CatApplication. This message will only appear in debug mode if DebugLogLevel" +
                    " is LogLevel.Debug or lower. To configure debugging, use SetMinimumDebugLogLevel and " +
                    "SetMinimumReleaseLogLevel.");
            }
#endif

            CatUIVersion = typeof(CatApplication).Assembly.GetName().Version;
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
            private bool _useReleaseStdoutLogging = true;
            private PlatformInfo? _platformInfo;

            private ImmutableArray<GraphicsApi> _graphicsApisTryingOrder =
                [GraphicsApi.OpenGlCore, GraphicsApi.Software];

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
            /// Sets the platform-specific app info object (this should be already given as a property for each
            /// platform, like a class that extends <see cref="PlatformInfo"/>). 
            /// </summary>
            /// <remarks>If you don't set this, expect random crashes and weird behaviour all across CatUI.</remarks>
            /// <param name="platformInfo">
            /// The app info object. Without it, you will not be able to use critical functionality like
            /// <see cref="CatApplication.Dispatcher"/> or other core functionality.
            /// </param>
            /// <returns>This builder.</returns>
            public AppBuilder SetPlatformInfo(PlatformInfo platformInfo)
            {
                _platformInfo = platformInfo;
                return this;
            }

            /// <summary>
            /// Simply sets <see cref="Trace.Listeners"/>, this is only a convenience function because its functionality
            /// can be easily achieved by directly manipulating <see cref="Trace.Listeners"/>.
            /// </summary>
            /// <remarks>
            /// It clears existing Trace listeners. Don't put <see cref="Console.Out"/> here, as that's already
            /// controlled by <see cref="SetUseReleaseStdoutLogging"/>.
            /// </remarks>
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
            /// Adds the <see cref="Console.Out"/> to <see cref="Trace.Listeners"/>, so that the logs will appear in the
            /// stdout stream of the application even in release mode. It is highly recommended to set this to true
            /// (the default value is true), as most UI-based apps still do this for easier debugging.
            /// </summary>
            /// <param name="enabled">True if this feature should be used, false otherwise.</param>
            /// <returns>This builder.</returns>
            public AppBuilder SetUseReleaseStdoutLogging(bool enabled)
            {
                _useReleaseStdoutLogging = enabled;
                return this;
            }

            /// <summary>
            /// Sets <see cref="CatApplication.GraphicsApisTryingOrder"/>, refer to its documentation for more info.
            /// </summary>
            /// <param name="graphicsApis">The graphics APIs trying order.</param>
            /// <returns>This builder.</returns>
            public AppBuilder SetGraphicsApisTryingOrder(ImmutableArray<GraphicsApi> graphicsApis)
            {
                _graphicsApisTryingOrder = graphicsApis;
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


                _instance = new CatApplication();
                Instance.AppName = _appName;
                Instance.DebugLogLevel = _debugLogLevel;
                Instance.ReleaseLogLevel = _releaseLogLevel;
                Instance.PlatformInformation = _platformInfo;
                Instance.GraphicsApisTryingOrder = _graphicsApisTryingOrder;

                Instance.UseReleaseStdoutLogging = _useReleaseStdoutLogging;
                if (_useReleaseStdoutLogging)
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                }

                Instance.PlatformInformation?.AppInitializer.Initialize();
                return Instance;
            }
        }
    }
}
