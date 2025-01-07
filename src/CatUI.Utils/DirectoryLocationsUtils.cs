using System;
using System.IO;

namespace CatUI.Utils
{
    /// <summary>
    /// Provides various cross-platform utilities for retrieving the locations of the most common directories in an operating system.
    /// Besides locations, it also provides functions to get the application's current permissions in those directories.
    /// It is a very helpful extension to <see cref="Environment.SpecialFolder"/> and <see cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>.
    /// </summary>
    /// <remarks>
    /// This is mostly useful for desktop environments, as access to the file system on mobiles is more complex
    /// and only a small subset of these functions will work on mobile devices.
    /// </remarks>
    public static class DirectoryLocationUtils
    {
        public static string GetKnownDirectoryPath(KnownDirectory directory)
        {
            Environment.SpecialFolder specialFolder;
            switch (directory)
            {
                case KnownDirectory.CurrentWorkingDirectory:
                    return Environment.CurrentDirectory;
                case KnownDirectory.LocalAppData:
                    specialFolder = Environment.SpecialFolder.LocalApplicationData;
                    break;
                case KnownDirectory.RoamingAppData:
                    specialFolder = Environment.SpecialFolder.ApplicationData;
                    break;
                case KnownDirectory.GlobalAppData:
                    specialFolder = Environment.SpecialFolder.CommonApplicationData;
                    break;
                case KnownDirectory.Documents:
                    specialFolder = Environment.SpecialFolder.MyDocuments;
                    break;
                case KnownDirectory.Pictures:
                    specialFolder = Environment.SpecialFolder.MyPictures;
                    break;
                case KnownDirectory.Music:
                    specialFolder = Environment.SpecialFolder.MyMusic;
                    break;
                case KnownDirectory.Videos:
                    specialFolder = Environment.SpecialFolder.MyVideos;
                    break;
                case KnownDirectory.Desktop:
                    specialFolder = Environment.SpecialFolder.Desktop;
                    break;
                case KnownDirectory.Fonts:
                    specialFolder = Environment.SpecialFolder.Fonts;
                    break;
                case KnownDirectory.UserProfile:
                    specialFolder = Environment.SpecialFolder.UserProfile;
                    break;

                case KnownDirectory.WinUserStartMenu:
                    specialFolder = Environment.SpecialFolder.StartMenu;
                    break;
                case KnownDirectory.WinUserStartupPrograms:
                    specialFolder = Environment.SpecialFolder.Startup;
                    break;
                case KnownDirectory.WinGlobalStartMenu:
                    specialFolder = Environment.SpecialFolder.CommonStartMenu;
                    break;
                case KnownDirectory.WinGlobalStartupPrograms:
                    specialFolder = Environment.SpecialFolder.CommonStartup;
                    break;
                default:
                    return "";
            }

            return Environment.GetFolderPath(specialFolder);
        }

        /// <summary>
        /// Returns the path to the directory defined by <see cref="KnownDirectory.LocalAppData"/>, but ensures that
        /// a directory named with your application name (see `APP_NAME`) is created. Does not handle directory creation exceptions.
        /// </summary>
        /// <remarks>
        /// The application name must not have spaces in the name, illegal characters (see what are the illegal characters
        /// in a directory name on each operating system, generally "*","\"",:/","\","&lt;",">",":","|","?" and 0-31 ASCII control characters)
        /// and should contain only ASCII characters (not mandatory).
        /// </remarks>
        /// <returns>The path to the application directory where local data should be written.</returns>
        public static string GetAppLocalDataDirectory()
        {
            string path = Path.Combine(GetKnownDirectoryPath(KnownDirectory.LocalAppData), "CatUIApplication");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Returns the path to the directory defined by <see cref="KnownDirectory.RoamingAppData"/>, but ensures that
        /// a directory named with your application name (see `APP_NAME`) is created. Does not handle directory creation exceptions.
        /// </summary>
        /// <remarks>
        /// The application name must not have spaces in the name, illegal characters (see what are the illegal characters
        /// in a directory name on each operating system, generally "*","\"",:/","\","&lt;",">",":","|","?" and 0-31 ASCII control characters)
        /// and should contain only ASCII characters (not mandatory).
        /// </remarks>
        /// <returns>The path to the application directory where roaming data should be written.</returns>
        public static string GetAppRoamingDataDirectory()
        {
            string path = Path.Combine(GetKnownDirectoryPath(KnownDirectory.RoamingAppData), "CatUIApplication");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Specifies a list of the most used directories in most operating systems. Not all directories are present is all operating systems;
        /// check if a directory is available using `METHOD`. To get the path of the directory, use <see cref="GetKnownDirectoryPath(KnownDirectory)"/>.
        /// </summary>
        /// <remarks>
        /// This only contain the most important directories, so not all the directories from <see cref="Environment.SpecialFolder"/> are present.
        /// All directories are constant in an operating system, except <see cref="CurrentWorkingDirectory"/>,
        /// which depends on the location of the application. The Windows-only directories will have the "Win" prefix,
        /// these are also included because some offer significant features when used correctly, improving the user experience.
        /// </remarks>
        public enum KnownDirectory
        {
            /// <summary>
            /// The only non-constant directory from the list. It is the directory where the application is running
            /// and can change if the app is moved, but should never change during the application lifetime.
            /// </summary>
            CurrentWorkingDirectory = 0,

            /// <summary>
            /// The most used directory for storing all application-specific data. Data stored here are available
            /// only to the current user on the current machine.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\AppData\Local</item>
            /// <item>Linux: /home/USERNAME/.local/share</item>
            /// <item>Mac: /Users/USERNAME/.local/share</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// You should create a directory inside this directory with your app name to use it correctly.
            /// Equivalent to <see cref="Environment.SpecialFolder.LocalApplicationData"/>
            /// </remarks>
            LocalAppData = 1,

            /// <summary>
            /// Useful for storing application data that a user might want to use on multiple devices,
            /// like preferences and important files that will be available on multiple devices in a local network.
            /// Data stored here are available only to the current user on all machines the user is connected on.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\AppData\Roaming</item>
            /// <item>Linux: /home/USERNAME/.config</item>
            /// <item>Mac: /Users/USERNAME/.config</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// You should create a directory inside this directory with your app name to use it correctly.
            /// Equivalent to <see cref="Environment.SpecialFolder.ApplicationData"/>
            /// </remarks>
            RoamingAppData = 2,

            /// <summary>
            /// Useful for storing application data that should be available to all the users of a device.
            /// The files from this directory will be accessible to all the users, useful for non-user specific data.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// Data stored here are available to all users on the current machine.
            /// <list type="bullet">
            /// <item>Windows: C:\ProgramData</item>
            /// <item>Linux: /usr/share</item>
            /// <item>Mac: /usr/share</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// You should create a directory inside this directory with your app name to use it correctly.
            /// Equivalent to <see cref="Environment.SpecialFolder.CommonApplicationData"/>
            /// </remarks>
            GlobalAppData = 3,

            /// <summary>
            /// Useful for storing documents that the user might want to share with other users or applications.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\Documents</item>
            /// <item>Linux: /home/USERNAME/Documents</item>
            /// <item>Mac: /Users/USERNAME</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.MyDocuments"/>
            /// </remarks>
            Documents = 4,

            /// <summary>
            /// Useful for storing pictures.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\Pictures</item>
            /// <item>Linux: /home/USERNAME/Pictures</item>
            /// <item>Mac: /Users/USERNAME/Pictures</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.MyPictures"/>
            /// </remarks>
            Pictures = 5,

            /// <summary>
            /// Useful for storing music.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\Music</item>
            /// <item>Linux: /home/USERNAME/Music</item>
            /// <item>Mac: /Users/USERNAME/Music</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.MyMusic"/>
            /// </remarks>
            Music = 6,

            /// <summary>
            /// Useful for storing videos.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\Videos</item>
            /// <item>Linux: /home/USERNAME/Videos</item>
            /// <item>Mac: N/A</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.MyVideos"/>
            /// </remarks>
            Videos = 7,

            /// <summary>
            /// Useful for creating shortcuts of your application.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME\Desktop</item>
            /// <item>Linux: /home/USERNAME/Desktop</item>
            /// <item>Mac: /Users/USERNAME/Desktop</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.Desktop"/>
            /// </remarks>
            Desktop = 8,

            /// <summary>
            /// This is where the system-wide fonts are located, if the operating system has this kind of feature.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// <list type="bullet">
            /// <item>Windows: C:\WINDOWS\Fonts</item>
            /// <item>Linux: N/A</item>
            /// <item>Mac: /Users/USERNAME/Library/Fonts</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.Fonts"/>
            /// </remarks>
            Fonts = 9,

            /// <summary>
            /// Useful for creating shortcuts of your application.
            /// You should generally not create files or directories at this level, as its only purpose is
            /// to help you find other OS-specific folders for advanced use-cases.
            /// <list type="bullet">
            /// <item>Windows: C:\Users\USERNAME</item>
            /// <item>Linux: /home/USERNAME</item>
            /// <item>Mac: /Users/USERNAME</item>
            /// </list>
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.UserProfile"/>
            /// </remarks>
            UserProfile = 10,


            #region Windows-only

            /// <summary>
            /// Windows-only. Used for creating a shortcut to the application in the user's Start Menu.
            /// This will only be visible to the current user and will not be shared between users on the same machine.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.StartMenu"/>
            /// (C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu).
            /// </remarks>
            WinUserStartMenu = 1 | (1 << 7),

            /// <summary>
            /// Windows-only. This is where shortcuts to programs that run on system startup are stored.
            /// Only accessible by the current user.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.Startup"/>
            /// (C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup).
            /// </remarks>
            WinUserStartupPrograms = 2 | (1 << 7),

            /// <summary>
            /// Windows-only. Used for creating a shortcut to the application in the user's Start Menu for all users.
            /// This will be visible to all the users on the same machine.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.CommonStartMenu"/>
            /// (C:\ProgramData\Microsoft\Windows\Start Menu).
            /// </remarks>
            WinGlobalStartMenu = 3 | (1 << 7),

            /// <summary>
            /// Windows-only. This is where shortcuts to programs that run on system startup are stored. Accessible to all users,
            /// meaning programs listed here run on startup regardless of which user is logged in.
            /// Beware that access to this directory might be blocked by the operating system, always check the permission
            /// using `METHOD`.
            /// </summary>
            /// <remarks>
            /// Equivalent to <see cref="Environment.SpecialFolder.CommonStartup"/>
            /// (C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup).
            /// </remarks>
            WinGlobalStartupPrograms = 4 | (1 << 7),

            #endregion
        }
    }
}
