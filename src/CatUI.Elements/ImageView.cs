using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Elements.Themes;

namespace CatUI.Elements
{
    public class ImageView : Element
    {
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

        public ImageView(
            Image source,
            ThemeDefinition<ImageViewThemeData>? themeOverrides = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
            : base(
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight)
        {
            Source = source;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
        }

        public override void Draw()
        {
            base.DrawBackground();

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

            Document?.Renderer.DrawImageFast(Source.SkiaImage!, AbsolutePosition);
        }
    }
}
