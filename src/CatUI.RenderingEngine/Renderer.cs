using System;
using CatUI.Data;
using CatUI.Data.Enums;
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

                // destroy the old surface
                Surface?.Dispose();
                Surface = null;
                Canvas = null;

                // re-create the render target
                _renderTarget?.Dispose();
                _renderTarget = new GRBackendRenderTarget((int)_newSize.Width, (int)_newSize.Height, _samples,
                    _stencilBits, _glInfo);
            }

            //create the surface
            if (Surface == null)
            {
                Surface = SKSurface.Create(Context, _renderTarget, SURFACE_ORIGIN, COLOR_TYPE);
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

        public float CalculateDimension(Dimension dimension, float dimensionForPercent = 0)
        {
            switch (dimension.MeasuringUnit)
            {
                default:
                case Unit.Dp:
                    return dimension.Value * _scale;
                case Unit.Pixels:
                    return dimension.Value;
                case Unit.Percent:
                    return
                        dimension.Value *
                        (dimensionForPercent == 0
                            ? Canvas?.DeviceClipBounds.Size.Width ?? 0
                            : dimensionForPercent) /
                        100f;
                case Unit.ViewportWidth:
                    return
                        dimension.Value * (Canvas?.DeviceClipBounds.Size.Width ?? 0) / 100f;
                case Unit.ViewportHeight:
                    return
                        dimension.Value * (Canvas?.DeviceClipBounds.Size.Height ?? 0) / 100f;
            }
        }
    }
}
