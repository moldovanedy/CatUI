using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        public string Text { get; set; } = string.Empty;
        public bool WordWrap { get; set; } = true;
        public TextOverflowMode TextOverflowMode { get; set; } = TextOverflowMode.Ellipsis;
        public TextAlignmentType TextAlignment { get; set; } = TextAlignmentType.Left;

        public TextElement(string text)
        {
            Text = text;
        }

        public TextElement(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = true,

            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<TextElementThemeData>? themeOverrides = null,
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
            WordWrap = wordWrap;
            TextAlignment = textAlignment;

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides(themeOverrides);
            }

            TextOverflowMode = textOverflowMode;
        }

        #region Builder
        public TextElement SetInitialText(string text)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            Text = text;
            return this;
        }

        public TextElement SetInitialWordWrap(bool wordWrap)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            WordWrap = wordWrap;
            return this;
        }

        public TextElement SetInitialTextOverflowMode(TextOverflowMode overflowMode)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            TextOverflowMode = overflowMode;
            return this;
        }

        public TextElement SetInitialTextAlignment(TextAlignmentType textAlignment)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            TextAlignment = textAlignment;
            return this;
        }
        #endregion //Builder
    }
}
