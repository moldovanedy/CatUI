using System;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Enums;
using SkiaSharp;

namespace CatUI.Elements
{
    public class ImageView : Element
    {
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
                _source = value;
                SourceProperty.Value = value;
            }
        }

        private Image? _source;
        public ObservableProperty<Image> SourceProperty { get; } = new();

        /// <summary>
        /// Specifies the image's horizontal alignment (on the X axis). <see cref="HorizontalAlignmentType.Stretch"/> is
        /// invalid here and will be treated as <see cref="HorizontalAlignmentType.Left"/>. The default value is
        /// <see cref="HorizontalAlignmentType.Left"/>.
        /// </summary>
        public HorizontalAlignmentType HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                HorizontalAlignmentProperty.Value = value;
            }
        }

        private HorizontalAlignmentType _horizontalAlignment = HorizontalAlignmentType.Left;

        public ObservableProperty<HorizontalAlignmentType> HorizontalAlignmentProperty { get; }
            = new(HorizontalAlignmentType.Left);

        /// <summary>
        /// Specifies the image's vertical alignment (on the Y axis). <see cref="VerticalAlignmentType.Stretch"/> is
        /// invalid here and will be treated as <see cref="VerticalAlignmentType.Top"/>. The default value is
        /// <see cref="VerticalAlignmentType.Top"/>.
        /// </summary>
        public VerticalAlignmentType VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                _verticalAlignment = value;
                VerticalAlignmentProperty.Value = value;
            }
        }

        private VerticalAlignmentType _verticalAlignment = VerticalAlignmentType.Top;

        public ObservableProperty<VerticalAlignmentType> VerticalAlignmentProperty { get; }
            = new(VerticalAlignmentType.Top);

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
                _shouldKeepAspectRatio = value;
                ShouldKeepAspectRatioProperty.Value = value;
            }
        }

        private bool _shouldKeepAspectRatio = true;
        public ObservableProperty<bool> ShouldKeepAspectRatioProperty { get; } = new(true);

        /// <summary>
        /// Specifies how will the image fit inside the element's space. The default value is
        /// <see cref="ImageFitType.CanShrink"/>.
        /// </summary>
        public ImageFitType ImageFit
        {
            get => _imageFit;
            set
            {
                _imageFit = value;
                ImageFitProperty.Value = value;
            }
        }

        private ImageFitType _imageFit = ImageFitType.CanShrink;
        public ObservableProperty<ImageFitType> ImageFitProperty { get; } = new(ImageFitType.CanShrink);

        /// <summary>
        /// Specifies the quality of the drawn image after it has been resized (or simply the quality of the resizing
        /// algorithm). Lower quality gives worse visuals but is faster. See <see cref="ImageResizeQuality"/> for possible
        /// values and the more technical internal details. The default value is <see cref="ImageResizeQuality.Medium"/>.
        /// </summary>
        public ImageResizeQuality ResizeQuality
        {
            get => _imageResizeQuality;
            set
            {
                _imageResizeQuality = value;
                ImageResizeQualityProperty.Value = value;
            }
        }

        private ImageResizeQuality _imageResizeQuality = ImageResizeQuality.Medium;
        public ObservableProperty<ImageResizeQuality> ImageResizeQualityProperty { get; } = new();

        private SKImage? _cachedScaledImage;

        public ImageView(
            Image source,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth,
                preferredHeight)
        {
            Source = source;
        }

        public override void Draw()
        {
            base.Draw();

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

                            if (width > AbsoluteWidth)
                            {
                                width = AbsoluteWidth;
                                height = width * aspectRatio;
                            }

                            if (height > AbsoluteHeight)
                            {
                                height = AbsoluteHeight;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = Math.Clamp(skiaImage.Width, 0f, AbsoluteWidth);
                            height = Math.Clamp(skiaImage.Height, 0f, AbsoluteHeight);
                        }

                        break;
                    }
                case ImageFitType.CanGrow:
                    {
                        if (ShouldKeepAspectRatio)
                        {
                            float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                            if (width < AbsoluteWidth)
                            {
                                width = AbsoluteWidth;
                                height = width * aspectRatio;
                            }

                            if (height < AbsoluteHeight)
                            {
                                height = AbsoluteHeight;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = Math.Max(skiaImage.Width, AbsoluteWidth);
                            height = Math.Max(skiaImage.Height, AbsoluteHeight);
                        }

                        break;
                    }
                case ImageFitType.CanShrinkAndGrow:
                    {
                        if (ShouldKeepAspectRatio)
                        {
                            float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                            if (AbsoluteWidth < AbsoluteHeight)
                            {
                                width = AbsoluteWidth;
                                height = width * aspectRatio;
                            }
                            else
                            {
                                height = AbsoluteHeight;
                                width = height * (1f / aspectRatio);
                            }
                        }
                        else
                        {
                            width = AbsoluteWidth;
                            height = AbsoluteHeight;
                        }

                        break;
                    }
                case ImageFitType.Cover:
                    {
                        //cover always obeys the aspect ratio
                        float aspectRatio = skiaImage.Width / (float)skiaImage.Height;

                        if (AbsoluteWidth > AbsoluteHeight)
                        {
                            width = AbsoluteWidth;
                            height = width * aspectRatio;
                        }
                        else
                        {
                            height = AbsoluteHeight;
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
                case HorizontalAlignmentType.Stretch:
                case HorizontalAlignmentType.Left:
                    x = AbsolutePosition.X;
                    break;
                case HorizontalAlignmentType.Center:
                    x = AbsolutePosition.X + ((AbsoluteWidth - width) / 2f);
                    break;
                case HorizontalAlignmentType.Right:
                    x = AbsolutePosition.X + (AbsoluteWidth - width);
                    break;
                default:
                    throw new ArgumentException("Invalid HorizontalAlignment", nameof(HorizontalAlignment));
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignmentType.Stretch:
                case VerticalAlignmentType.Top:
                    y = AbsolutePosition.Y;
                    break;
                case VerticalAlignmentType.Center:
                    y = AbsolutePosition.Y + ((AbsoluteHeight - height) / 2f);
                    break;
                case VerticalAlignmentType.Bottom:
                    y = AbsolutePosition.Y + (AbsoluteHeight - height);
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
    }
}
