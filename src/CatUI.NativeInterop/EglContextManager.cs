using System.Runtime.InteropServices;

namespace CatUI.NativeInterop
{
    public static partial class EglContextManager
    {
        private const string LIBRARY_NAME = "NativeLibraries/CatUI.Native";

        public static bool SetupEgl(out nint context, out nint display, out nint surface, nint window)
        {
            return setupEgl(0, out context, out display, out surface, window) != 0;
        }

        public static void TerminateEgl(nint display, nint surface, nint context)
        {
            terminateEgl(display, surface, context);
        }

        public static void SwapBuffers(nint display, nint surface)
        {
            swapBuffers(display, surface);
        }

        [LibraryImport(LIBRARY_NAME, EntryPoint = "setupEgl", StringMarshalling = StringMarshalling.Utf8)]
        private static partial int setupEgl(int renderingBackend, out nint context, out nint display, out nint surface, nint window);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "terminateEgl", StringMarshalling = StringMarshalling.Utf8)]
        private static partial void terminateEgl(nint display, nint surface, nint context);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "swapBuffers", StringMarshalling = StringMarshalling.Utf8)]
        private static partial void swapBuffers(nint display, nint surface);
    }
}
