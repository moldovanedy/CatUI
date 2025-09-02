using CatUI.Platform.CommonInterface;
#if CAT_LOCAL_WINDOWS
using CatUI.Platform.Windows.OS;
#elif CAT_LOCAL_LINUX
using CatUI.Platform.Linux.OS;
#endif

namespace CatUI.Platform
{
    /// <summary>
    /// Provides access to some common functions on the runtime platform (e.g. opening the file picker, showing alerts,
    /// etc.) The objects are always non-null if the platform supports the said feature (e.g. <see cref="WindowIcon"/>
    /// will be available on Windows and Linux, but null on macOS).
    /// </summary>
    /// <remarks>
    /// Some features are implemented at a higher level (for example, file pickers and window icons
    /// (IApplicationWindow.GetWindowIcon)), details are revealed on each feature separately.
    /// </remarks>
    public static class OS
    {
        /// <summary>
        /// Provides access to the window icon. IApplicationWindow.GetWindowIcon provides more sizes and also caches the
        /// result, so it's the preferred method to get the window icon instead of this.
        /// </summary>
        /// <remarks>
        /// The <see cref="IWindowIcon.GetWindowIcon"/> does NOT cache the icon, so the operation might be expensive.
        /// You are responsible for caching the icon.
        /// </remarks>
        public static IWindowIcon? WindowIcon { get; private set; }

        /// <summary>
        /// Shows a native alert dialog (a.k.a. message box). It presents the user a familiar interface for common
        /// actions, but the style is dependent on the runtime platform. Most modern apps choose to use custom
        /// dialogs instead of this native interface, but in the end, the decision is yours.
        /// </summary>
        /// <remarks>
        /// This feature is not supported on WebAssembly and Linux Flatpak (sandboxed), and it also might have slight
        /// issues on Linux (non-sandboxed) and iOS. Read the remarks on each implementation to find out more.
        /// </remarks>
        public static INativeAlert? NativeAlert { get; private set; }

        /// <summary>
        /// Shows a native file picker dialog. The user can pick any file from their device, you can allow
        /// multiple files, directory, and even additional options on the dialog on supporting platforms. This is very
        /// convenient, as the UI is familiar for the users, and you generally don't need storage permissions on
        /// sandboxed platforms.
        /// </summary>
        public static IFilePicker? FilePicker { get; private set; }

        private static bool _isInitialized;

        /// <summary>
        /// This initializes the platform-specific APIs. Calling this more than once does not have any effect unless
        /// this method threw an exception at the first call, which is highly unlikely.
        /// </summary>
        public static void Init()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

#if CAT_LOCAL_WINDOWS
            WindowIcon = new WindowIconWindows();
#elif CAT_LOCAL_LINUX
            WindowIcon = new WindowIconLinux();
#endif


#if CAT_LOCAL_WINDOWS
            NativeAlert = new NativeAlertWindows();
#elif CAT_LOCAL_LINUX
            NativeAlert = new NativeAlertLinux();
#endif


#if CAT_LOCAL_WINDOWS
            FilePicker = new FilePickerWindows();
#elif CAT_LOCAL_LINUX
            FilePicker = new FilePickerLinux();
#endif
        }
    }
}
