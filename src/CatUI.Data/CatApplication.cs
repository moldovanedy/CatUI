using System;

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
        /// The application name. It is not used yet.
        /// </summary>
        public string AppName { get; private set; } = "";
        /// <summary>
        /// The minimum logging level when DEBUG is defined. Defaults to <see cref="LogLevel.Debug"/>.
        /// </summary>
        public LogLevel DebugLogLevel { get; set; } = LogLevel.Debug;
        /// <summary>
        /// The minimum logging level when TRACE is defined and DEBUG is not. Defaults to <see cref="LogLevel.None"/>,
        /// so no logging in release by default.
        /// </summary>
        public LogLevel ReleaseLogLevel { get; set; } = LogLevel.None;

        private CatApplication()
        {
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
        /// The type of log message specific to the application. Logs are written by internal CatUI components.
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// No logging, only used when configuring the logging level.
            /// </summary>
            None = 0,
            /// <summary>
            /// Contains lots of details that are generally only used for debugging;
            /// very rarely enabled in a production app.
            /// </summary>
            Verbose = 1,
            /// <summary>
            /// Used when debugging or for internal events.
            /// </summary>
            Debug = 2,
            /// <summary>
            /// Generally used for suggestions when debugging. Generally disabled in production.
            /// </summary>
            Info = 3,
            /// <summary>
            /// Something is not configured properly. Generally enabled in production.
            /// </summary>
            Warning = 4,
            /// <summary>
            /// Something went wrong and these messages might help you to diagnose the problem.
            /// It is strongly encouraged to enable this level (or a lower one) in production.
            /// </summary>
            Error = 5,
            /// <summary>
            /// Something happened that makes the application unusable and cannot continue normal operation,
            /// generally something wrong on the client machine. Use this to present an error message, gather data and
            /// quit the application. This is very rarely (if at all) triggered.
            /// It is strongly encouraged to enable this level (or a lower one) in production.
            /// </summary>
            Critical = 6,
        }
        
        /// <summary>
        /// This is responsible for setting up the global <see cref="CatApplication"/> object.
        /// </summary>
        public class AppBuilder
        {
            private string _appName = "";
            private LogLevel _debugLogLevel = LogLevel.Debug;
            private LogLevel _releaseLogLevel = LogLevel.Warning;

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
            public AppBuilder SetMinimumDebugLogLevel(LogLevel debugLogLevel)
            {
                _debugLogLevel = debugLogLevel;
                return this;
            }

            /// <summary>
            /// Sets <see cref="CatApplication.ReleaseLogLevel"/>. This is only used when TRACE is set and DEBUG is not.
            /// </summary>
            /// <param name="releaseLogLevel"></param>
            /// <returns>This builder.</returns>
            public AppBuilder SetMinimumReleaseLogLevel(LogLevel releaseLogLevel)
            {
                _releaseLogLevel = releaseLogLevel;
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
                if (CatApplication._instance != null)
                {
                    throw new InvalidOperationException("A CatApplication has already been instantiated.");
                }
                
                //using Instance here is OK, as this has private access to CatApplication
                CatApplication.Instance.AppName = _appName;
                CatApplication.Instance.DebugLogLevel = _debugLogLevel;
                CatApplication.Instance.ReleaseLogLevel = _releaseLogLevel;
                return CatApplication.Instance;
            }
        }
    }

}
