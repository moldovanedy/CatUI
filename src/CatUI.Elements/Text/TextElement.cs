using System;
using System.Collections.Generic;

using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;
using CatUI.Elements.Themes.Text;

namespace CatUI.Elements.Text
{
    public abstract class TextElement : Element
    {
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                TextProperty.Value = value;
            }
        }
        private string _text = string.Empty;
        public ObservableProperty<string> TextProperty { get; } = new ObservableProperty<string>();

        public bool WordWrap
        {
            get
            {
                return _wordWrap;
            }
            set
            {
                _wordWrap = value;
                WordWrapProperty.Value = value;
            }
        }
        private bool _wordWrap;
        public ObservableProperty<bool> WordWrapProperty { get; } = new ObservableProperty<bool>();

        public TextOverflowMode TextOverflowMode
        {
            get
            {
                return _textOverflowMode;
            }
            set
            {
                _textOverflowMode = value;
                TextOverflowModeProperty.Value = value;
            }
        }
        private TextOverflowMode _textOverflowMode = TextOverflowMode.Ellipsis;
        public ObservableProperty<TextOverflowMode> TextOverflowModeProperty { get; }
           = new ObservableProperty<TextOverflowMode>();

        public TextAlignmentType TextAlignment
        {
            get
            {
                return _textAlignmentType;
            }
            set
            {
                _textAlignmentType = value;
                TextAlignmentProperty.Value = value;
            }
        }
        private TextAlignmentType _textAlignmentType = TextAlignmentType.Left;
        public ObservableProperty<TextAlignmentType> TextAlignmentProperty { get; }
            = new ObservableProperty<TextAlignmentType>();

        public string EllipsisString
        {
            get
            {
                return _ellipsisString;
            }
            set
            {
                _ellipsisString = value;
                EllipsisStringProperty.Value = value;
            }
        }
        private string _ellipsisString = "\u2026";
        public ObservableProperty<string> EllipsisStringProperty { get; } = new ObservableProperty<string>();

        /// <summary>
        /// If true, the element will expand beyond the set <see cref="Element.PreferredWidth"/> and <see cref="Element.PreferredHeight"/> without
        /// respecting the maximum width and height constraints (like <see cref="Element.MaxHeight"/>).
        /// </summary>
        /// <remarks>
        /// This expansion will generally happen:
        /// <list type="bullet">
        /// <item>when <see cref="WordWrap"/> is false</item>
        /// <item>when a word's width is larger than the element's width</item>
        /// <item>when the text is so large that it won't fit in the element's height (as it occupies more rows)</item>
        /// </list>
        /// <para>
        /// What exactly happens when this property is false and the text is larger than the element's size depends 
        /// on each element that derives from this class. For example, <see cref="Label"/> will have the 
        /// <see cref="EllipsisString"/> at the end of the last row.
        /// </para>
        /// </remarks>
        public bool AllowsExpansion
        {
            get
            {
                return _allowsExpansion;
            }
            set
            {
                _allowsExpansion = value;
                AllowsExpansionProperty.Value = value;
            }
        }
        private bool _allowsExpansion;
        public ObservableProperty<bool> AllowsExpansionProperty { get; } = new ObservableProperty<bool>();

        public TextElement(
            string text,
            TextAlignmentType textAlignment = TextAlignmentType.Left,
            TextOverflowMode textOverflowMode = TextOverflowMode.Ellipsis,
            bool wordWrap = false,
            bool allowsExpansion = true,

            List<Element>? children = null,
            ThemeDefinition<TextElementThemeData>? themeOverrides = null,
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

            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null) :
            base(children: children,
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

                 onDraw: onDraw,
                 onEnterDocument: onEnterDocument,
                 onExitDocument: onExitDocument,
                 onLoad: onLoad,
                 onPointerEnter: onPointerEnter,
                 onPointerLeave: onPointerLeave,
                 onPointerMove: onPointerMove)
        {
            Text = text;
            TextAlignment = textAlignment;
            TextOverflowMode = textOverflowMode;
            WordWrap = wordWrap;
            AllowsExpansion = allowsExpansion;

            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }
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

        public TextElement SetInitialTextAlignment(TextAlignmentType textAlignment)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            TextAlignment = textAlignment;
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

        public TextElement SetInitialWordWrap(bool wordWrap)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            WordWrap = wordWrap;
            return this;
        }

        public TextElement SetInitialAllowsExpansion(bool allowsExpansion)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            AllowsExpansion = allowsExpansion;
            return this;
        }
        #endregion //Builder
    }
}
