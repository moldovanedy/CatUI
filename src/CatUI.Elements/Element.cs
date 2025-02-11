using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Exceptions;
using CatUI.Elements.Behaviors;
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

        public PointerExitEventHandler? OnPointerExit
        {
            get => _onPointerExit;
            set
            {
                PointerExitEvent -= _onPointerExit;
                _onPointerExit = value;
                PointerExitEvent += _onPointerExit;
            }
        }

        private PointerExitEventHandler? _onPointerExit;

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

        /// <summary>
        /// Set this to another <see cref="ObjectRef{T}"/> variable to be able to use this element using
        /// <see cref="ObjectRef{T}.Value"/> (which will be this element). Very useful for accessing this element
        /// outside the scope of a single method.
        /// </summary>
        public ObjectRef<Element>? Ref
        {
            get => _ref;
            set
            {
                _ref = value;
                if (_ref != null)
                {
                    _ref.Value = this;
                }
            }
        }

        private ObjectRef<Element>? _ref;

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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension2 _position = new(0, 0);
        public ObservableProperty<Dimension2> PositionProperty { get; private set; } = new(new Dimension2(0, 0));

        /// <summary>
        /// <para>
        /// Represents the preferred width of the element. The layout engine will try to honor this value, but this might be
        /// influenced by other properties of an element (e.g. <see cref="IExpandable.CanExpandHorizontally" />)
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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _preferredWidth;
        public ObservableProperty<Dimension> PreferredWidthProperty { get; private set; } = new(Dimension.Unset);

        /// <summary>
        /// <para>
        /// Represents the preferred height of the element. The layout engine will try to honor this value, but this might be
        /// influenced by other properties of an element (e.g. <see cref="IExpandable.CanExpandVertically" />)
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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _preferredHeight;
        public ObservableProperty<Dimension> PreferredHeightProperty { get; private set; } = new(Dimension.Unset);

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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _minWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MinWidthProperty { get; private set; } = new(Dimension.Unset);

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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _minHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MinHeightProperty { get; private set; } = new(Dimension.Unset);

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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _maxWidth = Dimension.Unset;
        public ObservableProperty<Dimension> MaxWidthProperty { get; private set; } = new(Dimension.Unset);

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
                    MarkLayoutDirty();
                }
            }
        }

        private Dimension _maxHeight = Dimension.Unset;
        public ObservableProperty<Dimension> MaxHeightProperty { get; private set; } = new(Dimension.Unset);

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
                    MarkLayoutDirty();
                }
            }
        }

        private EdgeInset _margin = new();
        public ObservableProperty<EdgeInset> MarginProperty { get; private set; } = new(new EdgeInset());

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
        public ObservableProperty<IBrush> BackgroundProperty { get; private set; } = new(new ColorBrush(Color.Default));

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
        public ObservableProperty<CornerInset> CornerRadiusProperty { get; private set; } = new(new CornerInset());

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
        public ObservableProperty<string> NameProperty { get; private set; } = new("");

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
        public ObservableProperty<bool> VisibleProperty { get; private set; } = new(true);

        /// <summary>
        /// If the element is not enabled, it will not be considered in layout recalculations, will not take space in
        /// a layout and will generally give misleading values on properties that are related to layout in any way
        /// such as <see cref="Bounds" /> or Bounds' position. The default value is true.
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
        public ObservableProperty<bool> EnabledProperty { get; private set; } = new(true);

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
        public ObservableProperty<ContainerSizing> ElementContainerSizingProperty { get; private set; } = new();

        /// <summary>
        /// Represents the absolute coordinates of this element relative to the viewport. When the element is not
        /// inside the document, these bounds are not reliable (generally representing an empty rect). This also
        /// holds the margins of this element (not used yet).
        /// </summary>
        public ElementBounds Bounds { get; internal set; } = new();

        public int IndexInParent { get; private set; } = -1;

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
                    MarkLayoutDirty();
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

        private bool _shouldCheckForDuplicateChildren = true;

        /// <summary>
        /// Fired when the element needs to be redrawn. Do NOT use this as a continuous consistent source of events (like
        /// a "game loop" that fires x times a second) because this only fires when it's necessary.
        /// </summary>
        public event Action? DrawEvent;

        /// <summary>
        /// Fired when the element is added to a document (see <see cref="Document"/>).
        /// </summary>
        public event EnterDocumentEventHandler? EnterDocumentEvent;

        /// <summary>
        /// Fired when the element is removed from a document (see <see cref="Document"/>).
        /// </summary>
        public event ExitDocumentEventHandler? ExitDocumentEvent;

        /// <summary>
        /// Fired when the element is loaded (i.e. all the resources like images or icons are loaded and can be shown).
        /// Does NOT fire until all the children have loaded so, contrary to most of the other events, this event is
        /// fired in post-order (to children first, then itself), not pre-order.
        /// </summary>
        public event LoadEventHandler? LoadEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) enters the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerEnterEventHandler? PointerEnterEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) exits the <see cref="Bounds"/> of the element.
        /// </summary>
        public event PointerExitEventHandler? PointerExitEvent;

        /// <summary>
        /// Fired when a pointer (like a mouse cursor, for example) moves inside <see cref="Bounds"/> of the element.
        /// This will be fired a lot of times, so ensure your logic is not computationally heavy.
        /// </summary>
        public event PointerMoveEventHandler? PointerMoveEvent;


        public Element(
            Dimension? preferredWidth = null,
            Dimension? preferredHeight = null)
        {
            DrawEvent += Draw;
            EnterDocumentEvent += EnterDocument;
            ExitDocumentEvent += ExitDocument;
            LoadEvent += Loaded;
            PointerEnterEvent += PointerEnter;
            PointerExitEvent += PointerExit;
            PointerMoveEvent += PointerMove;

            PositionProperty.ValueChangedEvent += SetPosition;
            PreferredWidthProperty.ValueChangedEvent += SetPrefWidth;
            PreferredHeightProperty.ValueChangedEvent += SetPrefHeight;
            MinWidthProperty.ValueChangedEvent += SetMinWidth;
            MinHeightProperty.ValueChangedEvent += SetMinHeight;
            MaxWidthProperty.ValueChangedEvent += SetMaxWidth;
            MaxHeightProperty.ValueChangedEvent += SetMaxHeight;
            MarginProperty.ValueChangedEvent += SetMargin;
            BackgroundProperty.ValueChangedEvent += SetBackground;
            CornerRadiusProperty.ValueChangedEvent += SetCornerRadius;
            VisibleProperty.ValueChangedEvent += SetVisible;
            EnabledProperty.ValueChangedEvent += SetEnabled;
            ElementContainerSizingProperty.ValueChangedEvent += SetElementContainerSizing;

            Children.ItemInsertedEvent += OnChildInserted;
            Children.ItemRemovedEvent += OnChildRemoved;
            Children.ItemMovedEvent += OnChildMoved;
            Children.ListClearingEvent += OnChildrenListClearing;

            //we can't set the property itself because it calls MarkLayoutDirty(), which is a virtual method,
            //and it might cause the trouble to the more derived types
            _preferredWidth = preferredWidth ?? Dimension.Unset;
            PreferredWidthProperty.Value = _preferredWidth;

            _preferredHeight = preferredHeight ?? Dimension.Unset;
            PreferredHeightProperty.Value = _preferredHeight;
        }

        ~Element()
        {
            DrawEvent = null;
            EnterDocumentEvent = null;
            ExitDocumentEvent = null;
            LoadEvent = null;
            PointerEnterEvent = null;
            PointerExitEvent = null;
            PointerMoveEvent = null;

            PositionProperty = null!;
            PreferredWidthProperty = null!;
            PreferredHeightProperty = null!;
            MinWidthProperty = null!;
            MinHeightProperty = null!;
            MaxWidthProperty = null!;
            MaxHeightProperty = null!;
            MarginProperty = null!;
            BackgroundProperty = null!;
            CornerRadiusProperty = null!;
            VisibleProperty = null!;
            EnabledProperty = null!;
            ElementContainerSizingProperty = null!;

            //remove from the document, along with all children
            Document = null;
            Children = null!;
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
                Document?.Renderer.DrawRect(Bounds.GetContentBox(), Background);
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


        private bool _shouldRecalculateLayoutOnExit = true;

        private void OnChildInserted(object? sender, ObservableListInsertEventArgs<Element> e)
        {
            if (_shouldCheckForDuplicateChildren && Children.Count(el => el == e.Item) > 1)
            {
                throw new DuplicateElementException("Duplicate children are not allowed.");
            }

            e.Item.IsChildOfContainer = this is Container;
            e.Item._parent = this;
            e.Item.IndexInParent = e.Index;

            if (Document != null)
            {
                e.Item.Document = Document;
                MarkLayoutDirty();
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
            e.Item.IndexInParent = -1;
            e.Item.Bounds = new ElementBounds();

            if (_shouldRecalculateLayoutOnExit)
            {
                MarkLayoutDirty();
            }
        }

        private static void OnChildMoved(object? sender, ObservableListMoveEventArgs<Element> e)
        {
            e.Item.IndexInParent = e.NewIndex;
        }

        private void OnChildrenListClearing(object? sender, EventArgs e)
        {
            _shouldRecalculateLayoutOnExit = false;

            //will clear all children
            while (Children.Count > 0)
            {
                Children.RemoveAt(0);
            }

            _shouldRecalculateLayoutOnExit = true;
            MarkLayoutDirty();
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

        private bool _isPointerInside;
        //TODO: check if we can put this inside a single method
        //

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerEnterEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        internal virtual void CheckInvokePointerEnter(PointerEnterEventArgs e)
        {
            Rect bounds = Bounds.BoundingRect;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokePointerEnter(e);
            }

            if (_isPointerInside)
            {
                return;
            }

            _isPointerInside = true;
            var elementArgs = new PointerEnterEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
            PointerEnterEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerExitEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        internal virtual void CheckInvokePointerExit(PointerExitEventArgs e)
        {
            foreach (Element child in Children)
            {
                child.CheckInvokePointerExit(e);
            }

            Rect bounds = Bounds.BoundingRect;
            if (Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            if (!_isPointerInside)
            {
                return;
            }

            _isPointerInside = false;
            var elementArgs = new PointerExitEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.IsPressed);
            PointerExitEvent?.Invoke(this, elementArgs);
        }

        /// <summary>
        /// This should only be called by the document and only for the root element. It will call this recursively
        /// where applicable so in the end it will fire <see cref="PointerMoveEvent"/> on all the eligible elements.
        /// </summary>
        /// <param name="e">
        /// The arguments that MUST contain the absolute position of the pointer (in window coordinates,
        /// relative to the top-left corner) and whether the pointer is pressed or not.
        /// </param>
        internal virtual void CheckInvokePointerMove(PointerMoveEventArgs e)
        {
            if (!_isPointerInside)
            {
                return;
            }

            Rect bounds = Bounds.BoundingRect;
            if (!Rect.IsPointInside(ref bounds, e.Position))
            {
                return;
            }

            foreach (Element child in Children)
            {
                child.CheckInvokePointerMove(e);
            }

            var elementArgs = new PointerMoveEventArgs(
                new Point2D(e.AbsolutePosition.X - bounds.X, e.AbsolutePosition.Y - bounds.Y),
                e.AbsolutePosition,
                e.DeltaX,
                e.DeltaY,
                e.IsPressed);
            PointerMoveEvent?.Invoke(this, elementArgs);
        }

        private void SetPosition(Dimension2 value)
        {
            _position = value;
            MarkLayoutDirty();
        }

        private void SetPrefWidth(Dimension value)
        {
            _preferredWidth = value;
            MarkLayoutDirty();
        }

        private void SetPrefHeight(Dimension value)
        {
            _preferredHeight = value;
            MarkLayoutDirty();
        }

        private void SetMinWidth(Dimension value)
        {
            _minWidth = value;
            MarkLayoutDirty();
        }

        private void SetMinHeight(Dimension value)
        {
            _minHeight = value;
            MarkLayoutDirty();
        }

        private void SetMaxWidth(Dimension value)
        {
            _maxWidth = value;
            MarkLayoutDirty();
        }

        private void SetMaxHeight(Dimension value)
        {
            _maxHeight = value;
            MarkLayoutDirty();
        }

        private void SetMargin(EdgeInset value)
        {
            _margin = value;
            MarkLayoutDirty();
        }

        private void SetBackground(IBrush? value)
        {
            _background = value ?? new ColorBrush(Color.Default);
        }

        private void SetCornerRadius(CornerInset value)
        {
            _cornerRadius = value;
        }

        private void SetVisible(bool value)
        {
            _visible = value;
        }

        private void SetEnabled(bool value)
        {
            _enabled = value;
        }

        private void SetElementContainerSizing(ContainerSizing? value)
        {
            _elementContainerSizing = value;
        }

        #endregion //Internal invoke

        #region Internal event handlers

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
                parentWidth = _parent?.Bounds.BoundingRect.Width ?? 0;
                parentHeight = _parent?.Bounds.BoundingRect.Height ?? 0;
                parentXPos = _parent?.Bounds.BoundingRect.X ?? 0;
                parentYPos = _parent?.Bounds.BoundingRect.Y ?? 0;
            }

            float width, height;
            Point2D position;

            if (!PreferredWidth.IsUnset())
            {
                width = Math.Clamp(
                    CalculateDimension(PreferredWidth, parentWidth),
                    MinWidth.IsUnset() ? float.MinValue : CalculateDimension(MinWidth, parentWidth),
                    MaxWidth.IsUnset() ? float.MaxValue : CalculateDimension(MaxWidth, parentWidth));
            }
            else
            {
                width = MinWidth.IsUnset() ? 0 : CalculateDimension(MinWidth, parentWidth);
            }

            if (!PreferredHeight.IsUnset())
            {
                height = Math.Clamp(
                    CalculateDimension(PreferredHeight, parentHeight),
                    MinHeight.IsUnset() ? float.MinValue : CalculateDimension(MinHeight, parentHeight),
                    MaxHeight.IsUnset() ? float.MaxValue : CalculateDimension(MaxHeight, parentHeight));
            }
            else
            {
                height = MinHeight.IsUnset() ? 0 : CalculateDimension(MinHeight, parentHeight);
            }

            if (!Position.IsUnset())
            {
                position = new Point2D(
                    parentXPos + CalculateDimension(Position.X, parentWidth),
                    parentYPos + CalculateDimension(Position.Y, parentHeight));
            }
            else
            {
                position = new Point2D(parentXPos, parentYPos);
            }

            Bounds = new ElementBounds(
                new Rect(position.X, position.Y, width, height),
                new Vector4());
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
        public virtual void PointerExit(object sender, PointerExitEventArgs e) { }
        public virtual void PointerMove(object sender, PointerMoveEventArgs e) { }

        /// <summary>
        /// Deep clones the element. The element will not belong to the document, but will have all the original properties
        /// cloned, except callbacks (like <see cref="OnDraw"/>) and assets (like <see cref="Image"/>).
        /// </summary>
        /// <returns>
        /// A new clone of the object that is not attached to the document, but has the properties of the original.
        /// </returns>
        public virtual Element Duplicate()
        {
            return new Element
            {
                Position = _position,
                PreferredWidth = _preferredWidth,
                PreferredHeight = _preferredHeight,
                MinWidth = _minWidth,
                MinHeight = _minHeight,
                MaxWidth = _maxWidth,
                MaxHeight = _maxHeight,
                Margin = _margin,
                Background = _background.Duplicate(),
                CornerRadius = _cornerRadius,
                Visible = _visible,
                Enabled = _enabled,
                ElementContainerSizing = (ContainerSizing?)_elementContainerSizing?.Duplicate()
            };
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
        /// Will return the actual pixel value of the given dimension. If the element is not inside a document,
        /// this method might give unpredictable results that are incorrect (e.g. 0 when the measuring unit is
        /// <see cref="Unit.ViewportWidth"/> or <see cref="Unit.ViewportHeight"/>).
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

        /// <summary>
        /// Will recalculate the layout of this element (<see cref="RecalculateLayout"/>) and call this function
        /// on all its children (can be an expensive operation). Then it will notify the parent that this child modified
        /// its layout and must act accordingly (i.e. recompute bounds if necessary). It is generally not necessary
        /// to call this directly, as changing the parameters will call this automatically if it's necessary.
        /// </summary>
        /// <remarks>
        /// If this element is not <see cref="Enabled"/>, it does nothing (the same for any child). If the element
        /// is a child of a <see cref="Container"/>, it will only call this for children, as bounds should already
        /// be set by the parent, and it's the responsibility of the <see cref="Container"/> to recalculate the bounds
        /// of this element (inside <see cref="Container.RecalculateContainerChildren"/>). If this element is a
        /// <see cref="Container"/>, it calls <see cref="RecalculateLayout"/> only for this element, not calling
        /// this method on any child (if the container is NOT a child of another container, otherwise it will simply
        /// call <see cref="Container.RecalculateContainerChildren"/>).
        /// </remarks>
        public void MarkLayoutDirty()
        {
            if (!Enabled || !IsInsideDocument())
            {
                return;
            }

            if (!IsChildOfContainer)
            {
                RecalculateLayout();
            }

            if (this is Container container)
            {
                container.RecalculateContainerChildren();
                return;
            }

            foreach (Element child in Children)
            {
                if (child.Enabled)
                {
                    child.MarkLayoutDirty();
                }
            }

            //TODO: notify parent
        }

        #endregion //Public API
    }
}
