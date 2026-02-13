using System;
using System.Collections.Generic;
using System.Reflection;
using CatUI.Data;
using CatUI.Elements;
using CatUI.Windowing.Common;
using SkiaSharp;

namespace CatUI.Windowing.Headless;

/// <summary>
/// A minimal surface implementation that can be used in advanced scenarios, like drawing in applications that already
/// use a drawing context (e.g., 3D applications, games).
/// </summary>
public class HeadlessSurface
{
    public UiDocument Document { get; }
    private IGraphicsBackend GraphicsBackend { get; }

    /// <summary>
    /// Fired when the window is "dirty" and it needs a repaint, either partially or fully. This is fired before the
    /// redrawing.
    /// </summary>
    public event Action<double>? FrameUpdatedEvent;

    private readonly List<Action<double>> _animationFrameCallbacks = [];

    /// <summary>
    /// Creates a new surface used for specialized drawing.
    /// </summary>
    /// <param name="viewportSize">
    /// The initial size of the viewport. This will be scaled according to contentScale, so this is NOT the framebuffer
    /// size, but rather the framebuffer size divided by the content scale.
    /// </param>
    /// <param name="graphicsBackend">
    /// An implementation for a graphics backend. You must implement its methods to use this class.
    /// </param>
    /// <param name="contentScale">
    /// The content scale of the UI. This is generally taken from the runtime platform but can also remain at 1, in
    /// which case all dimensions will be the same pixel size on all devices, regardless of user preferences (this is
    /// generally not desired).
    /// </param>
    public HeadlessSurface(
        Size viewportSize,
        IGraphicsBackend graphicsBackend,
        float contentScale = 1f)
    {
        GraphicsBackend = graphicsBackend;

        Document = new UiDocument(viewportSize, contentScale);
        SetContentScale(contentScale);
        Document.SetAnimationFrameAdder(RequestAnimationFrame);
    }

    /// <inheritdoc cref="IApplicationWindow.RequestAnimationFrame"/>
    public void RequestAnimationFrame(Action<double> frameCallback)
    {
        _animationFrameCallbacks.Add(frameCallback);
    }

    /// <summary>
    /// This functions does the actual rendering: it invokes all the callbacks from <see cref="RequestAnimationFrame"/>,
    /// it invokes <see cref="FrameUpdatedEvent"/>, it redraws everything (if needed), and finally presents it.
    /// </summary>
    /// <remarks>
    /// This does NOT call the dispatcher. It is your responsibility to call the dispatcher (e.g., DesktopDispatcher)
    /// to ensure CatUI internals work properly.
    /// </remarks>
    /// <param name="delta">
    /// The time it passed from the last frame to this frame. Only used for callbacks registered with
    /// <see cref="RequestAnimationFrame"/>.
    /// </param>
    /// <param name="forceRedraw">
    /// If true, will force a full redrawing even if nothing changed (this involves recreating the surface if needed
    /// (<see cref="IGraphicsBackend.RecreateSurface"/>) and then drawing all elements).
    /// </param>
    public void DoFrameActions(double delta, bool forceRedraw = false)
    {
        bool hadFrameCallbacks = false;
        if (_animationFrameCallbacks.Count > 0)
        {
            //if a callback registers another callback, this will effectively become an infinite loop,
            //to prevent this, before executing all the callbacks, store their number
            //and only execute that number of callbacks
            int thisFrameCount = _animationFrameCallbacks.Count;

            for (int i = 0; i < thisFrameCount; i++)
            {
                _animationFrameCallbacks[i].Invoke(delta);
                hadFrameCallbacks = true;
            }

            _animationFrameCallbacks.RemoveRange(0, thisFrameCount);
        }

        if (forceRedraw || hadFrameCallbacks || Document.Renderer.IsCanvasDirty)
        {
            if (forceRedraw || Document.Renderer.IsCanvasDirty)
            {
                FrameUpdatedEvent?.Invoke(delta);
                FullyRedraw();
            }

            GraphicsBackend.PresentFramebuffer();

            if (Document.Renderer.IsCanvasDirty)
            {
                Document.Renderer.SkipCanvasPresentation();
            }
        }
    }

    /// <summary>
    /// Will modify the app state according to app termination (see remarks), will detach all elements from the
    /// document and will call <see cref="IGraphicsBackend.DestroyAndTerminate"/> for <see cref="GraphicsBackend"/>.
    /// </summary>
    /// <remarks>
    /// The app state will go as following: Active -> Inactive -> Hidden -> Detached (starting from its current state,
    /// so if the app is already hidden, it will NOT enter Active and Inactive).
    /// </remarks>
    public void Terminate()
    {
        if (Document.CurrentAppState == UiDocument.AppState.Active)
        {
            SetAppState(UiDocument.AppState.Inactive);
        }

        if (Document.CurrentAppState == UiDocument.AppState.Inactive)
        {
            SetAppState(UiDocument.AppState.Hidden);
        }

        if (Document.CurrentAppState == UiDocument.AppState.Hidden)
        {
            SetAppState(UiDocument.AppState.Detached);
        }

        //remove all the elements from the document
        Document.Root = null;
        GraphicsBackend.DestroyAndTerminate();
    }

    /// <summary>
    /// Will set a new content scale, which will trigger a repaint.
    /// </summary>
    /// <param name="contentScale">The new content scale.</param>
    public void SetContentScale(float contentScale)
    {
        DocumentInvoke("WndSetContentScale", contentScale);
    }

    /// <summary>
    /// Sets the internal app state for this document (i.e. <see cref="UiDocument.CurrentAppState"/>).
    /// </summary>
    /// <param name="newState">The new app state.</param>
    public void SetAppState(UiDocument.AppState newState)
    {
        DocumentInvoke("WndSetAppState", newState);
    }

    /// <summary>
    /// Will resize the viewport used for drawing. Will repaint the next frame. Note that this will be scaled according
    /// to the given content scale, so this is not always the framebuffer size, but rather the framebuffer size
    /// divided by the content scale.
    /// </summary>
    /// <param name="newSize">The new viewport size.</param>
    public void ResizeViewport(Size newSize)
    {
        Size framebufferSize = new(
            (int)(newSize.Width * Document.ContentScale), (int)(newSize.Height * Document.ContentScale));

        DocumentInvoke("WndSetFramebufferSize", framebufferSize);
        GraphicsBackend.Resized(
            (int)(newSize.Width * Document.ContentScale), (int)(newSize.Height * Document.ContentScale));

        Document.Renderer.SetCanvasDirty();
        // DoFrameActions();
    }


    private void FullyRedraw()
    {
        Document.Renderer.BeginDraw();

        SKSurface newSurface = GraphicsBackend.RecreateSurface(Document.Renderer.Surface!);
        if (newSurface != Document.Renderer.Surface)
        {
            Document.Renderer.SetPlatformManagedData(newSurface, newSurface.Canvas);
        }

        Document.Renderer.ResetAndClear();

        Document.DrawAllElements();
        Document.Renderer.Flush();
        Document.Renderer.EndDraw();
    }

    /// <summary>
    /// Dangerously calls non-internal instance methods from <see cref="Document"/>. These are necessary to make
    /// sure we don't have public access to those setters, only implementations of <see cref="IApplicationWindow"/>
    /// should be allowed to modify those.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="args">The arguments to give.</param>
    private void DocumentInvoke(string methodName, params object[] args)
    {
        MethodInfo? func = Document.GetType().GetMethod(
            methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (func != null)
        {
            func.Invoke(Document, args);
        }
    }
}
