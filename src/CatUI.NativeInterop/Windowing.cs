using System;
using System.Runtime.InteropServices;

namespace CatUI.NativeInterop
{
    public static partial class Windowing
    {
        private const string LIBRARY_NAME = "NativeLibraries/CatUI.Native";

        public static bool InitializeGlfw()
        {
            return initializeGlfw() != 0;
        }

        public static void TerminateGlfw()
        {
            terminateGlfw();
        }


        public static nint CreateWindow(int width, int height, string title = "", WindowFlags windowFlags =
            WindowFlags.WindowModeWindowed |
            WindowFlags.WindowHintResizable |
            WindowFlags.WindowHintVisible |
            WindowFlags.WindowHintDecorated |
            WindowFlags.WindowHintDpiAware |
            WindowFlags.WindowHintFocused)
        {
            return createWindow(width, height, title, (int)windowFlags);
        }

        public static bool ReceivedCloseRequest(nint window)
        {
            return receivedCloseRequest(window) != 0;
        }

        public static void WaitForEvents()
        {
            waitForEvents();
        }

        public static void PollEvents()
        {
            pollEvents();
        }

        [LibraryImport(LIBRARY_NAME, EntryPoint = "initializeGlfw", StringMarshalling = StringMarshalling.Utf8)]
        private static partial int initializeGlfw();

        [LibraryImport(LIBRARY_NAME, EntryPoint = "terminateGlfw", StringMarshalling = StringMarshalling.Utf8)]
        private static partial void terminateGlfw();


        [LibraryImport(LIBRARY_NAME, EntryPoint = "createWindow", StringMarshalling = StringMarshalling.Utf8)]
        private static partial nint createWindow(int width, int height, string title, int windowFlags);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "receivedCloseRequest", StringMarshalling = StringMarshalling.Utf8)]
        private static partial int receivedCloseRequest(nint window);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "waitForEvents", StringMarshalling = StringMarshalling.Utf8)]
        private static partial void waitForEvents();

        [LibraryImport(LIBRARY_NAME, EntryPoint = "pollEvents", StringMarshalling = StringMarshalling.Utf8)]
        private static partial void pollEvents();

        [Flags]
        public enum WindowFlags
        {
            WindowModeWindowed = 0,
            WindowModeMinimized = 1,
            WindowModeMaximized = 2,
            WindowModeFullscreen = 3,
            WindowModeExclusiveFullscreen = 4,

            WindowHintResizable = 8,
            WindowHintVisible = 16,
            WindowHintDecorated = 32,
            WindowHintDpiAware = 64,
            WindowHintFocused = 128,

            WindowHintAlwaysOnTop = 256,
            WindowTransparentFramebuffer = 512,
        }
    }
}
