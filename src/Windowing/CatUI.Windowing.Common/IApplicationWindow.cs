using System;
using CatUI.Elements;

namespace CatUI.Windowing.Common
{
    /// <summary>
    /// The base for all application windows. This interface contains all the methods that are common to all platforms.
    /// Most application windows will also implement other interfaces, as the app lifecycle is different from platform to
    /// platform.
    /// </summary>
    public interface IApplicationWindow
    {
        /// <summary>
        /// Represents the width of the window without any decoration, scaled according to user preferences if
        /// <see cref="IsDpiAware"/> is true. Setting this will resize the window on supporting platforms and will
        /// throw <see cref="PlatformNotSupportedException"/> if the platform doesn't allow programmatic window resizing
        /// (e.g. mobile).
        /// </summary>
        /// <remarks>For the unscaled equivalent, see <see cref="FramebufferWidth"/>.</remarks>
        public int Width { get; set; }

        /// <summary>
        /// Represents the height of the window without any decoration, scaled according to user preferences if
        /// <see cref="IsDpiAware"/> is true. Setting this will resize the window on supporting platforms and will
        /// throw <see cref="PlatformNotSupportedException"/> if the platform doesn't allow programmatic window resizing
        /// (e.g. mobile).
        /// </summary>
        /// <remarks>For the unscaled equivalent, see <see cref="FramebufferHeight"/>.</remarks>
        public int Height { get; set; }

        /// <summary>
        /// If true, it means the window respects the platform scaling that is generally set by the users. This means that
        /// <see cref="Width"/> and <see cref="Height"/> do NOT map to pixels 1:1, but rather they are scaled.
        /// This is generally the best behavior because CatUI will manage scaling for you, even if the scale is
        /// adjusted by the platform during runtime.
        /// </summary>
        public bool IsDpiAware { get; }

        /// <summary>
        /// Returns the width of the frame buffer, meaning the unscaled equivalent of <see cref="Width"/>.
        /// This represents direct pixel values.
        /// </summary>
        public int FramebufferWidth { get; }

        /// <summary>
        /// Returns the height of the frame buffer, meaning the unscaled equivalent of <see cref="Height"/>.
        /// This represents direct pixel values.
        /// </summary>
        public int FramebufferHeight { get; }

        /// <summary>
        /// Represents the UI document attached to the window. This is NOT platform-specific, it is an abstraction
        /// provided by CatUI.
        /// </summary>
        public UiDocument Document { get; }

        /// <summary>
        /// Called when the user or the platform requested the application close. Returning true (the default behavior)
        /// will close the window immediately, while returning false will make the window continue running.
        /// This can be useful for implementing a prompt for the user (e.g. if they would like to save changes before the app is closed).
        /// </summary>
        /// <remarks>
        /// Although the window will be closed after this returns true, your app will still run until the end of the Main function.
        /// This is ignored on mobile platforms (Android and iOS) because they don't have a "close" button, and the app
        /// lifecycle is radically different from desktop and web.
        /// </remarks>
        public Func<bool> OnCloseRequested { get; set; }

        /// <summary>
        /// Closes this window by releasing all its resources, if any.
        /// </summary>
        public void Close();

        /// <summary>
        /// This function works similarly to the requestAnimationFrame web API. The given callback will be executed
        /// before starting to draw the next frame.
        /// </summary>
        /// <param name="frameCallback">
        /// A function that receives the time between the last frame and the current frame in seconds as a parameter.
        /// </param>
        public void RequestAnimationFrame(Action<double> frameCallback);

        /// <summary>
        /// Fired when the platform's window manager detected a resize done by the user or when the window was resized
        /// programmatically (if the platform supports this). When you handle this event, the new values might have
        /// already been set to this instance (<see cref="Width"/> and <see cref="Height"/>), but you have the old
        /// values in this event as well.
        /// </summary>
        public event WindowResizedEventHandler? ResizedEvent;

        /// <summary>
        /// An event that is fired when the internal windowing system decides to redraw
        /// a part of the viewport or the whole viewport. Do NOT use this as a "game loop", as this won't be invoked
        /// regularly, but rather only when it's necessary (something has changed visually).
        /// </summary>
        /// <remarks>
        /// If you want a reliable continuous loop for each frame, see <see cref="RequestAnimationFrame(Action{double})"/>.
        /// This event won't be invoked even when using <see cref="RequestAnimationFrame(Action{double})"/>
        /// unless something really has been drawn.
        /// </remarks>
        public event Action<double>? FrameUpdatedEvent;
    }
}
