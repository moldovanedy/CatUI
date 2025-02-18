using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Events.Input.Pointer;
using CatUI.RenderingEngine;
using SkiaSharp;

namespace CatUI.Elements
{
    /// <summary>
    /// Represents the root of all elements. Every window has one document, and all elements attached to the document
    /// will participate in the application lifecycle.
    /// </summary>
    public partial class UiDocument
    {
        /// <summary>
        /// Fired when the pointer (generally mouse cursor) is moved, either by the user, by the platform or by your app.
        /// The position is always relative to the top-left corner of the client area of the window (this means that
        /// any kind of window decoration will NOT be taken into account).
        /// </summary>
        /// <remarks>
        /// This will only fire while the pointer is inside the window's client area. The coordinates are in dp, not pixels.
        /// </remarks>
        public event PointerMoveEventHandler? PointerMoved;

        /// <summary>
        /// Fired when the pointer enters the window's client area. The coordinates are relative to the top-left
        /// corner of the window and might be negative, especially on fast pointer movements.
        /// </summary>
        public event PointerEnterEventHandler? PointerEnter;

        /// <summary>
        /// Fired when the pointer leaves the window's client area. The coordinates are relative to the top-left
        /// corner of the window and might be negative, especially on fast pointer movements.
        /// </summary>
        public event PointerExitEventHandler? PointerLeave;

        /// <summary>
        /// Represents the root element of the document/window. All other elements are children of this element or one of its descendants.
        /// Setting this to another element will remove the previous one with all the children (invoking ExitDocument) and 
        /// attach this element to the document, along with its children (calling EnterDocument).
        /// </summary>
        public Element? Root
        {
            get => _root;
            set
            {
                if (_root != value)
                {
                    _root?.InvokeExitDocument();
                    _root?.Children.Clear();
                }

                _root = value;
                if (_root == null)
                {
                    return;
                }

                _root.Layout.SetFixedWidth("100%").SetFixedHeight("100%");
                _root.Document = this;
                _root.Bounds = new Rect(0, 0, ViewportSize.Width, ViewportSize.Height);
            }
        }

        private Element? _root;

        public Size ViewportSize
        {
            get => _viewportSize;
            private set
            {
                _viewportSize = value;
                Renderer.SetNewSize(new SKSize(value.Width, value.Height));
                Root?.MarkLayoutDirty();
            }
        }

        private Size _viewportSize = new();

        public Renderer Renderer { get; private set; } = new();
        public int ElementCacheSize { get; set; } = 4096;

        public Color BackgroundColor
        {
            get => _background;
            set
            {
                _background = value;
                Renderer.SetBgColor(value);
            }
        }

        private Color _background = new(0xff_ff_ff);

        public float ContentScale
        {
            get => _contentScale;
            private set
            {
                _contentScale = value;
                Renderer.SetContentScale(value);
            }
        }

        private float _contentScale = 1f;

        private readonly Dictionary<string, Element> _cachedElements = new();

        public UiDocument(Size initialViewportSize = default, float initialContentScale = 1f)
        {
            ViewportSize = initialViewportSize;
            ContentScale = initialContentScale;
            Renderer.SetBgColor(_background);
        }

        public void DrawAllElements()
        {
            Root?.InvokeDraw();
        }

        public Element? GetElementByName(string name)
        {
            if (Root == null)
            {
                return null;
            }

            return _cachedElements.TryGetValue(name, out Element? element) ? element : Search(Root, name);
        }

        public void CacheElement(string name, Element element)
        {
            if (!_cachedElements.TryGetValue(name, out _))
            {
                _cachedElements.Add(name, element);

                //remove the first element from the cache, as the dictionary is full
                if (_cachedElements.Count <= ElementCacheSize)
                {
                    _cachedElements.Remove(_cachedElements.ElementAt(0).Key);
                }
            }
        }

        private Element? Search(Element current, string name)
        {
            foreach (Element child in current.Children)
            {
                if (child.Name == name)
                {
                    if (_cachedElements.Count <= ElementCacheSize)
                    {
                        CacheElement(child.Name, child);
                    }

                    return child;
                }

                Search(child, name);
            }

            return null;
        }

        //Private access for implementations of windowing. This is to avoid having public setters as it's not OK.
        //There might be better ways of doing this without reflection

        // ReSharper disable UnusedMember.Local

        #region Set by window

        /// <summary>
        /// Will be used by window implementation to set the viewport's size. Do NOT modify its signature.
        /// </summary>
        /// <param name="viewportSize">The new viewport size.</param>
        private void WndSetViewportSize(Size viewportSize)
        {
            ViewportSize = viewportSize;
        }

        /// <summary>
        /// Will be used by window implementation to set the app content scale. Do NOT modify its signature.
        /// </summary>
        /// <param name="scale">The new scale.</param>
        private void WndSetContentScale(float scale)
        {
            ContentScale = scale;
        }

        /// <summary>
        /// Called when the pointer moves.
        /// </summary>
        /// <param name="e"></param>
        private void WndCallPointerMove(PointerMoveEventArgs e)
        {
            PointerMoved?.Invoke(this, e);

            Root?.CheckInvokePointerEnter(new PointerEnterEventArgs(e.Position, e.AbsolutePosition, false));
            Root?.CheckInvokePointerExit(new PointerExitEventArgs(e.Position, e.AbsolutePosition, false));
            Root?.CheckInvokePointerMove(e);
        }

        /// <summary>
        /// Called when the pointer enters the window client area.
        /// </summary>
        /// <param name="e"></param>
        private void WndCallPointerEnter(PointerEnterEventArgs e)
        {
            PointerEnter?.Invoke(this, e);
            Root?.CheckInvokePointerEnter(e);
        }

        /// <summary>
        /// Called when the pointer leaves the window client area.
        /// </summary>
        /// <param name="e"></param>
        private void WndCallPointerLeave(PointerExitEventArgs e)
        {
            PointerLeave?.Invoke(this, e);
            Root?.CheckInvokePointerExit(e);
        }
        // ReSharper restore UnusedMember.Local

        #endregion
    }
}
