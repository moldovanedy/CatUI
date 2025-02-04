using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Exceptions;
using CatUI.Elements.Containers;
using CatUI.Utils;

namespace CatUI.Elements
{
    public partial class Element
    {
        public Action? OnDraw
        {
            get => _onDraw;
            set
            {
                DrawEvent -= _onDraw;
                _onDraw = value;
                DrawEvent += _onDraw;
            }
        }

        private Action? _onDraw;

        public EnterDocumentEventHandler? OnEnterDocument
        {
            get => _onEnterDocument;
            set
            {
                EnterDocumentEvent -= _onEnterDocument;
                _onEnterDocument = value;
                EnterDocumentEvent += _onEnterDocument;
            }
        }

        private EnterDocumentEventHandler? _onEnterDocument;

        public ExitDocumentEventHandler? OnExitDocument
        {
            get => _onExitDocument;
            set
            {
                ExitDocumentEvent -= _onExitDocument;
                _onExitDocument = value;
                ExitDocumentEvent += _onExitDocument;
            }
        }

        private ExitDocumentEventHandler? _onExitDocument;

        public LoadEventHandler? OnLoad
        {
            get => _onLoad;
            set
            {
                LoadEvent -= _onLoad;
                _onLoad = value;
                LoadEvent += _onLoad;
            }
        }

        private LoadEventHandler? _onLoad;

        public PointerEnterEventHandler? OnPointerEnter
        {
            get => _onPointerEnter;
            set
            {
                PointerEnterEvent -= _onPointerEnter;
                _onPointerEnter = value;
                PointerEnterEvent += _onPointerEnter;
            }
        }

        private PointerEnterEventHandler? _onPointerEnter;

        public PointerLeaveEventHandler? OnPointerLeave
        {
            get => _onPointerLeave;
            set
            {
                PointerLeaveEvent -= _onPointerLeave;
                _onPointerLeave = value;
                PointerLeaveEvent += _onPointerLeave;
            }
        }

        private PointerLeaveEventHandler? _onPointerLeave;

        public PointerMoveEventHandler? OnPointerMove
        {
            get => _onPointerMove;
            set
            {
                PointerMoveEvent -= _onPointerMove;
                _onPointerMove = value;
                PointerMoveEvent += _onPointerMove;
            }
        }

        private PointerMoveEventHandler? _onPointerMove;

        private Element? _parent;

        public ObservableList<Element> Children
        {
            get => _children;
            set
            {
                _children.Clear();
                _children.AddRange(value);
            }
        }

        private readonly ObservableList<Element> _children = new();

        /// <summary>
        /// Represents the top-left corner's position relative to the parent's top-left corner. When the element is
        /// inside a container, this value generally won't get considered. Default is (0, 0). 
        /// </summary>
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

        private Dimension2 _position = new(0, 0);
        public ObservableProperty<Dimension2> PositionProperty { get; } = new(new Dimension2(0, 0));

        /// <summary>
        /// <para>
        /// Represents the preferred width of the element. The layout engine will try to honor this value, but this might be
        /// influenced by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion" />)
        /// or if the element is inside a container.
        /// </para>
        /// <para>
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that
        /// the element will be in. The default value is <see cref="Dimension.Unset"/>.
        /// </para>
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
                    RecalculateLayout();
                }
            }
        }

        private Dimension _preferredWidth;
        public ObservableProperty<Dimension> PreferredWidthProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// <para>
        /// Represents the preferred height of the element. The layout engine will try to honor this value, but this might be
        /// influenced by other properties of an element (e.g. <see cref="Text.TextElement.AllowsExpansion" />)
        /// or if the element is inside a container.
        /// </para>
        /// <para>
        /// Please consult the documentation for the properties of the element you want to use, as well as the containers that
        /// the element will be in. The default value is <see cref="Dimension.Unset"/>.
        /// </para>
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

        private Dimension _preferredHeight;
        public ObservableProperty<Dimension> PreferredHeightProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// Represents the minimum width that the element can have.
        /// By default, it has the invalid value (<see cref="Dimension.Unset"/>), meaning the restriction is not applied.
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

        private Dimension _minWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MinWidthProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default, it has the invalid value (<see cref="Dimension.Unset"/>), meaning the restriction is not applied.
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

        private Dimension _minHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MinHeightProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// Represents the maximum width that the element can have.
        /// By default, it has the invalid value (<see cref="Dimension.Unset"/>), meaning the restriction is not applied.
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

        private Dimension _maxWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MaxWidthProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// Represents the minimum height that the element can have.
        /// By default, it has the invalid value (<see cref="Dimension.Unset"/>), meaning the restriction is not applied.
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

        private Dimension _maxHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MaxHeightProperty { get; } = new(Dimension.Unset);

        /// <summary>
        /// The default value is a new <see cref="EdgeInset"/> with all the dimensions invalid (<see cref="Dimension.Unset"/>).
        /// </summary>
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

        private EdgeInset _padding = new();
        public ObservableProperty<EdgeInset> PaddingProperty { get; } = new(new EdgeInset());

        /// <summary>
        /// The default value is a new <see cref="EdgeInset"/> with all the dimensions invalid (<see cref="Dimension.Unset"/>).
        /// </summary>
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

        private EdgeInset _margin = new();
        public ObservableProperty<EdgeInset> MarginProperty { get; } = new(new EdgeInset());

        /// <summary>
        /// Specifies the brush to use to draw the element's background. By default, it's completely transparent,
        /// so no drawing of the background happens.
        /// </summary>
        public IBrush Background
        {
            get => _background;
            set
            {
                _background = value;
                BackgroundProperty.Value = value;
            }
        }

        private IBrush _background = new ColorBrush(Color.Default);
        public ObservableProperty<IBrush> BackgroundProperty { get; } = new(new ColorBrush(Color.Default));

        /// <summary>
        /// The radius of the corners of the element. Influences the <see cref="Background"/> drawing, as well as clipping.
        /// The default value is a new <see cref="CornerInset"/> with no radius, so all corners have a radius of 0.
        /// </summary>
        public CornerInset CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                CornerRadiusProperty.Value = value;
            }
        }

        private CornerInset _cornerRadius = new();
        public ObservableProperty<CornerInset> CornerRadiusProperty { get; } = new(new CornerInset());

        /// <summary>
        /// Represents the name of this element. This is useful for finding the element inside a hierarchy.
        /// The default value is an empty string; it can have any value, except null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the name is set to null.</exception>
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

        private string _name = "";
        public ObservableProperty<string> NameProperty { get; } = new("");

        /// <summary>
        /// Controls whether this element is visible or not in the application. An invisible element will still occupy
        /// space in the layout and be moved in a container, just that it is not visible (it is hidden).
        /// The default value is true.
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

                    foreach (Element child in Children)
                    {
                        child.Visible = value;
                    }

                    RequestRedraw();
                }
            }
        }

        private bool _visible = true;
        public ObservableProperty<bool> VisibleProperty { get; } = new(true);

        /// <summary>
        /// If the element is not enabled, it will not be considered in layout recalculations, will not take space in
        /// a layout and will generally give misleading values on properties that are related to layout in any way
        /// such as <see cref="Bounds" /> or <see cref="AbsolutePosition" />. The default value is true.
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

                    foreach (Element child in Children)
                    {
                        child.Enabled = value;
                    }

                    RequestRedraw();
                }
            }
        }

        private bool _enabled = true;
        public ObservableProperty<bool> EnabledProperty { get; } = new(true);

        /// <summary>
        /// Gives information on how to work with this element inside a container. The value is dependent on each
        /// type of container. If the given value is incompatible with what the container expects, the container will
        /// simply ignore the value and act as if it was null (see remarks). The default value is null.
        /// </summary>
        /// <remarks>
        /// If this is null (the default value), the container will interpret this element as stated in the documentation
        /// corresponding to that type of container from <see cref="ContainerSizing"/> for the default value
        /// (e.g. for a <see cref="HBoxContainer"/> the documentation will be present in <see cref="HBoxContainerSizing"/>).
        /// </remarks>
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

        private ContainerSizing? _elementContainerSizing;
        public ObservableProperty<ContainerSizing> ElementContainerSizingProperty { get; } = new();

        public ElementBounds Bounds { get; internal set; } = new();

        /// <summary>
        /// Gets or sets the document of this element and all its children. Will also add the element to the document.
        /// </summary>
        /// <remarks>
        /// If the element already belongs to a document, this will remove the element, along with all its children,
        /// then add this element along with its previous children to the specified document. It WILL invoke the
        /// document enter/exit events.
        /// </remarks>
        public UiDocument? Document
        {
            get => _document;
            set
            {
                //the element is not in a document and the given document is non-null
                if (_document == null && value != null)
                {
                    _document = value;
                    InvokeEnterDocument();
                    RecalculateLayout();
                    MakeChildrenEnterDocument(Children);
                }
                //the element is in a document and the given document is another document or null
                else if (_document != value)
                {
                    GetParent()?.Children.Remove(this);
                    _document = value;

                    if (value != null)
                    {
                        GetParent()?.Children.Add(this);
                    }
                }
            }
        }

        private UiDocument? _document;

        /// <summary>
        /// True when the element's parent is a container. Only the direct parent is taken into account, not the grandparent
        /// etc.
        /// </summary>
        public bool IsChildOfContainer { get; private set; }

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

        private Point2D _absolutePosition = Point2D.Zero;

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

        private float _absoluteWidth;

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

        private float _absoluteHeight;

        private bool _shouldCheckForDuplicateChildren = true;

        public event Action? DrawEvent;
        public event EnterDocumentEventHandler? EnterDocumentEvent;
        public event ExitDocumentEventHandler? ExitDocumentEvent;
        public event LoadEventHandler? LoadEvent;
        public event PointerEnterEventHandler? PointerEnterEvent;
        public event PointerLeaveEventHandler? PointerLeaveEvent;
        public event PointerMoveEventHandler? PointerMoveEvent;


        public Element(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
        {
            InitEvents();

            Children.ItemInsertedEvent += OnChildInserted;
            Children.ItemRemovedEvent += OnChildRemoved;
            Children.ListClearingEvent += OnChildrenListClearing;

            //we can't set the property itself because it calls RecalculateLayout(), which is a virtual method,
            //and it might cause the trouble to the more derived types
            _preferredWidth = preferredWidth ?? Dimension.Unset;
            PreferredWidthProperty.Value = _preferredWidth;

            _preferredHeight = preferredHeight ?? Dimension.Unset;
            PreferredHeightProperty.Value = _preferredHeight;
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
            PointerLeaveEvent -= OnPointerLeave;
            PointerLeaveEvent -= PointerLeave;
            PointerMoveEvent -= OnPointerMove;
            PointerMoveEvent -= PointerMove;

            Children.ItemInsertedEvent -= OnChildInserted;
            Children.ItemRemovedEvent -= OnChildRemoved;
            Children.ListClearingEvent -= OnChildrenListClearing;

            Document = null;
            _parent = null;
        }

        #region Visual

        protected virtual void DrawBackground()
        {
            if (!_visible)
            {
                return;
            }

            if (!Background.IsSkippable)
            {
                Document?.Renderer.DrawRect(Bounds.GetPaddingBox(), Background);
            }
        }

        #endregion //Visual

        private void DrawChildren()
        {
            if (!_visible)
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.InvokeDraw();
            }
        }

        private void InitEvents()
        {
            DrawEvent += Draw;
            EnterDocumentEvent += EnterDocument;
            ExitDocumentEvent += ExitDocument;
            LoadEvent += Loaded;
            PointerEnterEvent += PointerEnter;
            PointerLeaveEvent += PointerLeave;
            PointerMoveEvent += PointerMove;
        }


        private void OnChildInserted(object? sender, ObservableListInsertEventArgs<Element> e)
        {
            if (_shouldCheckForDuplicateChildren && Children.Count(el => el == e.Item) > 1)
            {
                throw new DuplicateElementException("Duplicate children are not allowed.");
            }

            e.Item.IsChildOfContainer = this is Container;
            e.Item._parent = this;

            if (Document != null)
            {
                e.Item.Document = Document;
                RecalculateLayout();
            }
        }

        private void OnChildRemoved(object? sender, ObservableListRemoveEventArgs<Element> e)
        {
            e.Item.IsChildOfContainer = false;

            if (Document != null)
            {
                e.Item.InvokeExitDocument();
            }

            e.Item.Children.Clear();
            e.Item._parent = null;
            e.Item._document = null;
            RecalculateLayout();
        }

        private void OnChildrenListClearing(object? sender, EventArgs e)
        {
            //will clear all children
            while (Children.Count > 0)
            {
                Children.RemoveAt(0);
            }
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
            foreach (Element child in Children)
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

            foreach (Element child in Children)
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

        private void MakeChildrenEnterDocument(ObservableList<Element> children)
        {
            foreach (Element child in children)
            {
                child.Document = Document;
                MakeChildrenEnterDocument(child.Children);
            }
        }

        #endregion //Internal event handlers

        #region Public API

        public virtual void Draw()
        {
            DrawBackground();
        }

        public virtual void EnterDocument(object sender) { }
        public virtual void ExitDocument(object sender) { }
        public virtual void Loaded(object sender) { }
        public virtual void PointerEnter(object sender, PointerEnterEventArgs e) { }
        public virtual void PointerLeave(object sender, PointerLeaveEventArgs e) { }
        public virtual void PointerMove(object sender, PointerMoveEventArgs e) { }

        public virtual Element Duplicate()
        {
            return new Element();
        }

        /// <summary>
        /// This method is for special cases only! When you have to add a lot of elements at once, and you already ensured
        /// that you have no duplicates, disable the check before adding the children and enable it immediately afterward.
        /// Any element insertion will first check to see if the new element isn't already there and, if it is,
        /// it throws an <see cref="DuplicateElementException"/>. The check is active by default, and you should really
        /// not mess with it unless you know what you are doing. The case above is the only reason this method exists.
        /// </summary>
        /// <remarks>
        /// If you disable this check, and you insert duplicate children, the whole element hierarchy might get corrupted,
        /// and you will get undefined behaviour.
        /// </remarks>
        /// <param name="shouldEnable">If true, enables the check; if false, disables it.</param>
        public void ToggleDuplicateChildrenCheck(bool shouldEnable)
        {
            _shouldCheckForDuplicateChildren = shouldEnable;
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
