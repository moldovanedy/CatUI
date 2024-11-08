using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Themes.Text;

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
            HorizontalAlignmentType horizontalAlignment = HorizontalAlignmentType.Left,
            VerticalAlignmentType verticalAlignment = VerticalAlignmentType.Top,
            bool wordWrap = true,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            UIDocument? doc = null,
            List<Element>? children = null,
            Dictionary<string, LabelThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(text: text,
                 wordWrap: wordWrap,
                 textOverflowMode: textOverflowMode,
                 doc: doc,
                 children: children,
                 position: position,
                 width: width,
                 height: height,
                 minHeight: minHeight,
                 minWidth: minWidth,
                 maxHeight: maxHeight,
                 maxWidth: maxWidth)
        {
            DrawEvent += PrivateDraw;

            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides<LabelThemeData>(themeOverrides);
            }
        }

        ~Label()
        {
            DrawEvent -= PrivateDraw;
        }

        public Label SetInitialHorizontalAlignment(HorizontalAlignmentType horizontalAlignment)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            HorizontalAlignment = horizontalAlignment;
            return this;
        }

        public Label SetInitialVerticalAlignment(VerticalAlignmentType verticalAlignment)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            VerticalAlignment = verticalAlignment;
            return this;
        }

        public Label SetInitialTextBreakMode(TextBreakMode textBreakMode)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            TextBreakMode = textBreakMode;
            return this;
        }

        public Label SetInitialHyphenCharacter(char hyphenCharacter)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            HyphenCharacter = hyphenCharacter;
            return this;
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

            LabelThemeData currentTheme = base.GetElementThemeOverrideOrDefault<LabelThemeData>(Label.STYLE_NORMAL);
            if (WordWrap)
            {
                Point2D rowPosition = this.Bounds.StartPoint;
                int charactersDrawn = 0;
                while (charactersDrawn < Text.Length)
                {

                    int thisRowChars =
                        base.Document?.Renderer?.DrawTextRow(
                            //TODO: find ways to optimize this, preferably inside the rendering engine
                            Text.Substring(charactersDrawn),
                            rowPosition,
                            currentTheme.FontSize,
                            size,
                            currentTheme.TextColor,
                            HorizontalAlignment,
                            TextBreakMode,
                            HyphenCharacter) ??
                        1;
                    charactersDrawn += thisRowChars;
                    rowPosition = new Point2D(
                        rowPosition.X,
                        rowPosition.Y +
                        CalculateDimension(currentTheme.FontSize));
                }
            }
            else
            {
                base.Document?.Renderer?.DrawTextRow(
                    text: Text,
                    topLeftPoint: this.Bounds.StartPoint,
                    fontSize: currentTheme.FontSize,
                    elementSize: size,
                    color: base.GetElementThemeOverrideOrDefault<LabelThemeData>(Label.STYLE_NORMAL).TextColor,
                    horizontalAlignment: HorizontalAlignment,
                    overflowMode: base.TextOverflowMode);
            }
        }
    }
}
