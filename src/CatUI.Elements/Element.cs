using System;
using System.Collections.Generic;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Enums;
using CatUI.Elements.Themes;

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

        public const string STYLE_NORMAL = "normal";
        public const string STYLE_HOVER = "hover";
        private Dictionary<string, ElementThemeData> _themeOverrides = new Dictionary<string, ElementThemeData>();

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

        protected bool IsInstantiated { get; private set; } = false;
        private Element? _parent;

        /// <summary>
        /// Contains a list of all children (public and internal).
        /// </summary>
        private readonly List<Element> _children = new List<Element>();

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
            Dictionary<string, ElementThemeData>? themeOverrides = null,
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

            if (themeOverrides != null)
            {
                SetInitialThemeOverrides(themeOverrides);
            }
            if (position != null)
            {
                SetInitialPosition(position ?? new Dimension2(0, 0));
            }
            if (width != null)
            {
                SetInitialWidth(width ?? new Dimension(0));
            }
            if (height != null)
            {
                SetInitialHeight(height ?? new Dimension(0));
            }
            if (minHeight != null)
            {
                SetInitialMinHeight(minHeight ?? Dimension.Unset);
            }
            if (minWidth != null)
            {
                SetInitialMinWidth(minWidth ?? Dimension.Unset);
            }
            if (maxHeight != null)
            {
                SetInitialMaxHeight(maxHeight ?? Dimension.Unset);
            }
            if (maxWidth != null)
            {
                SetInitialMaxWidth(maxWidth ?? Dimension.Unset);
            }
            Instantiate();

            if (children != null)
            {
                AddChildren(children);
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

        /// <summary>
        /// Sets the document of this element and all its children. It should generally be called only for elements
        /// that will have children at creation time.
        /// Will not actually add the element to the document 
        /// (see <see cref="AddChild(Element, bool)"/> or <see cref="AddChildren(List{Element}, bool)"/>).
        /// </summary>
        /// <remarks>
        /// If the element already belongs to a document, this will remove the element, along with all its children, 
        /// then add this element along with its previous children to the specified document.
        /// </remarks>
        /// <param name="document">The document to which this element should be added</param>
        public Element SetDocument(UIDocument document)
        {
            if (Document != null && Document != document)
            {
                GetParent()?.RemoveChild(this);
            }

            Document = document;
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetDocument(document);
            }
            return this;
        }

        public Element SetInitialThemeOverrides<T>(Dictionary<string, T> themeOverrides) where T : ElementThemeData, new()
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            SetElementThemeOverrides(themeOverrides);
            return this;
        }

        public Element SetInitialPosition(Dimension2 position)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _position = position;
            return this;
        }

        public Element SetInitialWidth(Dimension width)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _width = width;
            return this;
        }

        public Element SetInitialHeight(Dimension height)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _height = height;
            return this;
        }

        public Element SetInitialMinWidth(Dimension minWidth)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _minWidth = minWidth;
            return this;
        }

        public Element SetInitialMinHeight(Dimension minHeight)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _minHeight = minHeight;
            return this;
        }

        public Element SetInitialMaxWidth(Dimension maxWidth)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _maxWidth = maxWidth;
            return this;
        }

        public Element SetInitialMaxHeight(Dimension maxHeight)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _maxHeight = maxHeight;
            return this;
        }

        /// <summary>
        /// This sets up the element for the eventual addition to the document tree. If the document is set, 
        /// this will automatically add the element to the document.
        /// </summary>
        /// <remarks>
        /// Always call this before setting the children. Otherwise, you will get an exception in <see cref="SetChildren(Element[])"/>.
        /// This method does not act on already added children.
        /// Calling this method more than once will not have any effect.
        /// </remarks>
        /// <returns>The element itself.</returns>
        public virtual Element Instantiate()
        {
            if (IsInstantiated)
            {
                return this;
            }

            IsInstantiated = true;
            RecalculateBounds();

            if (Document != null)
            {
                GetParent()?.AddChild(this);
            }
            return this;
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
                    _cachedPublicChildren = new List<Element>() { child };
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

        public void AddChildren(params Element[] children)
        {
            foreach (Element child in children)
            {
                AddChild(child);
            }
        }

        public void AddChildren(List<Element> children)
        {
            foreach (Element child in children)
            {
                AddChild(child);
            }
        }

        public void AddInternalChildren(params Element[] children)
        {
            foreach (Element child in children)
            {
                AddChild(child, true);
            }
        }

        public void AddInternalChildren(List<Element> children)
        {
            foreach (Element child in children)
            {
                AddChild(child);
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

            child._parent = null;
            child.Document = null;
            child.InvokeExitDocument();
            child.RemoveAllChildren();
        }

        public void RemoveAllChildren()
        {
            foreach (Element child in _children)
            {
                child.SetParent(null);
                child.Document = null;
                child.InvokeExitDocument();
                child.RemoveAllChildren();
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

        public ElementThemeData? GetElementThemeOverride(string state)
        {
            _themeOverrides.TryGetValue(state, out ElementThemeData? themeOverride);
            return themeOverride;
        }

        public T? GetElementThemeOverride<T>(string state) where T : ThemeData, new()
        {
            _themeOverrides.TryGetValue(state, out ElementThemeData? themeOverride);
            if (themeOverride is T castedTheme)
            {
                return castedTheme;
            }
            else
            {
                return null;
            }
        }

        public T GetElementThemeOverrideOrDefault<T>(string state) where T : ThemeData, new()
        {
            _themeOverrides.TryGetValue(state, out ElementThemeData? themeOverride);
            if (themeOverride is T castedTheme)
            {
                return castedTheme;
            }
            else
            {
                return new T();
            }
        }

        public void SetElementThemeOverride(string state, ElementThemeData themeOverride)
        {
            _themeOverrides[state] = themeOverride;
        }

        public void SetElementThemeOverrides(Dictionary<string, ElementThemeData> themeOverrides)
        {
            _themeOverrides = themeOverrides;
        }

        public void SetElementThemeOverrides<T>(Dictionary<string, T> themeOverrides) where T : ElementThemeData, new()
        {
            foreach (string state in themeOverrides.Keys)
            {
                if (themeOverrides.TryGetValue(state, out T? themeData))
                {
                    _themeOverrides[state] = themeData;
                }
            }
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
        public int CalculateDimension(Dimension dimension, float pixelDimensionForPercent = 0)
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
                        return (int)(dimension.Value * (Document?.ContentScale ?? 1));
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
                _themeOverrides.GetValueOrDefault(Element.STYLE_NORMAL, new ElementThemeData("")).Background
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
