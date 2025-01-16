using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Containers;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
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
            //Element
            string name = "",
            List<Element>? children = null,
            ThemeDefinition<ImageViewThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null,
            ContainerSizing? elementContainerSizing = null,
            bool visible = true,
            bool enabled = true,
            //Element actions
            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :

            //ReSharper disable ArgumentsStyleNamedExpression
            base(
                name: name,
                children: children,
                position: position,
                preferredWidth: preferredWidth,
                preferredHeight: preferredHeight,
                minHeight: minHeight,
                minWidth: minWidth,
                maxHeight: maxHeight,
                maxWidth: maxWidth,
                elementContainerSizing: elementContainerSizing,
                visible: visible,
                enabled: enabled,
                //
                onDraw: onDraw,
                onEnterDocument: onEnterDocument,
                onExitDocument: onExitDocument,
                onLoad: onLoad,
                onPointerEnter: onPointerEnter,
                onPointerLeave: onPointerLeave,
                onPointerMove: onPointerMove)
        //ReSharper enable ArgumentsStyleNamedExpression
        {
            DrawEvent += PrivateDrawImage;

            Source = source;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
        }

        ~ImageView()
        {
            DrawEvent -= PrivateDrawImage;
        }

        private void PrivateDrawImage()
        {
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
