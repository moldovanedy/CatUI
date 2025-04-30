using System;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Utils;
using SkiaSharp;

namespace CatUI.Elements.Media
{
    public class ImageView : Element
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<ImageView>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<ImageView>? _ref;

        /// <summary>
        /// Represents the source image object that will be drawn. It is always the original image an is not subject
        /// to internal resizing (it might get resized, but everything will only affect a copy of this, not the original
        /// image). The default value is null, meaning nothing will be drawn.
        /// </summary>
        public Image? Source
        {
            get => _source;
            set
            {
                SetSource(value);
                SourceProperty.Value = value;
            }
        }

        private Image? _source;
        public ObservableProperty<Image> SourceProperty { get; } = new();

        private void SetSource(Image? value)
        {
            _source = value;
            MarkLayoutDirty();
        }

        /// <summary>
        /// Specifies the image's horizontal alignment (on the X axis). The default value is
        /// <see cref="HorizontalAlignmentType.Left"/>.
        /// </summary>
        public HorizontalAlignmentType HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                SetHorizontalAlignment(value);
                HorizontalAlignmentProperty.Value = value;
            }
        }

        private HorizontalAlignmentType _horizontalAlignment = HorizontalAlignmentType.Left;

        public ObservableProperty<HorizontalAlignmentType> HorizontalAlignmentProperty { get; }
            = new(HorizontalAlignmentType.Left);

        private void SetHorizontalAlignment(HorizontalAlignmentType value)
        {
            _horizontalAlignment = value;
            RequestRedraw();
        }

        /// <summary>
        /// Specifies the image's vertical alignment (on the Y axis). The default value is
        /// <see cref="VerticalAlignmentType.Top"/>.
        /// </summary>
        public VerticalAlignmentType VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                SetVerticalAlignment(value);
                VerticalAlignmentProperty.Value = value;
            }
        }

        private VerticalAlignmentType _verticalAlignment = VerticalAlignmentType.Top;

        public ObservableProperty<VerticalAlignmentType> VerticalAlignmentProperty { get; }
            = new(VerticalAlignmentType.Top);

        private void SetVerticalAlignment(VerticalAlignmentType value)
        {
            _verticalAlignment = value;
            RequestRedraw();
        }

        /// <summary>
        /// Specifies whether the image should keep its aspect ratio or not when it's subject to resize. When this is false,
        /// it will allow the image to be resized freely, which means the image might get "squished" because the original
        /// width/height ratio is not respected (it depends on your use case whether this is bad or not). The default
        /// value is true.
        /// </summary>
        public bool ShouldKeepAspectRatio
        {
            get => _shouldKeepAspectRatio;
            set
            {
                SetShouldKeepAspectRatio(value);
                ShouldKeepAspectRatioProperty.Value = value;
            }
        }

        private bool _shouldKeepAspectRatio = true;
        public ObservableProperty<bool> ShouldKeepAspectRatioProperty { get; } = new(true);

        private void SetShouldKeepAspectRatio(bool value)
        {
            _shouldKeepAspectRatio = value;
            MarkLayoutDirty();
        }

        /// <summary>
        /// Specifies how will the image fit inside the element's space. The default value is
        /// <see cref="ImageFitType.CanShrink"/>.
        /// </summary>
        public ImageFitType ImageFit
        {
            get => _imageFit;
            set
            {
                SetImageFit(value);
                ImageFitProperty.Value = value;
            }
        }

        private ImageFitType _imageFit = ImageFitType.CanShrink;
        public ObservableProperty<ImageFitType> ImageFitProperty { get; } = new(ImageFitType.CanShrink);

        private void SetImageFit(ImageFitType value)
        {
            _imageFit = value;
            RequestRedraw();
        }

        /// <summary>
        /// Specifies the quality of the drawn image after it has been resized (or simply the quality of the resizing
        /// algorithm). Lower quality gives worse visuals but is faster. See <see cref="ImageResizeQuality"/> for possible
        /// values and the more technical internal details. The default value is <see cref="ImageResizeQuality.Medium"/>.
        /// </summary>
        public ImageResizeQuality ResizeQuality
        {
            get => _resizeQuality;
            set
            {
                SetResizeQuality(value);
                ResizeQualityProperty.Value = value;
            }
        }

        private ImageResizeQuality _resizeQuality = ImageResizeQuality.Medium;
        public ObservableProperty<ImageResizeQuality> ResizeQualityProperty { get; } = new();

        private void SetResizeQuality(ImageResizeQuality value)
        {
            _resizeQuality = value;
            RequestRedraw();
        }

        private SKImage? _cachedScaledImage;

        public ImageView()
        {
            InitPropertiesEvents();
        }

        public ImageView(Image source)
        {
            Source = source;
            InitPropertiesEvents();
        }

        private void InitPropertiesEvents()
        {
            SourceProperty.ValueChangedEvent += SetSource;
            HorizontalAlignmentProperty.ValueChangedEvent += SetHorizontalAlignment;
            VerticalAlignmentProperty.ValueChangedEvent += SetVerticalAlignment;
            ShouldKeepAspectRatioProperty.ValueChangedEvent += SetShouldKeepAspectRatio;
            ImageFitProperty.ValueChangedEvent += SetImageFit;
            ResizeQualityProperty.ValueChangedEvent += SetResizeQuality;
        }

        //~ImageView()
        //{
        //    SourceProperty = null!;
        //    HorizontalAlignmentProperty = null!;
        //    VerticalAlignmentProperty = null!;
        //    ShouldKeepAspectRatioProperty = null!;
        //    ImageFitProperty = null!;
        //    ResizeQualityProperty = null!;
        //}

        protected override void Draw(object sender)
        {
            base.Draw(sender);

            if (Source == null)
            {
                return;
            }

            //TODO: this is for tinting the image, for further use
            //

            // new SKPaint
            // {
            //     ImageFilter = SKImageFilter.CreateBlendMode(
            //         SKBlendMode.DstIn,
            //         SKImageFilter.CreateColorFilter(
            //             SKColorFilter.CreateBlendMode(
            //                 new SKColor(0xff_21_96_d5),
            //                 SKBlendMode.Darken)))
            // }

            SKImage? skiaImage = Source.SkiaImage!;
            if (skiaImage == null)
            {
                throw new NullReferenceException("Source's internal SkiaSharp image was null.");
            }

            float width = skiaImage.Width, height = skiaImage.Height;
            switch (ImageFit)
            {
                case ImageFitType.CanShrink:
                    {
                        if (ShouldKeepAspectRatio)
                        {
                            float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                            if (width > Bounds.Width)
                            {
                                width = Bounds.Width;
                                height = width * aspectRatio;
                            }

                            if (height > Bounds.Height)
                            {
                                height = Bounds.Height;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = Math.Clamp(skiaImage.Width, 0f, Bounds.Width);
                            height = Math.Clamp(skiaImage.Height, 0f, Bounds.Height);
                        }

                        break;
                    }
                case ImageFitType.CanGrow:
                    {
                        if (ShouldKeepAspectRatio)
                        {
                            float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                            if (width < Bounds.Width)
                            {
                                width = Bounds.Width;
                                height = width * aspectRatio;
                            }

                            if (height < Bounds.Height)
                            {
                                height = Bounds.Height;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = Math.Max(skiaImage.Width, Bounds.Width);
                            height = Math.Max(skiaImage.Height, Bounds.Height);
                        }

                        break;
                    }
                case ImageFitType.CanShrinkAndGrow:
                    {
                        if (ShouldKeepAspectRatio)
                        {
                            float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                            if (Bounds.Width < Bounds.Height)
                            {
                                width = Bounds.Width;
                                height = width * aspectRatio;
                            }
                            else
                            {
                                height = Bounds.Height;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = Bounds.Width;
                            height = Bounds.Height;
                        }

                        break;
                    }
                case ImageFitType.Cover:
                    {
                        //cover always obeys the aspect ratio
                        float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                        if (Bounds.Width > Bounds.Height)
                        {
                            width = Bounds.Width;
                            height = width * aspectRatio;
                        }
                        else
                        {
                            height = Bounds.Height;
                            width = height * (1f / aspectRatio);
                        }

                        break;
                    }
                case ImageFitType.None:
                default:
                    break;
            }

            float x, y;
            switch (HorizontalAlignment)
            {
                case HorizontalAlignmentType.Left:
                    x = Bounds.X;
                    break;
                case HorizontalAlignmentType.Center:
                    x = Bounds.X + ((Bounds.Width - width) / 2f);
                    break;
                case HorizontalAlignmentType.Right:
                    x = Bounds.X + (Bounds.Width - width);
                    break;
                default:
                    throw new ArgumentException("Invalid HorizontalAlignment", nameof(HorizontalAlignment));
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignmentType.Top:
                    y = Bounds.Y;
                    break;
                case VerticalAlignmentType.Center:
                    y = Bounds.Y + ((Bounds.Height - height) / 2f);
                    break;
                case VerticalAlignmentType.Bottom:
                    y = Bounds.Y + (Bounds.Height - height);
                    break;
                default:
                    throw new ArgumentException("Invalid VerticalAlignment", nameof(VerticalAlignment));
            }

            //to avoid cache misses, 0.5 is chosen as threshold
            if (_cachedScaledImage != null &&
                Math.Abs(width - _cachedScaledImage.Width) < 0.5f &&
                Math.Abs(height - _cachedScaledImage.Height) < 0.5f)
            {
                Document?.Renderer.DrawImageFast(_cachedScaledImage, new Point2D(x, y));
            }
            else
            {
                SKImage? scaledImage = null;

                Document?.Renderer.DrawImage(
                    skiaImage,
                    new Point2D(x, y),
                    new Size(width, height),
                    ResizeQuality,
                    null,
                    out scaledImage);

                _cachedScaledImage = scaledImage ?? skiaImage;
            }
        }

        public override ImageView Duplicate()
        {
            ImageView el = new()
            {
                Source = _source,
                HorizontalAlignment = _horizontalAlignment,
                VerticalAlignment = _verticalAlignment,
                ShouldKeepAspectRatio = _shouldKeepAspectRatio,
                ImageFit = _imageFit,
                ResizeQuality = _resizeQuality,
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };

            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
