using System;
using System.Runtime.InteropServices;
using CatUI.Platform.CommonInterface;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Platform.Linux.OS;

public partial class SoftwareRendererLinux : ISoftwareRenderer
{
    #region Library P/Invoke

    //we use the very old OpenGL 1.1 spec, so we need a compatibility profile 

    [LibraryImport("libGL.so.1")]
    private static partial void glClear(uint mask);

    [LibraryImport("libGL.so.1")]
    private static partial void glClearColor(float r, float g, float b, float a);

    [LibraryImport("libGL.so.1")]
    private static partial void glRasterPos2i(int x, int y);

    [LibraryImport("libGL.so.1")]
    private static unsafe partial void glDrawPixels(int width, int height, uint format, uint type, void* pixels);

    [LibraryImport("libGL.so.1")]
    private static partial void glViewport(int x, int y, int width, int height);

    [LibraryImport("libGL.so.1")]
    private static partial void glMatrixMode(uint mode);

    [LibraryImport("libGL.so.1")]
    private static partial void glLoadIdentity();

    [LibraryImport("libGL.so.1")]
    private static partial void glOrtho(
        double left, double right,
        double bottom, double top,
        double zNear, double zFar);

    [LibraryImport("libGL.so.1")]
    private static partial void glPixelZoom(float x, float y);

    private const uint GL_COLOR_BUFFER_BIT = 0x00004000;
    private const uint GL_RGBA = 0x1908;
    private const uint GL_UNSIGNED_BYTE = 0x1401;
    private const uint GL_PROJECTION = 0x1701;
    private const uint GL_MODELVIEW = 0x1700;

    #endregion

    public unsafe void Draw(
        IntPtr nativeWindow,
        IntPtr pixelBuffer,
        int framebufferWidth,
        int framebufferHeight,
        int bytesPerRow,
        int windowWidth, int windowHeight)
    {
        glViewport(0, 0, framebufferWidth, framebufferHeight);

        //set up orthographic projection
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        glOrtho(0, framebufferWidth, 0, framebufferHeight, -1, 1);

        //flip the image
        glPixelZoom(1, -1);
        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();

        glClearColor(0f, 0f, 0f, 1f);
        glClear(GL_COLOR_BUFFER_BIT);
        glRasterPos2i(0, framebufferHeight);
        glDrawPixels(framebufferWidth, framebufferHeight, GL_RGBA, GL_UNSIGNED_BYTE, (void*)pixelBuffer);
        GLFW.SwapBuffers((Window*)nativeWindow);
    }

    public void Resized(int newWidth, int newHeight)
    {
        //no-op
    }
}
