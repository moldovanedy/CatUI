using System.Runtime.InteropServices;

namespace CatUI.NativeInterop
{
    public partial class EGL
    {
        private const string LIBRARY_NAME = "NativeLibraries/libEGL";

        public static bool Initialize(nint display, ref int major, ref int minor)
        {
            return eglInitialize(display, ref major, ref minor) != 0;
        }

        public static bool Terminate(nint display)
        {
            return eglTerminate(display) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="nativeDisplay"></param>
        /// <param name="attribList"></param>
        /// <returns>EGLDisplay</returns>
        public static nint GetPlatformDisplay(uint platform, nint nativeDisplay, long[] attribList)
        {
            return eglGetPlatformDisplay(platform, nativeDisplay, attribList);
        }

        public static bool ChooseConfig(nint display, int[] attribList, nint config, int configSize, int[] numConfig)
        {
            return eglChooseConfig(display, attribList, config, configSize, numConfig) != 0;
        }

        public static nint CreateContext(nint display, nint config, nint shareContext, int[] attribList)
        {
            return eglCreateContext(display, config, shareContext, attribList);
        }

        public static nint CreateWindowSurface(nint display, nint config, nint nativeWindow, int[] attribList)
        {
            return eglCreateWindowSurface(display, config, nativeWindow, attribList);
        }

        public static bool MakeCurrent(nint display, nint drawSurface, nint readSurface, nint context)
        {
            return eglMakeCurrent(display, drawSurface, readSurface, context) != 0;
        }

        public static bool SwapBuffers(nint display, nint surface)
        {
            return eglSwapBuffers(display, surface) != 0;
        }

        public static bool DestroySurface(nint display, nint surface)
        {
            return eglDestroySurface(display, surface) != 0;
        }

        public static bool DestroyContext(nint display, nint context)
        {
            return eglDestroyContext(display, context) != 0;
        }

        //EGLenum = uint
        //EGLAttrib = long
        //EGLBoolean = uint

        //nint = EGLConfig, EGLSurface, EGLContext, EGLDisplay

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglInitialize", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglInitialize(nint display, ref int major, ref int minor);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglGetPlatformDisplay", StringMarshalling = StringMarshalling.Utf8)]
        private static partial nint eglGetPlatformDisplay(uint platform, nint nativeDisplay, [In] long[] attribList);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglChooseConfig", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglChooseConfig(nint display, [In] int[] attribList, nint config, int configSize, [In] int[] numConfig);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglCreateContext", StringMarshalling = StringMarshalling.Utf8)]
        private static partial nint eglCreateContext(nint display, nint config, nint shareContext, [In] int[] attribList);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglCreateWindowSurface", StringMarshalling = StringMarshalling.Utf8)]
        private static partial nint eglCreateWindowSurface(nint display, nint config, nint nativeWindow, [In] int[] attribList);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglMakeCurrent", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglMakeCurrent(nint display, nint drawSurface, nint readSurface, nint context);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglSwapBuffers", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglSwapBuffers(nint display, nint surface);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglTerminate", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglTerminate(nint display);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglDestroySurface", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglDestroySurface(nint display, nint surface);

        [LibraryImport(LIBRARY_NAME, EntryPoint = "eglDestroyContext", StringMarshalling = StringMarshalling.Utf8)]
        private static partial uint eglDestroyContext(nint display, nint context);
    }
}
