using System.Collections.Generic;
using CatUI.Data;
using CatUI.Elements.Styles;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        public new TextElementStyle Style { get; set; } = new TextElementStyle();
        public string Text { get; set; } = string.Empty;
        public Dimension FontSize { get; set; } = new Dimension(16);

        public TextElement(
            string text,
            Dimension fontSize = default(Dimension),
            UIDocument? doc = null,
            List<Element>? children = null,
            TextElementStyle? style = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(doc: doc,
                 children: children,
                 position: position,
                 width: width,
                 height: height,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
        {
            Text = text;
            if (fontSize.IsUnset())
            {
                fontSize = new Dimension(16);
            }
            FontSize = fontSize;

            if (style != null)
            {
                Style = style;
            }
            else
            {
                Style = new TextElementStyle();
            }
        }
    }
}
