using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Styles;

namespace CatUI.Elements
{
    public class Element
    {
        public Action? OnDraw;
        public Action? OnEnterDocument;
        public Action? OnExitDocument;
        public Action? OnLoad;
        public Action? OnPointerEnter;
        public Action? OnPointerExit;
        public Action? OnPointerMove;

        public event Action? DrawEvent;
        public event Action? EnterDocumentEvent;
        public event Action? ExitDocumentEvent;
        public event Action? LoadEvent;
        public event Action? PointerEnterEvent;
        public event Action? PointerExitEvent;
        public event Action? PointerMoveEvent;

        public ElementStyle Style { get; set; } = new ElementStyle();
        public Dimension2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension2 _position = new Dimension2();

        public Dimension Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _width = new Dimension();

        /// <summary>
        /// Represents the minimum width that the element can have.
        /// By default it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                if (value != _minWidth)
                {
                    _minWidth = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _minWidth = Dimension.Unset;

        /// <summary>
        /// Represents the maximum width that the element can have.
        /// By default it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MaxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                if (value != _maxWidth)
                {
                    _maxWidth = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _maxWidth = Dimension.Unset;

        public Dimension Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _height = new Dimension();

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                if (value != _minHeight)
                {
                    _minHeight = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _minHeight = Dimension.Unset;

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MaxHeight
        {
            get
            {
                return _maxHeight;
            }
            set
            {
                if (value != _maxHeight)
                {
                    _maxHeight = value;
                    RecalculateBounds();
                }
            }
        }
        private Dimension _maxHeight = Dimension.Unset;

        public EdgeInset Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                if (value != _padding)
                {
                    _padding = value;
                    RecalculateBounds();
                }
            }
        }
        private EdgeInset _padding = new EdgeInset();

        public EdgeInset Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                if (value != _margin)
                {
                    _margin = value;
                    RecalculateBounds();
                }
            }
        }
        private EdgeInset _margin = new EdgeInset();
        public string Name { get; set; } = string.Empty;

        public ElementBounds Bounds { get; internal set; } = new ElementBounds();
        public bool IsInternal { get; private set; } = false;
        public UIDocument? Document { get; private set; }

        private Element? _parent;

        /// <summary>
        /// Contains a list of all children (public and internal).
        /// </summary>
#if NET8_0_OR_GREATER
        private readonly List<Element> _children = [];
#else
        private readonly List<Element> _children = new List<Element>();
#endif

        /// <summary>
        /// It's a cache of public children only.
        /// </summary>
        private List<Element>? _cachedPublicChildren = null;

        public Element()
        {
            Init();
        }

        public Element(
            UIDocument? doc = null,
            List<Element>? children = null,
            ElementStyle? style = null,
            Dimension2? position = null,
            Dimension? width = null,
            Dimension? height = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null)
        {
            Init();
            this.Document = doc;

            if (style != null)
            {
                this.Style = style;
            }
            else
            {
                this.Style = new ElementStyle();
            }

            Position = position ?? new Dimension2(0, 0);
            Width = width ?? new Dimension(0);
            Height = height ?? new Dimension(0);
            MinHeight = minHeight ?? Dimension.Unset;
            MinWidth = minWidth ?? Dimension.Unset;
            MaxHeight = maxHeight ?? Dimension.Unset;
            MaxWidth = maxWidth ?? Dimension.Unset;

            if (children != null)
            {
                foreach (Element child in children)
                {
                    AddChild(child);
                }
            }
        }

        ~Element()
        {
            DrawEvent -= OnDraw;
            DrawEvent -= Draw;
            EnterDocumentEvent -= OnEnterDocument;
            EnterDocumentEvent -= EnterDocument;
            ExitDocumentEvent -= OnExitDocument;
            ExitDocumentEvent -= ExitDocument;
            LoadEvent -= OnLoad;
            LoadEvent -= Loaded;
            PointerEnterEvent -= OnPointerEnter;
            PointerEnterEvent -= PointerEnter;
            PointerExitEvent -= OnPointerExit;
            PointerExitEvent -= PointerExit;
            PointerMoveEvent -= OnPointerMove;
            PointerMoveEvent -= PointerMove;

            DrawEvent -= PrivateDraw;

            _children.Clear();
            _cachedPublicChildren = null;

            Document = null;
            _parent = null;
        }

        #region Internal invoke
        internal void InvokeDraw()
        {
            DrawEvent?.Invoke();
            DrawChildren();
        }

        internal void InvokeEnterDocument()
        {
            EnterDocumentEvent?.Invoke();
        }

        internal void InvokeExitDocument()
        {
            ExitDocumentEvent?.Invoke();
        }

        internal void InvokeLoad()
        {
            foreach (Element child in _children)
            {
                child.InvokeLoad();
            }

            LoadEvent?.Invoke();
        }

        internal void InvokePointerEnter()
        {
            PointerEnterEvent?.Invoke();
        }

        internal void InvokePointerExit()
        {
            PointerExitEvent?.Invoke();
        }

        internal void InvokePointerMove()
        {
            PointerMoveEvent?.Invoke();
        }
        #endregion //Internal invoke

        #region Internal event handlers
        private void PrivateDraw()
        {
            RecalculateBounds();
            DrawBackground();
        }

        private void RecalculateBounds()
        {
            float parentWidth, parentHeight, parentXPos, parentYPos;
            if (this.Document?.Root == this)
            {
                parentWidth = this.Document.ViewportSize.Width;
                parentHeight = this.Document.ViewportSize.Height;
                parentXPos = 0;
                parentYPos = 0;
            }
            else
            {
                parentWidth = _parent?.Bounds.Width ?? 0;
                parentHeight = _parent?.Bounds.Height ?? 0;
                parentXPos = _parent?.Bounds.StartPoint.X ?? 0;
                parentYPos = _parent?.Bounds.StartPoint.Y ?? 0;
            }

            float elementFinalWidth = Math.Clamp(
                CalculateDimension(Width, parentWidth),
                MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            float elementFinalHeight = Math.Clamp(
                CalculateDimension(Height, parentHeight),
                MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));

            this.Bounds = new ElementBounds(
                new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight)),
                elementFinalWidth,
                elementFinalHeight,
                new float[4],
                new float[4]);
        }
        #endregion

        #region Public API
        public virtual void Draw() { }
        public virtual void EnterDocument() { }
        public virtual void ExitDocument() { }
        public virtual void Loaded() { }
        public virtual void PointerEnter() { }
        public virtual void PointerExit() { }
        public virtual void PointerMove() { }

        public void AddChild(Element child, bool isInternal = false)
        {
            if (this.Document == null)
            {
                throw new Exception("The element is not inside the logical tree.");
            }

            _children.Add(child);
            child.IsInternal = isInternal;
            if (!isInternal)
            {
                if (_cachedPublicChildren == null)
                {
#if NET8_0_OR_GREATER
                    _cachedPublicChildren = [child];
#else
                    _cachedPublicChildren = new List<Element>() { child };
#endif
                }
                else
                {
                    _cachedPublicChildren.Add(child);
                }
            }

            child.SetParent(this);
            child.Document = this.Document;
            child.InvokeEnterDocument();
        }

        public void AddChildren(List<Element> children, bool areInternal = false)
        {
            foreach (Element child in children)
            {
                AddChild(child, areInternal);
            }
        }

        public List<Element> GetChildren(bool includeInternal = false)
        {
            if (includeInternal)
            {
                return _children;
            }
            else
            {
                if (_cachedPublicChildren == null)
                {
                    _cachedPublicChildren = _children.Where((child) => !child.IsInternal).ToList();
                }
                return _cachedPublicChildren;
            }
        }

        public Element GetChild(int index, bool includeInternal = false)
        {
            if (includeInternal)
            {
                if (index < 0)
                {
                    return _children[^index];
                }
                else
                {
                    return _children[index];
                }
            }
            else
            {
                EnsureChildrenCache();
                if (index < 0)
                {
                    return _cachedPublicChildren![^index];
                }
                else
                {
                    return _cachedPublicChildren![index];
                }
            }
        }

        public bool MoveChild(Element child, int toIndex, bool includeInternal = false)
        {
            if (includeInternal)
            {
                //clear cache, as it's too difficult to determine where to insert the element
                _cachedPublicChildren = null;

                if (toIndex > _children.Count || toIndex < -_children.Count)
                {
                    return false;
                }

                bool removed = _children.Remove(child);
                if (!removed)
                {
                    return false;
                }

                if (toIndex > 0)
                {
                    _children.Insert(toIndex, child);
                }
                else
                {
                    _children.Insert(_children.Count - toIndex, child);
                }
            }
            else
            {
                EnsureChildrenCache();

                if (toIndex > _cachedPublicChildren!.Count || toIndex < -_cachedPublicChildren.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public void RemoveChild(Element child)
        {
            _children.Remove(child);
            if (child.IsInternal)
            {
                _cachedPublicChildren?.Remove(child);
            }

            child.SetParent(null);
            child.Document = null;
            child.InvokeExitDocument();
        }

        public void RemoveAllChildren()
        {
            foreach (Element child in _children)
            {
                child.SetParent(null);
                child.Document = null;
                child.InvokeExitDocument();
            }
            _children.Clear();
            _cachedPublicChildren = null;
        }

        public Element? GetParent()
        {
            return _parent;
        }

        public bool IsInsideDocument()
        {
            return Document != null;
        }

        /// <summary>
        /// Will return the actual pixel value of the given dimension.
        /// </summary>
        /// <param name="dimension">The dimension to get the pixel value from.</param>
        /// <param name="pixelDimensionForPercent">
        /// Only applicable when dimension is in percentage, represents the dimension at 100%,
        /// usually set as the parent's width or height.
        /// </param>
        /// <remarks>
        /// If dimension is unset, this method returns 0.
        /// </remarks>
        /// <returns>The pixel value of the given dimension.</returns>
        public float CalculateDimension(Dimension dimension, float pixelDimensionForPercent = 0)
        {
            if (dimension.IsUnset())
            {
                return 0;
            }

            switch (dimension.MeasuringUnit)
            {
                default:
                case Unit.Dp:
                    {
                        //TODO: get the screen DPI
                        return (int)dimension.Value;
                    }
                case Unit.Pixels:
                    {
                        return (int)dimension.Value;
                    }
                case Unit.Percent:
                    {
                        if (pixelDimensionForPercent == 0)
                        {
                            return 0;
                        }
                        return (int)(dimension.Value * pixelDimensionForPercent / 100f);
                    }
                case Unit.ViewportWidth:
                    {
                        if (Document == null)
                        {
                            return 0;
                        }
                        return (int)(dimension.Value * Document.ViewportSize.Width / 100f);
                    }
                case Unit.ViewportHeight:
                    {
                        if (Document == null)
                        {
                            return 0;
                        }
                        return (int)(dimension.Value * Document.ViewportSize.Height / 100f);
                    }
            }
        }
        #endregion //Public API

        #region Visual
        private void DrawBackground()
        {
            this.Document?.Renderer?.DrawRect(
                this.Bounds.GetContentBox(),
                Style.Background
                );
        }
        #endregion //Visual

        internal void SetParent(Element? parent)
        {
            if (parent == null)
            {
                _parent?.RemoveChild(this);
            }

            _parent = parent;
        }

        /// <summary>
        /// Sets the document of this element and all its children.
        /// Should only be called from <see cref="UIDocument"/> when <see cref="UIDocument.Root"/> is changed.
        /// </summary>
        /// <param name="document"></param>
        internal void SetDocument(UIDocument document)
        {
            Document = document;

            foreach (Element child in _children)
            {
                child.SetDocument(document);
            }
        }

        private void DrawChildren()
        {
            foreach (Element child in _children)
            {
                child.InvokeDraw();
            }
        }

        private void Init()
        {
            DrawEvent += OnDraw;
            DrawEvent += Draw;
            EnterDocumentEvent += OnEnterDocument;
            EnterDocumentEvent += EnterDocument;
            ExitDocumentEvent += OnExitDocument;
            ExitDocumentEvent += ExitDocument;
            LoadEvent += OnLoad;
            LoadEvent += Loaded;
            PointerEnterEvent += OnPointerEnter;
            PointerEnterEvent += PointerEnter;
            PointerExitEvent += OnPointerExit;
            PointerExitEvent += PointerExit;
            PointerMoveEvent += OnPointerMove;
            PointerMoveEvent += PointerMove;

            DrawEvent += PrivateDraw;
        }

        private void EnsureChildrenCache()
        {
            if (_cachedPublicChildren == null)
            {
                _cachedPublicChildren = GetChildren();
            }
        }
    }
}
