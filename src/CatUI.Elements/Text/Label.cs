using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Text
{
    public class Label : TextElement
    {
        public TextBreakMode TextBreakMode { get; set; } = TextBreakMode.SoftBreak;
        public char HyphenCharacter { get; set; } = '-';

        public Label() : base(string.Empty)
        {
            DrawEvent += PrivateDraw;
        }

        public Label(
            string text,

            TextBreakMode breakMode = TextBreakMode.SoftBreak,
            char hyphenCharacter = '-',

            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = true,

            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<LabelThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null) :
            base(text: text,
                 textAlignment: textAlignment,
                 textOverflowMode: textOverflowMode,
                 wordWrap: wordWrap,

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
            TextBreakMode = breakMode;
            HyphenCharacter = hyphenCharacter;

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides(themeOverrides);
            }
        }

        ~Label()
        {
            DrawEvent -= PrivateDraw;
        }

        #region Builder
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
        #endregion //Builder

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

            LabelThemeData currentTheme = base.GetElementFinalThemeData<LabelThemeData>(Label.STYLE_NORMAL);
            if (WordWrap)
            {
                float rowSize = CalculateDimension(currentTheme.FontSize) * currentTheme.LineHeight;
                Point2D rowPosition = base.Bounds.StartPoint;
                rowPosition.Y += rowSize / 2f;

                int charactersDrawn = 0;
                while (
                    charactersDrawn < Text.Length &&
                    //TODO: also take into account the line height and next row's vertical size on the left-hand expression
                    rowPosition.Y < base.Bounds.StartPoint.Y + base.Bounds.Width)
                {
                    int thisRowChars =
                        base.Document?.Renderer?.DrawTextRow(
                            //TODO: find ways to optimize this, preferably inside the rendering engine
                            text: Text.Substring(charactersDrawn),
                            topLeftPoint: rowPosition,
                            fontSize: currentTheme.FontSize,
                            elementSize: size,
                            fillBrush: currentTheme.FillBrush,
                            outlineBrush: currentTheme.OutlineBrush,
                            textAlignment: base.TextAlignment,
                            breakMode: TextBreakMode,
                            hyphenCharacter: HyphenCharacter) ??
                        1;
                    charactersDrawn += thisRowChars;
                    rowPosition = new Point2D(rowPosition.X, rowPosition.Y + rowSize);
                }
            }
            else
            {
                base.Document?.Renderer?.DrawTextRow(
                    text: Text,
                    topLeftPoint: this.Bounds.StartPoint,
                    fontSize: currentTheme.FontSize,
                    elementSize: size,
                    fillBrush: currentTheme.FillBrush,
                    outlineBrush: currentTheme.OutlineBrush,
                    textAlignment: base.TextAlignment,
                    overflowMode: base.TextOverflowMode);
            }
        }
    }
}
