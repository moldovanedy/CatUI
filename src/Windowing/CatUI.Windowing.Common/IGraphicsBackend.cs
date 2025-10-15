using SkiaSharp;

namespace CatUI.Windowing.Common;

/// <summary>
/// This is the interface for the different graphics backends on the target platforms, especially desktop.
/// </summary>
public interface IGraphicsBackend
{
    const SKColorType COLOR_TYPE = SKColorType.Rgba8888;
    const GRSurfaceOrigin SURFACE_ORIGIN = GRSurfaceOrigin.BottomLeft;

    /// <summary>
    /// This will be called before creating the window, so it's a good place to set the minimum required version
    /// of the graphics API or other pre-window creation setup.
    /// </summary>
    void PrepareWindowCreation();

    /// <summary>
    /// This is after the window was created, useful for loading the eventual API bindings for the selected
    /// graphics API.
    /// </summary>
    void PostWindowCreation();

    /// <summary>
    /// This will be responsible for (re)creating the drawing surface when needed. Will return the previousSurface
    /// if no recreation is necessary.
    /// </summary>
    /// <param name="previousSurface">The previous surface that was drawn onto.</param>
    SKSurface RecreateSurface(SKSurface previousSurface);

    /// <summary>
    /// This will be called when the canvas was already redrawn, and now it needs to be presented to the user.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// This is called when the window is terminated, useful for freeing memory used by graphics objects.
    /// </summary>
    void DestroyAndTerminate();

    /// <summary>
    /// This will be called before <see cref="RecreateSurface"/> to notify the backend that the surface was resized.
    /// </summary>
    /// <param name="width">The new width of the surface.</param>
    /// <param name="height">The new height of the surface.</param>
    void Resized(int width, int height);

    /// <summary>
    /// Called when the swap interval changes. 0 means unsynchronized; 1 means V-Sync; n is refresh-rate / n.
    /// </summary>
    /// <param name="swapInterval"></param>
    void SwapIntervalChanged(int swapInterval);
}
