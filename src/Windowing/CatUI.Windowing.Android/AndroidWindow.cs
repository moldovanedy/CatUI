using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Android.OS;
using Android.Util;
using Android.Views;
using CatUI.Data;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements;
using CatUI.Windowing.Android.PlatformImplementations;
using CatUI.Windowing.Common;
using SkiaSharp.Views.Android;
using Activity = Android.App.Activity;
using Environment = System.Environment;

namespace CatUI.Windowing.Android
{
    public class AndroidWindow : Activity, IApplicationWindow
    {
        #region Properties

        public int Width
        {
            get => (int)(FramebufferWidth / Document.ContentScale);
            set => throw new PlatformNotSupportedException("Android does not allow programmatic window resizing.");
        }

        public int Height
        {
            get => (int)(FramebufferHeight / Document.ContentScale);
            set => throw new PlatformNotSupportedException("Android does not allow programmatic window resizing.");
        }

        public bool IsDpiAware => true;

        public int FramebufferWidth
        {
            get
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    return Window?.WindowManager?.CurrentWindowMetrics.Bounds.Width() ?? 0;
                }

                DisplayMetrics metrics = new();
                Window?.WindowManager?.DefaultDisplay?.GetMetrics(metrics);
                return metrics.WidthPixels;
            }
        }

        public int FramebufferHeight
        {
            get
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    return Window?.WindowManager?.CurrentWindowMetrics.Bounds.Height() ?? 0;
                }

                DisplayMetrics metrics = new();
                Window?.WindowManager?.DefaultDisplay?.GetMetrics(metrics);
                return metrics.HeightPixels;
            }
        }

        public UiDocument Document { get; }

        public Func<bool> OnCloseRequested { get; set; } = () => true;

        #endregion

        public event WindowResizedEventHandler? ResizedEvent;
        public event Action<double>? FrameUpdatedEvent;

        private readonly List<Action<double>> _animationFrameCallbacks = [];
        private Choreographer? _choreographer;
        private readonly Stopwatch _stopwatch = new();

        private SKCanvasView? _canvasView;

        public AndroidWindow()
        {
            Document = new UiDocument(true, new Data.Size(0, 0));
            ResizedEvent += OnResize;
        }

        #region Activity lifecycle

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Document.ContentScale = ApplicationContext?.Resources?.DisplayMetrics?.Density ?? 1f;

            if (_canvasView != null)
            {
                _canvasView.PaintSurface -= OnCanvasRedrawRequested;
                _canvasView.LayoutChange -= OnCanvasLayoutChange;

                _canvasView.Touch -= OnTouchEvent;
            }

            RequestWindowFeature(WindowFeatures.NoTitle);
            _canvasView = new SKCanvasView(ApplicationContext);
            SetContentView(_canvasView);

            _canvasView.PaintSurface += OnCanvasRedrawRequested;
            _canvasView.LayoutChange += OnCanvasLayoutChange;

            _canvasView.Touch += OnTouchEvent;

            _choreographer = Choreographer.Instance;
            SetupCallbacks();
        }

        #endregion

        #region Event handling

        private readonly Dictionary<int, Point2D> activePointers = [];

        private void OnTouchEvent(object? sender, View.TouchEventArgs e)
        {
            if (e.Event == null)
            {
                return;
            }

            int pointerIndex = e.Event.ActionIndex;
            int pointerId = e.Event.GetPointerId(pointerIndex);
            MotionEventActions action = e.Event.ActionMasked;

            switch (action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        MotionEvent.PointerCoords coords = new();
                        e.Event.GetPointerCoords(pointerIndex, coords);
                        Point2D pos = new(coords.X, coords.Y);
                        activePointers[pointerId] = pos;

                        Document.SimulatePointerEnter(
                            new PointerEnterEventArgs(pos, pos, false));
                        Document.SimulatePointerDown(
                            new PointerDownEventArgs(pos, pos));
                        break;
                    }

                case MotionEventActions.Move:
                    {
                        //handle all batched events
                        for (int i = 0; i < e.Event.HistorySize; i++)
                        {
                            for (int ptr = 0; ptr < e.Event.PointerCount; ptr++)
                            {
                                int id = e.Event.GetPointerId(ptr);
                                if (!activePointers.TryGetValue(id, out Point2D previousPos))
                                {
                                    continue;
                                }

                                MotionEvent.PointerCoords coords = new();
                                e.Event.GetHistoricalPointerCoords(ptr, i, coords);
                                Point2D pos = new(coords.X, coords.Y);
                                activePointers[id] = pos;

                                Document.SimulatePointerMove(
                                    new PointerMoveEventArgs(
                                        pos,
                                        pos,
                                        pos.X - previousPos.X,
                                        pos.Y - previousPos.Y,
                                        true));
                            }
                        }

                        //handle the latest one
                        for (int ptr = 0; ptr < e.Event.PointerCount; ptr++)
                        {
                            int id = e.Event.GetPointerId(ptr);
                            if (!activePointers.TryGetValue(id, out Point2D previousPos))
                            {
                                continue;
                            }

                            MotionEvent.PointerCoords coords = new();
                            e.Event.GetPointerCoords(ptr, coords);
                            Point2D pos = new(coords.X, coords.Y);
                            activePointers[id] = pos;

                            Document.SimulatePointerMove(
                                new PointerMoveEventArgs(
                                    pos,
                                    pos,
                                    pos.X - previousPos.X,
                                    pos.Y - previousPos.Y,
                                    true));
                        }

                        break;
                    }

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Cancel:
                    {
                        if (!activePointers.TryGetValue(pointerId, out Point2D pos))
                        {
                            break;
                        }

                        Document.SimulatePointerUp(
                            new PointerUpEventArgs(
                                pos,
                                pos,
                                action == MotionEventActions.Cancel));
                        Document.SimulatePointerExit(
                            new PointerExitEventArgs(pos, pos, false));

                        activePointers.Remove(pointerId);
                        break;
                    }
            }
        }

        #endregion

        /// <inheritdoc cref="IApplicationWindow.Close"/>
        /// <remarks>
        /// This will immediately close the application (using Finish and then Environment.Exit(0)), so
        /// running threads might not finish and be immediately destroyed. It is generally NOT recommended to close the
        /// app this way on Android and iOS and instead let the platform figure out when to close the app. 
        /// </remarks>
        public void Close()
        {
            Finish();
            Environment.Exit(0);
        }

        public void RequestAnimationFrame(Action<double> frameCallback)
        {
            _animationFrameCallbacks.Add(frameCallback);
        }

        private void SetupCallbacks()
        {
            _choreographer?.PostFrameCallback(new FrameCallback(this));
        }

        private void OnCanvasRedrawRequested(object? sender, SKPaintSurfaceEventArgs e)
        {
            Document.Renderer.BeginDraw();
            Document.Renderer.SetPlatformManagedData(e.Surface, e.Surface.Canvas);
            DoFrameActions(_stopwatch.Elapsed.TotalSeconds);
            Document.Renderer.EndDraw();

            _stopwatch.Restart();
        }

        private void OnCanvasLayoutChange(object? sender, View.LayoutChangeEventArgs e)
        {
            int lastWidth = e.OldRight - e.OldLeft;
            int lastHeight = e.OldBottom - e.OldTop;
            int currentWidth = e.Right - e.Left;
            int currentHeight = e.Bottom - e.Top;

            if (currentWidth != lastWidth || currentHeight != lastHeight)
            {
                ResizedEvent?.Invoke(this,
                    new WindowResizedEventArgs(lastWidth, lastHeight, currentWidth, currentHeight));
            }
        }

        private void FullyRedraw()
        {
            Document.Renderer.ResetAndClear();
            Document.DrawAllElements();
            Document.Renderer.Flush();
        }

        private void DoFrameActions(double delta)
        {
            if (_animationFrameCallbacks.Count > 0)
            {
                //if a callback registers another callback, this will effectively become an infinite loop,
                //to prevent this, before executing all the callbacks, store their number
                //and only execute that number of callbacks
                int thisFrameCount = _animationFrameCallbacks.Count;

                for (int i = 0; i < thisFrameCount; i++)
                {
                    _animationFrameCallbacks[i].Invoke(delta);
                }

                _animationFrameCallbacks.RemoveRange(0, thisFrameCount);
            }

            if (CatApplication.Instance.AppInitializer != null &&
                CatApplication.Instance.Dispatcher is AndroidDispatcher dispatcher)
            {
                dispatcher.CallActions();
            }

            if (Document.Renderer.IsCanvasDirty)
            {
                //we can only draw when the draw call is coming from OnCanvasRedrawRequested, so otherwise we request
                //redrawing
                if (Document.Renderer.CanDraw)
                {
                    FrameUpdatedEvent?.Invoke(delta);
                    FullyRedraw();
                    Document.Renderer.SkipCanvasPresentation();
                }
                else
                {
                    _canvasView?.Invalidate();
                }
            }
        }

        private void OnResize(object sender, WindowResizedEventArgs e)
        {
            DocumentInvoke("WndSetViewportSize", new Data.Size(e.NewWidth, e.NewHeight));
            Document.Renderer.SetCanvasDirty();
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


        internal class FrameCallback : Java.Lang.Object, Choreographer.IFrameCallback
        {
            private readonly AndroidWindow _window;

            internal FrameCallback(AndroidWindow window)
            {
                _window = window;
            }

            public void DoFrame(long currentTimestamp)
            {
                _window.DoFrameActions(_window._stopwatch.Elapsed.TotalSeconds);
                _window._stopwatch.Restart();
                //loop
                _window._choreographer?.PostFrameCallback(this);
            }
        }
    }
}
