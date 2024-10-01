using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Styles;

namespace CatUI.Elements.Shapes
{
    public class Rectangle : Element
    {
        //public IBrush Background { get; set; } = new ColorBrush();

        public Rectangle() { }

        public Rectangle(
            IBrush rectBrush,
            UIDocument? doc = null,
            List<Element>? children = null,
            ElementStyle? style = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(doc: doc,
                 children: children,
                 style: style,
                 position: position,
                 width: width,
                 height: height,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
        {
            base.Style.Background = rectBrush;
        }

        //public override void Draw()
        //{
        //    //if (Background is ColorBrush colorBrush)
        //    //{
        //    //    SKPaint paint = new SKPaint()
        //    //    {
        //    //        Color = colorBrush.Color,
        //    //    };
        //    //    Document?.Renderer?.Canvas?.DrawRect(
        //    //        Position.X.Value,
        //    //        Position.Y.Value,
        //    //        Width.Value,
        //    //        Height.Value, paint);
        //    //}
        //}
    }
}
