using System;
using System.Collections.Generic;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        public string Text { get; set; } = string.Empty;
        public bool WordWrap { get; set; } = true;
        public TextOverflowMode TextOverflowMode { get; set; } = TextOverflowMode.Ellipsis;

        public TextElement(string text)
        {
            Text = text;
        }

        public TextElement(
            string text,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = true,
            UIDocument? doc = null,
            List<Element>? children = null,
            Dictionary<string, TextElementThemeData>? themeOverrides = null,
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

            if (themeOverrides != null)
            {
                base.SetElementThemeOverrides<TextElementThemeData>(themeOverrides);
            }

            TextOverflowMode = textOverflowMode;
        }

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
    }
}
