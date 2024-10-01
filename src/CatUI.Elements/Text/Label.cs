using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Styles;

namespace CatUI.Elements.Text
{
    public class Label : TextElement
    {
        public HorizontalAlignmentType HorizontalAlignment { get; set; } = HorizontalAlignmentType.Left;
        public VerticalAlignmentType VerticalAlignment { get; set; } = VerticalAlignmentType.Top;
        public TextBreakMode TextBreakMode { get; set; } = TextBreakMode.SoftBreak;
        public char HyphenCharacter { get; set; } = '-';

        public Label() : base(string.Empty)
        {
            DrawEvent += PrivateDraw;
        }
        public Label(
            string text,
            Dimension fontSize,
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Left,
            VerticalAlignmentType verticalAlignment = VerticalAlignmentType.Top,
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
            base(text: text,
                 doc: doc,
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
            DrawEvent += PrivateDraw;

            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
        }

        ~Label()
        {
            DrawEvent -= PrivateDraw;
        }

        private void PrivateDraw()
        {
            Element? parent = GetParent();
            float parentXPos, parentYPos, parentWidth, parentHeight;
            if (base.Document?.Root == this)
            {
                parentWidth = base.Document.ViewportSize.Width;
                parentHeight = base.Document.ViewportSize.Height;
                parentXPos = 0;
                parentYPos = 0;
            }
            else
            {
                parentWidth = parent?.Bounds.Width ?? 0;
                parentHeight = parent?.Bounds.Height ?? 0;
                parentXPos = parent?.Bounds.StartPoint.X ?? 0;
                parentYPos = parent?.Bounds.StartPoint.Y ?? 0;
            }

            float elementFinalWidth = CalculateDimension(Width, parentWidth);
            float elementFinalHeight = CalculateDimension(Height, parentHeight);
            Size size = new Size(elementFinalWidth, elementFinalHeight);

            base.Bounds = new ElementBounds(
                new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight)),
                elementFinalWidth,
                elementFinalHeight,
                new float[4],
                new float[4]);

            Point2D rowPosition = this.Bounds.StartPoint;
            int charactersDrawn = 0;
            while (charactersDrawn < Text.Length)
            {
                int thisRowChars =
                    base.Document?.Renderer?.DrawTextRow(
                        //TODO: find ways to optimize this
                        Text.Substring(charactersDrawn),
                        rowPosition,
                        FontSize,
                        size,
                        base.Style.TextColor,
                        HorizontalAlignment,
                        TextBreakMode,
                        HyphenCharacter) ??
                    1;
                charactersDrawn += thisRowChars;
                rowPosition = new Point2D(
                    rowPosition.X,
                    rowPosition.Y +
                    (this.Document?.Renderer?.CalculateDimension(FontSize) ?? 0));
            }
        }
    }
}
