using System;
using CatUI.Data;
using CatUI.Data.Shapes;
using SkiaSharp;

namespace CatUI.RenderingEngine
{
    public partial class Renderer
    {
        public SKSurface? Surface { get; private set; }
        public SKCanvas? Canvas { get; private set; }
        public GRContext? Context { get; private set; }

        public bool IsCanvasDirty { get; private set; }

        private const SKColorType COLOR_TYPE = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SURFACE_ORIGIN = GRSurfaceOrigin.BottomLeft;

        private GRGlFramebufferInfo _glInfo;
        private GRBackendRenderTarget? _renderTarget;
        private SKSize _lastSize;
        private SKSize _newSize;

        private Color _bgColor;
        private int _framebufferBinding;
        private int _stencilBits;
        private int _samples;
        private float _scale = 1;

        public void SetContentScale(float scale)
        {
            _scale = scale;
        }

        public float GetContentScale()
        {
            return _scale;
        }

        public void SetFramebufferData(int fbBinding, int stencilBits, int samples)
        {
            _framebufferBinding = fbBinding;
            _stencilBits = stencilBits;
            _samples = samples;
        }

        public void SetNewSize(SKSize size)
        {
            _newSize = size;
        }

        public void SetBgColor(Color backgroundColor)
        {
            _bgColor = backgroundColor;
        }

        /// <summary>
        /// Will clear the viewport with the viewport's background color,
        /// but will also take care of recreating the surface if needed (for example on a window resize).
        /// </summary>
        public void ResetAndClear()
        {
            //create the contexts if not done already
            if (Context == null)
            {
                var glInterface = GRGlInterface.Create();
                Context = GRContext.CreateGl(glInterface);

                if (Context == null)
                {
                    throw new NullReferenceException(
                        "Graphics context is null. This is probably an internal graphics error.");
                }
            }

            //manage the drawing surface
            if (_renderTarget == null || _lastSize != _newSize || !_renderTarget.IsValid)
            {
                _lastSize = _newSize;

                int maxSamples = Context.GetMaxSurfaceSampleCount(COLOR_TYPE);
                if (_samples > maxSamples)
                {
                    _samples = maxSamples;
                }

                _glInfo = new GRGlFramebufferInfo((uint)_framebufferBinding, COLOR_TYPE.ToGlSizedFormat());

                //destroy the old surface
                Surface?.Dispose();
                Surface = null;
                Canvas = null;

                //re-create the render target
                _renderTarget?.Dispose();
                _renderTarget = new GRBackendRenderTarget((int)_newSize.Width, (int)_newSize.Height, _samples,
                    _stencilBits, _glInfo);
            }

            //create the surface
            if (Surface == null)
            {
                Surface = SKSurface.Create(Context, _renderTarget, SURFACE_ORIGIN, COLOR_TYPE);
                if (Surface == null)
                {
                    throw new NullReferenceException(
                        "Drawing surface is null. This is probably an internal graphics error.");
                }

                Canvas = Surface.Canvas;
            }

            ArgumentNullException.ThrowIfNull(Canvas);
            using (new SKAutoCanvasRestore(Canvas, true))
            {
                Canvas.Clear(_bgColor);
            }
        }

        /// <summary>
        /// Flushes the SkiaSharp contents to GL.
        /// </summary>
        public void Flush()
        {
            Canvas?.Flush();
            Context?.Flush();
        }

        public void SetCanvasDirty()
        {
            IsCanvasDirty = true;
        }

        /// <summary>
        /// Will make the canvas appear "clean" by setting <see cref="IsCanvasDirty"/> to false.
        /// Should only be called by the internal windowing system when the updated interface is presented
        /// or in special circumstances when you simply don't want to present the updated interface.
        /// </summary>
        /// <remarks>
        /// Will not stop the redrawing internally, so there aren't any performance benefits from calling this.
        /// </remarks>
        public void SkipCanvasPresentation()
        {
            IsCanvasDirty = false;
        }

        #region Layered drawing

        /// <summary>
        /// Analog to <see cref="SKCanvas.Save"/>. Pushes the current canvas state on a stack.
        /// </summary>
        /// <returns>The value that should be given to <see cref="RestoreCanvasState(int)"/> to return to this state.</returns>
        /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
        public int SaveCanvasState()
        {
            if (Canvas == null)
            {
                throw new NullReferenceException("Canvas was null.");
            }

            return Canvas.Save();
        }

        /// <summary>
        /// Analog to <see cref="SKCanvas.RestoreToCount"/>. Restores the canvas state to the given state on the stack.
        /// Use <see cref="RestoreCanvasState()"/> to only go back one state.
        /// </summary>
        /// <param name="initialState">The state to return to. It's the value returned by <see cref="SaveCanvasState"/>.</param>
        /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
        public void RestoreCanvasState(int initialState)
        {
            if (Canvas == null)
            {
                throw new NullReferenceException("Canvas was null.");
            }

            Canvas.RestoreToCount(initialState);
        }

        /// <summary>
        /// Analog to <see cref="SKCanvas.Restore"/>. Restores the previous canvas state (the one before the call to
        /// <see cref="SaveCanvasState"/>).
        /// </summary>
        /// <exception cref="NullReferenceException">If <see cref="Canvas"/> is null.</exception>
        public void RestoreCanvasState()
        {
            if (Canvas == null)
            {
                throw new NullReferenceException("Canvas was null.");
            }

            Canvas.Restore();
        }

        #endregion

        #region Clipping

        /// <summary>
        /// Sets the clip region as the given rect. The coordinates are absolute pixel coordinates.
        /// </summary>
        /// <param name="clipRect"></param>
        public void SetClipRect(Rect clipRect)
        {
            Canvas?.ClipRect(clipRect, SKClipOperation.Intersect, true);
        }

        /// <summary>
        /// Sets the clip region as the given path. It is generally much slower than <see cref="SetClipRect"/>.
        /// The coordinates are absolute pixel coordinates.
        /// </summary>
        /// <param name="clipPath"></param>
        public void SetClipPath(PathClipShape clipPath)
        {
            Canvas?.ClipPath(clipPath.SkiaPath, SKClipOperation.Intersect, true);
        }

        #endregion
    }
}
