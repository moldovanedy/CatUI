using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Themes;

namespace CatUI.Elements
{
    public partial class Element
    {
        public Action? OnDraw;
        public EnterDocumentEventHandler? OnEnterDocument;
        public ExitDocumentEventHandler? OnExitDocument;
        public LoadEventHandler? OnLoad;
        public PointerEnterEventHandler? OnPointerEnter;
        public PointerLeaveEventHandler? OnPointerExit;
        public PointerMoveEventHandler? OnPointerMove;

        public event Action? DrawEvent;
        public event EnterDocumentEventHandler? EnterDocumentEvent;
        public event ExitDocumentEventHandler? ExitDocumentEvent;
        public event LoadEventHandler? LoadEvent;
        public event PointerEnterEventHandler? PointerEnterEvent;
        public event PointerLeaveEventHandler? PointerExitEvent;
        public event PointerMoveEventHandler? PointerMoveEvent;

        public const string STYLE_NORMAL = "normal";
        public const string STYLE_HOVER = "hover";
        private readonly ThemeDefinition<ElementThemeData> _themeDefinition = new ThemeDefinition<ElementThemeData>();

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
                    PositionProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension2 _position = new Dimension2();
        public ObservableProperty<Dimension2> PositionProperty { get; } = new ObservableProperty<Dimension2>();

        /// <summary>
        /// Represents the preferred width of the element. The layout engine will try to honor this value, but this might be influenced
        /// by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion"/>) or if the element is inside a container.
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that the element will be in.
        /// </summary>
        public Dimension PreferredWidth
        {
            get
            {
                return _preferredWidth;
            }
            set
            {
                if (value != _preferredWidth)
                {
                    _preferredWidth = value;
                    PreferredWidthProperty.Value = value;
                }
            }
        }
        private Dimension _preferredWidth = new Dimension();
        public ObservableProperty<Dimension> PreferredWidthProperty { get; } = new ObservableProperty<Dimension>();

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
                    MinWidthProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension _minWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MinWidthProperty { get; } = new ObservableProperty<Dimension>();

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
                    MaxWidthProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension _maxWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MaxWidthProperty { get; } = new ObservableProperty<Dimension>();

        /// <summary>
        /// Represents the preferred height of the element. The layout engine will try to honor this value, but this might be influenced
        /// by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion"/>) or if the element is inside a container.
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that the element will be in.
        /// </summary>
        public Dimension PreferredHeight
        {
            get
            {
                return _preferredHeight;
            }
            set
            {
                if (value != _preferredHeight)
                {
                    _preferredHeight = value;
                    PreferredHeightProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension _preferredHeight = new Dimension();
        public ObservableProperty<Dimension> PreferredHeightProperty { get; } = new ObservableProperty<Dimension>();

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
                    MinHeightProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension _minHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MinHeightProperty { get; } = new ObservableProperty<Dimension>();

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
                    MaxHeightProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private Dimension _maxHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MaxHeightProperty { get; } = new ObservableProperty<Dimension>();

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
                    PaddingProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private EdgeInset _padding = new EdgeInset();
        public ObservableProperty<EdgeInset> PaddingProperty { get; } = new ObservableProperty<EdgeInset>();

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
                    MarginProperty.Value = value;
                    RecalculateLayout();
                }
            }
        }
        private EdgeInset _margin = new EdgeInset();
        public ObservableProperty<EdgeInset> MarginProperty { get; } = new ObservableProperty<EdgeInset>();

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value ?? throw new ArgumentNullException(
                    "Name",
                    "The name of an element can be empty, but not null.");
                NameProperty.Value = value;
            }
        }
        private string _name = string.Empty;
        public ObservableProperty<string> NameProperty { get; } = new ObservableProperty<string>();

        public ElementBounds Bounds { get; internal set; } = new ElementBounds();
        public bool IsInternal { get; private set; }
        public UIDocument? Document { get; private set; }

        protected bool IsInstantiated { get; private set; }

        protected Point2D InternalPosition
        {
            get
            {
                return _internalPosition;
            }
            set
            {
                if (value.X != _internalPosition.X || value.Y != _internalPosition.Y)
                {
                    _internalPosition = value;
                    RecalculateBounds();
                }
            }
        }
        private Point2D _internalPosition = Point2D.Zero;

        protected float InternalWidth
        {
            get
            {
                return _internalWidth;
            }
            set
            {
                if (value != _internalWidth)
                {
                    _internalWidth = value;
                    RecalculateBounds();
                }
            }
        }
        private float _internalWidth;

        protected float InternalHeight
        {
            get
            {
                return _internalHeight;
            }
            set
            {
                if (value != _internalHeight)
                {
                    _internalHeight = value;
                    RecalculateBounds();
                }
            }
        }
        private float _internalHeight;

        private Element? _parent;

        /// <summary>
        /// Contains a list of all children (public and internal).
        /// </summary>
        private readonly List<Element> _children = new List<Element>();

        /// <summary>
        /// It's a cache of public children only.
        /// </summary>
        private List<Element>? _cachedPublicChildren;

        public Element()
        {
            Init();
        }

        public Element(
            UIDocument? doc = null,
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
            Dimension2? position = null,
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null,
            Dimension? minHeight = null,
            Dimension? minWidth = null,
            Dimension? maxHeight = null,
            Dimension? maxWidth = null)
        {
            Init();
            Document = doc;

            if (themeOverrides != null)
            {
                SetInitialThemeOverrides(themeOverrides);
            }
            if (position != null)
            {
                SetInitialPosition(position ?? new Dimension2(0, 0));
            }
            if (preferredWidth != null)
            {
                SetInitialWidth(preferredWidth ?? new Dimension(0));
            }
            if (preferredHeight != null)
            {
                SetInitialHeight(preferredHeight ?? new Dimension(0));
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
            PointerExitEvent -= PointerLeave;
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
        /// Will also add the element to the document 
        /// (like <see cref="AddChild(Element, bool)"/> or <see cref="AddChildren(Element[])"/>).
        /// </summary>
        /// <remarks>
        /// If the element already belongs to a document, this will remove the element, along with all its children, 
        /// then add this element along with its previous children to the specified document.
        /// </remarks>
        /// <param name="document">The document to which this element should be added</param>
        public Element SetDocument(UIDocument? document)
        {
            //the element is not in a document and the given document is non-null
            if (Document == null && document != null)
            {
                InvokeEnterDocument();
            }
            //the element is in a document and the given document is another document or null
            else if (Document != document)
            {
                GetParent()?.RemoveChild(this);
                Document = document;

                if (document != null)
                {
                    GetParent()?.AddChild(this);
                }
            }

            // for (int i = 0; i < _children.Count; i++)
            // {
            //     _children[i].SetDocument(document);
            // }
            return this;
        }

        #region Builder
        public Element SetInitialThemeOverrides<T>(ThemeDefinition<T> themeOverrides) where T : ElementThemeData, new()
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

            _preferredWidth = width;
            return this;
        }

        public Element SetInitialHeight(Dimension height)
        {
            if (IsInstantiated)
            {
                throw new Exception("Element is already instantiated, use direct properties instead");
            }

            _preferredHeight = height;
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
            RecalculateLayout();

            if (Document != null)
            {
                GetParent()?.AddChild(this);
            }
            return this;
        }
        #endregion //Builder

        #region Internal invoke
        internal void InvokeDraw()
        {
            DrawEvent?.Invoke();
            DrawChildren();
        }

        internal void InvokeEnterDocument()
        {
            EnterDocumentEvent?.Invoke(this);
        }

        internal void InvokeExitDocument()
        {
            ExitDocumentEvent?.Invoke(this);
        }

        internal void InvokeLoad()
        {
            foreach (Element child in _children)
            {
                child.InvokeLoad();
            }

            LoadEvent?.Invoke(this);
        }

        internal void InvokePointerEnter()
        {
            PointerEnterEvent?.Invoke(this, new PointerEnterEventArgs(Point2D.Zero, false));
        }

        internal void InvokePointerLeave()
        {
            PointerExitEvent?.Invoke(this, new PointerLeaveEventArgs(Point2D.Zero, false));
        }

        internal void InvokePointerMove()
        {
            PointerMoveEvent?.Invoke(this, new PointerMoveEventArgs(Point2D.Zero, false));
        }
        #endregion //Internal invoke

        #region Internal event handlers
        private void PrivateDraw()
        {
            RecalculateLayout();
            DrawBackground();
        }

        protected virtual void RecalculateLayout()
        {
            float parentWidth, parentHeight, parentXPos, parentYPos;
            if (Document?.Root == this)
            {
                parentWidth = Document.ViewportSize.Width;
                parentHeight = Document.ViewportSize.Height;
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

            if (!PreferredWidth.IsUnset())
            {
                InternalWidth = Math.Clamp(
                    CalculateDimension(PreferredWidth, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }

            if (!PreferredHeight.IsUnset())
            {
                InternalHeight = Math.Clamp(
                    CalculateDimension(PreferredHeight, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }

            if (!Position.IsUnset())
            {
                InternalPosition = new Point2D(
                     parentXPos + CalculateDimension(Position.X, parentWidth),
                     parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            // Bounds = new ElementBounds(
            //     new Point2D(
            //         parentXPos + CalculateDimension(Position.X, parentWidth),
            //         parentYPos + CalculateDimension(Position.Y, parentHeight)),
            //     elementFinalWidth,
            //     elementFinalHeight,
            //     new float[4],
            //     new float[4]);
        }

        private void RecalculateBounds()
        {
            Bounds = new ElementBounds(
                startPoint: InternalPosition,
                width: InternalWidth,
                height: InternalHeight,
                paddings: new float[4],
                margins: new float[4]);
        }
        #endregion

        #region Public API
        public virtual void Draw() { }
        public virtual void EnterDocument(object sender) { }
        public virtual void ExitDocument(object sender) { }
        public virtual void Loaded(object sender) { }
        public virtual void PointerEnter(object sender, PointerEnterEventArgs e) { }
        public virtual void PointerLeave(object sender, PointerLeaveEventArgs e) { }
        public virtual void PointerMove(object sender, PointerMoveEventArgs e) { }

        public void AddChild(Element child, bool isInternal = false)
        {
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

            child._parent = this;
            if (Document != null)
            {
                child.Document = Document;
                child.InvokeEnterDocument();

                foreach (Element grandChild in child._children)
                {
                    grandChild.InvokeEnterDocument();
                }
            }
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
                AddChild(child, true);
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
                //clear cache, as it's too difficult to determine (very fast) where to insert the element
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

            if (Document != null)
            {
                child.InvokeExitDocument();
            }
            child.RemoveAllChildren();
        }

        public void RemoveAllChildren()
        {
            while (_children.Count != 0)
            {
                RemoveChild(_children[0]);
            }

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
            return _themeDefinition.GetThemeDataForState(state);
        }

        public T? GetElementThemeOverride<T>(string state) where T : ElementThemeData, new()
        {
            return (T?)_themeDefinition.GetThemeDataForState(state);
        }

        /// <summary>
        /// Returns the element's ThemeData that should be applied and all styling should respect this theme.
        /// If there is no Theme in the document tree hierarchy (not even at root level), this will return 
        /// the default ThemeData of the specified type parameter.
        /// </summary>
        /// <typeparam name="T">The ThemeData type of the element.</typeparam>
        /// <param name="state">The state for which the styling is applied.</param>
        /// <returns>The ThemeData that should be respected by the element at the given state.</returns>
        public T GetElementFinalThemeData<T>(string state) where T : ElementThemeData, new()
        {
            T? themeData = (T?)_themeDefinition.GetThemeDataForState(state);
            if (themeData is T castedTheme)
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
            _themeDefinition.SetThemeDataForState(state, themeOverride);
        }

        public void SetElementThemeOverrides<T>(ThemeDefinition<T> themeOverrides) where T : ElementThemeData, new()
        {
            foreach (string state in themeOverrides.GetStates())
            {
                _themeDefinition.SetThemeDataForState(state, themeOverrides.GetThemeDataForState(state)!);
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
        protected virtual void DrawBackground()
        {
            IBrush fillBrush = GetElementFinalThemeData<ElementThemeData>(Element.STYLE_NORMAL).Background;

            if (!fillBrush.IsSkippable)
            {
                Document?.Renderer?.DrawRect(Bounds.GetPaddingBox(), fillBrush);
            }
        }
        #endregion //Visual

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
            PointerExitEvent += PointerLeave;
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
