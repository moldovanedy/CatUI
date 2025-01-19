using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Elements.Containers;
using CatUI.Elements.Themes;

namespace CatUI.Elements
{
    public partial class Element
    {
        public readonly Action? OnDraw;
        public readonly EnterDocumentEventHandler? OnEnterDocument;
        public readonly ExitDocumentEventHandler? OnExitDocument;
        public readonly LoadEventHandler? OnLoad;
        public readonly PointerEnterEventHandler? OnPointerEnter;
        public readonly PointerLeaveEventHandler? OnPointerLeave;
        public readonly PointerMoveEventHandler? OnPointerMove;

        /// <summary>
        /// Contains a list of all children (public and internal).
        /// </summary>
        private readonly List<Element> _children = new();

        private float _absoluteHeight;
        private Point2D _absolutePosition = Point2D.Zero;
        private float _absoluteWidth;

        /// <summary>
        /// It's a cache of public children only.
        /// </summary>
        private List<Element>? _cachedPublicChildren;

        private ContainerSizing? _elementContainerSizing;
        private bool _enabled;

        private EdgeInset _margin = new();
        private Dimension _maxHeight = Dimension.Unset;
        private Dimension _maxWidth = Dimension.Unset;
        private Dimension _minHeight = Dimension.Unset;
        private Dimension _minWidth = Dimension.Unset;

        private string _name;
        private EdgeInset _padding = new();
        private Element? _parent;
        private Dimension2 _position = new();
        private Dimension _preferredHeight = new();
        private Dimension _preferredWidth = new();
        private bool _visible;


        public Element(
            string name = "",
            List<Element>? children = null,
            ThemeDefinition<ElementThemeData>? themeOverrides = null,
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
            //Element actions
            Action? onDraw = null,
            EnterDocumentEventHandler? onEnterDocument = null,
            ExitDocumentEventHandler? onExitDocument = null,
            LoadEventHandler? onLoad = null,
            PointerEnterEventHandler? onPointerEnter = null,
            PointerLeaveEventHandler? onPointerLeave = null,
            PointerMoveEventHandler? onPointerMove = null)
        {
            if (IsInstantiated)
            {
                throw new InvalidOperationException("Element has already been instantiated.");
            }

            _name = name;
            if (themeOverrides != null)
            {
                SetElementThemeOverrides(themeOverrides);
            }

            if (position != null)
            {
                _position = position;
            }

            if (preferredWidth != null)
            {
                _preferredWidth = preferredWidth;
            }

            if (preferredHeight != null)
            {
                _preferredHeight = preferredHeight;
            }

            if (minHeight != null)
            {
                _minHeight = minHeight;
            }

            if (minWidth != null)
            {
                _minWidth = minWidth;
            }

            if (maxHeight != null)
            {
                _maxHeight = maxHeight;
            }

            if (maxWidth != null)
            {
                _maxWidth = maxWidth;
            }

            if (elementContainerSizing != null)
            {
                _elementContainerSizing = elementContainerSizing;
            }

            _visible = visible;
            _enabled = enabled;

            OnDraw = onDraw;
            OnEnterDocument = onEnterDocument;
            OnExitDocument = onExitDocument;
            OnLoad = onLoad;
            OnPointerEnter = onPointerEnter;
            OnPointerLeave = onPointerLeave;
            OnPointerMove = onPointerMove;

            InitEvents();
            IsInstantiated = true;

            if (Document != null)
            {
                GetParent()?.AddChild(this);
            }

            if (children != null)
            {
                AddChildren(children);
            }
        }

        public Dimension2 Position
        {
            get => _position;
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

        public ObservableProperty<Dimension2> PositionProperty { get; } = new();

        /// <summary>
        /// Represents the preferred width of the element. The layout engine will try to honor this value, but this might be
        /// influenced
        /// by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion" />) or if the element is
        /// inside a container.
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that
        /// the element will be in.
        /// </summary>
        public Dimension PreferredWidth
        {
            get => _preferredWidth;
            set
            {
                if (value != _preferredWidth)
                {
                    _preferredWidth = value;
                    PreferredWidthProperty.Value = value;
                }
            }
        }

        public ObservableProperty<Dimension> PreferredWidthProperty { get; } = new();

        /// <summary>
        /// Represents the minimum width that the element can have.
        /// By default, it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MinWidth
        {
            get => _minWidth;
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

        public ObservableProperty<Dimension> MinWidthProperty { get; } = new();

        /// <summary>
        /// Represents the maximum width that the element can have.
        /// By default, it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MaxWidth
        {
            get => _maxWidth;
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

        public ObservableProperty<Dimension> MaxWidthProperty { get; } = new();

        /// <summary>
        /// Represents the preferred height of the element. The layout engine will try to honor this value, but this might be
        /// influenced
        /// by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion" />) or if the element is
        /// inside a container.
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that
        /// the element will be in.
        /// </summary>
        public Dimension PreferredHeight
        {
            get => _preferredHeight;
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

        public ObservableProperty<Dimension> PreferredHeightProperty { get; } = new();

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default, it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MinHeight
        {
            get => _minHeight;
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

        public ObservableProperty<Dimension> MinHeightProperty { get; } = new();

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default, it has the invalid value, meaning the restriction is not applied.
        /// </summary>
        public Dimension MaxHeight
        {
            get => _maxHeight;
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

        public ObservableProperty<Dimension> MaxHeightProperty { get; } = new();

        public EdgeInset Padding
        {
            get => _padding;
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

        public ObservableProperty<EdgeInset> PaddingProperty { get; } = new();

        public EdgeInset Margin
        {
            get => _margin;
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

        public ObservableProperty<EdgeInset> MarginProperty { get; } = new();

        public string Name
        {
            get => _name;
            set
            {
                _name = value ?? throw new ArgumentNullException(
                    nameof(value),
                    "The name of an element can be empty, but not null.");
                NameProperty.Value = value;
            }
        }

        public ObservableProperty<string> NameProperty { get; } = new();

        /// <summary>
        /// Controls whether this element is visible or not in the application. An invisible element will still occupy
        /// space in the layout and be moved in a container, just that it is not visible (hidden).
        /// </summary>
        /// <seealso cref="Enabled" />
        public bool Visible
        {
            get => _visible;
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    VisibleProperty.Value = value;

                    foreach (Element child in _children)
                    {
                        child.Visible = value;
                    }

                    RequestRedraw();
                }
            }
        }

        public ObservableProperty<bool> VisibleProperty { get; } = new();

        /// <summary>
        /// If the element is not enabled, it will not be considered in layout recalculations, will not take space in
        /// a layout and will generally give misleading values on properties that are related to layout in any way
        /// such as <see cref="Bounds" /> or <see cref="AbsolutePosition" />.
        /// </summary>
        /// <seealso cref="Visible" />
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    EnabledProperty.Value = value;

                    foreach (Element child in _children)
                    {
                        child.Enabled = value;
                    }

                    RequestRedraw();
                }
            }
        }

        public ObservableProperty<bool> EnabledProperty { get; } = new();

        public ContainerSizing? ElementContainerSizing
        {
            get => _elementContainerSizing;
            set
            {
                if (value != _elementContainerSizing)
                {
                    _elementContainerSizing = value;
                    ElementContainerSizingProperty.Value = value;
                }
            }
        }

        public ObservableProperty<ContainerSizing> ElementContainerSizingProperty { get; } = new();

        public ElementBounds Bounds { get; internal set; } = new();
        public bool IsInternal { get; private set; }
        public UiDocument? Document { get; private set; }

        /// <summary>
        /// True when the element's parent is a container. Only the direct parent is taken into account, not the grandparent
        /// etc.
        /// </summary>
        public bool IsChildOfContainer { get; private set; }

        protected bool IsInstantiated { get; private set; }

        public Point2D AbsolutePosition
        {
            get => _absolutePosition;
            set
            {
                if (Math.Abs(value.X - _absolutePosition.X) > 0.01 ||
                    Math.Abs(value.Y - _absolutePosition.Y) > 0.01)
                {
                    _absolutePosition = value;
                    RecalculateBounds();
                }
            }
        }

        public float AbsoluteWidth
        {
            get => _absoluteWidth;
            set
            {
                if (Math.Abs(value - _absoluteWidth) > 0.01)
                {
                    _absoluteWidth = value;
                    RecalculateBounds();
                }
            }
        }

        public float AbsoluteHeight
        {
            get => _absoluteHeight;
            set
            {
                if (Math.Abs(value - _absoluteHeight) > 0.01)
                {
                    _absoluteHeight = value;
                    RecalculateBounds();
                }
            }
        }

        public event Action? DrawEvent;
        public event EnterDocumentEventHandler? EnterDocumentEvent;
        public event ExitDocumentEventHandler? ExitDocumentEvent;
        public event LoadEventHandler? LoadEvent;
        public event PointerEnterEventHandler? PointerEnterEvent;
        public event PointerLeaveEventHandler? PointerLeaveEvent;
        public event PointerMoveEventHandler? PointerMoveEvent;

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
            PointerLeaveEvent -= OnPointerLeave;
            PointerLeaveEvent -= PointerLeave;
            PointerMoveEvent -= OnPointerMove;
            PointerMoveEvent -= PointerMove;

            DrawEvent -= PrivateDraw;

            _children.Clear();
            _cachedPublicChildren = null;

            Document = null;
            _parent = null;
        }

        /// <summary>
        /// Sets the document of this element and all its children. Will also add the element to the document
        /// (like <see cref="AddChild(Element, bool)" /> or <see cref="AddChildren(Element[])" />).
        /// </summary>
        /// <remarks>
        /// If the element already belongs to a document, this will remove the element, along with all its children,
        /// then add this element along with its previous children to the specified document.
        /// </remarks>
        /// <param name="document">The document to which this element should be added.</param>
        /// <returns>The current element (return this).</returns>
        public Element SetDocument(UiDocument? document)
        {
            //the element is not in a document and the given document is non-null
            if (Document == null && document != null)
            {
                Document = document;
                InvokeEnterDocument();
                RecalculateLayout();
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

            return this;
        }

        #region Visual

        protected virtual void DrawBackground()
        {
            if (!_visible)
            {
                return;
            }

            ElementThemeData currentTheme =
                GetElementFinalThemeData<ElementThemeData>(ElementThemeData.STYLE_NORMAL) ??
                new ElementThemeData().GetDefaultData(ElementThemeData.STYLE_NORMAL);

            IBrush? fillBrush = currentTheme.Background;
            if (fillBrush == null)
            {
                return;
            }

            if (!fillBrush.IsSkippable)
            {
                Document?.Renderer.DrawRect(Bounds.GetPaddingBox(), fillBrush);
            }
        }

        #endregion //Visual

        private void DrawChildren()
        {
            if (!_visible)
            {
                return;
            }

            foreach (Element child in _children)
            {
                child.InvokeDraw();
            }
        }

        private void InitEvents()
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
            PointerLeaveEvent += OnPointerLeave;
            PointerLeaveEvent += PointerLeave;
            PointerMoveEvent += OnPointerMove;
            PointerMoveEvent += PointerMove;

            DrawEvent += PrivateDraw;
        }

        private void EnsureChildrenCache()
        {
            _cachedPublicChildren ??= GetChildren();
        }

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
            PointerLeaveEvent?.Invoke(this, new PointerLeaveEventArgs(Point2D.Zero, false));
        }

        internal void InvokePointerMove()
        {
            PointerMoveEvent?.Invoke(this, new PointerMoveEventArgs(Point2D.Zero, false));
        }

        #endregion //Internal invoke

        #region Internal event handlers

        private void PrivateDraw()
        {
            //RecalculateLayout();
            DrawBackground();
        }

        internal virtual void RecalculateLayout()
        {
            if (IsChildOfContainer || !_enabled)
            {
                return;
            }

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
                AbsoluteWidth = Math.Clamp(
                    CalculateDimension(PreferredWidth, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }

            if (!PreferredHeight.IsUnset())
            {
                AbsoluteHeight = Math.Clamp(
                    CalculateDimension(PreferredHeight, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }

            if (!Position.IsUnset())
            {
                AbsolutePosition = new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight));
            }

            foreach (Element child in _children)
            {
                child.RecalculateLayout();
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
                AbsolutePosition,
                AbsoluteWidth,
                AbsoluteHeight,
                new float[4],
                new float[4]);
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

        public virtual Element Duplicate()
        {
            return new Element(
                _name,
                _children,
                _themeOverrides,
                _position,
                _preferredWidth,
                _preferredHeight,
                _minHeight,
                _minWidth,
                _maxHeight,
                _maxWidth,
                _elementContainerSizing,
                _visible,
                _enabled);
        }

        public void AddChild(Element child, bool isInternal = false)
        {
            _children.Add(child);
            child.IsInternal = isInternal;
            if (!isInternal)
            {
                if (_cachedPublicChildren == null)
                {
                    _cachedPublicChildren = new List<Element> { child };
                }
                else
                {
                    _cachedPublicChildren.Add(child);
                }
            }

            child.IsChildOfContainer = this is Container;

            child._parent = this;
            if (Document != null)
            {
                child.Document = Document;
                child.InvokeEnterDocument();
                child.RecalculateLayout();

                MakeChildrenEnterDocument(child._children);
                RecalculateLayout();
            }
        }

        private void MakeChildrenEnterDocument(List<Element> children)
        {
            foreach (Element child in children)
            {
                child.Document = Document;
                child.InvokeEnterDocument();

                MakeChildrenEnterDocument(child._children);
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

        /// <summary>
        /// Returns a list of all the children of the element. DO NOT modify this list by adding or removing elements!
        /// That could cause crashes or unexpected behavior.
        /// </summary>
        /// <remarks>
        /// This doesn't clone the list in any way, so it has a very good performance and can be called without caching the
        /// result.
        /// </remarks>
        /// <param name="includeInternal">If true, will include the internal children in the list as well.</param>
        /// <returns>A list of all the children of the element, even the internal ones if includeInternal is true.</returns>
        public List<Element> GetChildren(bool includeInternal = false)
        {
            if (includeInternal)
            {
                return _children;
            }

            if (_cachedPublicChildren != null)
            {
                return _cachedPublicChildren;
            }

            _cachedPublicChildren = _children.Where(child => !child.IsInternal).ToList();
            return _cachedPublicChildren;
        }

        public Element GetChild(int index, bool includeInternal = false)
        {
            if (includeInternal)
            {
                return index < 0 ? _children[^index] : _children[index];
            }

            EnsureChildrenCache();
            return index < 0 ? _cachedPublicChildren![^index] : _cachedPublicChildren![index];
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

            child.IsChildOfContainer = false;

            if (Document != null)
            {
                child.InvokeExitDocument();
            }

            child.RemoveAllChildren();

            child._parent = null;
            child.Document = null;
            RecalculateLayout();
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
                        return dimension.Value * (Document?.ContentScale ?? 1);
                    }
                case Unit.Pixels:
                    {
                        return dimension.Value;
                    }
                case Unit.Percent:
                    {
                        if (pixelDimensionForPercent == 0)
                        {
                            return 0;
                        }

                        return dimension.Value * pixelDimensionForPercent / 100f;
                    }
                case Unit.ViewportWidth:
                    {
                        if (Document == null)
                        {
                            return 0;
                        }

                        return dimension.Value * Document.ViewportSize.Width / 100f;
                    }
                case Unit.ViewportHeight:
                    {
                        if (Document == null)
                        {
                            return 0;
                        }

                        return dimension.Value * Document.ViewportSize.Height / 100f;
                    }
            }
        }

        [SuppressMessage("Performance", "CA1822:Mark members as static")]
        public void RequestRedraw()
        {
            //TODO: implement
        }

        #endregion //Public API
    }
}
