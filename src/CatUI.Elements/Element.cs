using System;
using System.Linq;
using CatUI.Data;
using CatUI.Data.Assets;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.Enums;
using CatUI.Data.Events.Document;
using CatUI.Data.Exceptions;
using CatUI.Data.Shapes;
using CatUI.Elements.Containers;
using CatUI.Utils;
using Container = CatUI.Elements.Containers.Container;

namespace CatUI.Elements
{
    public partial class Element
    {
        public DrawEventHandler? OnDraw
        {
            get => _onDraw;
            set
            {
                DrawEvent -= _onDraw;
                _onDraw = value;
                DrawEvent += _onDraw;
            }
        }

        private DrawEventHandler? _onDraw;

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

                //this is because we set children to null on destruction, so it can cause trouble
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (value != null)
                {
                    _shouldRecalculateLayout = false;
                    foreach (Element child in value)
                    {
                        child._shouldRecalculateLayout = false;
                        _children.Add(child);
                        child._shouldRecalculateLayout = true;
                    }

                    _shouldRecalculateLayout = true;

                    MarkLayoutDirty();
                }
            }
        }

        private readonly ObservableList<Element> _children = [];

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
                    SetPosition(value);
                    PositionProperty.Value = value;
                }
            }
        }

        private Dimension2 _position = new(0, 0);
        public ObservableProperty<Dimension2> PositionProperty { get; private set; } = new(new Dimension2(0, 0));

        private void SetPosition(Dimension2 value)
        {
            _position = value;
            MarkLayoutDirty();
        }

        /// <summary>
        /// Specifies the brush to use to draw the element's background. By default, it's completely transparent,
        /// so no drawing of the background happens.
        /// </summary>
        public IBrush Background
        {
            get => _background;
            set
            {
                SetBackground(value);
                BackgroundProperty.Value = value;
            }
        }

        private IBrush _background = new ColorBrush(Color.Default);
        public ObservableProperty<IBrush> BackgroundProperty { get; private set; } = new(new ColorBrush(Color.Default));

        private void SetBackground(IBrush? value)
        {
            _background = value ?? new ColorBrush(Color.Default);
        }

        /// <summary>
        /// Sets the clipping path of the element. This is used for clipping the element's content and drawing (if
        /// <see cref="ClipType"/> has at least set <see cref="ClipApplicability.Drawing"/>) and for hit testing
        /// (i.e. check if the user pointer is inside the element or touching it), so consider if you really need to set it
        /// because it can be computationally expensive if you set it to a <see cref="PathClipShape"/>.
        /// Default value is null, so the hit testing will be just like a rectangle with values from <see cref="Bounds"/>
        /// and no drawing clipping will happen.
        /// </summary>
        /// <remarks>
        /// It can be computationally expensive to check if the pointer is inside because sometimes the check will
        /// happen every frame for several tenths of seconds or entire seconds, but for any kind of shape other than
        /// <see cref="PathClipShape"/>, the performance impact is very low to negligible. However, for maximum performance,
        /// you should leave this as null and only set it when necessary.
        /// </remarks>
        public ClipShape? ClipPath
        {
            get => _clipPath;
            set
            {
                SetClipPath(value);
                ClipPathProperty.Value = value;
            }
        }

        private ClipShape? _clipPath;
        public ObservableProperty<ClipShape> ClipPathProperty { get; private set; } = new(null);

        private void SetClipPath(ClipShape? value)
        {
            _clipPath = value;
        }

        /// <summary>
        /// Represents how the <see cref="ClipPath"/> will be used. Even if <see cref="ClipApplicability.HitTesting"/>
        /// is not set, the hit testing will still happen, just that it will happen on <see cref="Bounds"/> instead of
        /// <see cref="ClipPath"/>. The default value is <see cref="ClipApplicability.All"/>.
        /// </summary>
        /// <remarks>If <see cref="ClipPath"/> is null, this property is ignored.</remarks>
        public ClipApplicability ClipType
        {
            get => _clipType;
            set
            {
                SetClipType(value);
                ClipTypeProperty.Value = value;
            }
        }

        private ClipApplicability _clipType = ClipApplicability.All;

        public ObservableProperty<ClipApplicability> ClipTypeProperty { get; private set; } =
            new(ClipApplicability.All);

        private void SetClipType(ClipApplicability value)
        {
            _clipType = value;
        }

        /// <summary>
        /// Represents the ID of this element. This is useful for finding the element inside a hierarchy.
        /// The default value is null. This ID must be unique to all the elements in the document, otherwise setting
        /// this will result in a <see cref="DuplicateIdException"/> to be thrown.
        /// </summary>
        /// <remarks>
        /// Each time that you add or update the ID, the entry in the document element cache (i.e. the one that's used
        /// for <see cref="UiDocument.GetElementById"/>) will be updated. Similarly, when you set this to null,
        /// the entry is removed. The entries are automatically removed when the element is removed from the
        /// document or added when the element is added to the document if needed. 
        /// </remarks>
        public string? Id
        {
            get => _id;
            set
            {
                SetId(value);
                IdProperty.Value = value;
            }
        }

        private string? _id;
        public ObservableProperty<string> IdProperty { get; private set; } = new(null);

        private void SetId(string? value)
        {
            if (_id != null)
            {
                Document?.RemoveFromIdCache(_id);
            }

            _id = value;

            if (_id != null)
            {
                Document?.AddToIdCache(this);
            }
        }

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
                    SetVisible(value);
                    VisibleProperty.Value = value;
                }
            }
        }

        private bool _visible = true;
        public ObservableProperty<bool> VisibleProperty { get; private set; } = new(true);

        private void SetVisible(bool value)
        {
            _visible = value;
            foreach (Element child in Children)
            {
                child.Visible = value;
            }

            RequestRedraw();
        }

        /// <summary>
        /// If the element is not enabled, it will not be considered in layout recalculations, will not take space in
        /// a layout and will generally give misleading values on properties that are related to layout in any way
        /// such as <see cref="Bounds" />. The default value is true.
        /// </summary>
        /// <seealso cref="Visible" />
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value != _enabled)
                {
                    SetEnabled(value);
                    EnabledProperty.Value = value;
                }
            }
        }

        private bool _enabled = true;
        public ObservableProperty<bool> EnabledProperty { get; private set; } = new(true);

        private void SetEnabled(bool value)
        {
            _enabled = value;
            foreach (Element child in Children)
            {
                child.Enabled = value;
            }

            RequestRedraw();
        }

        /// <summary>
        /// Gives information on how to work with this element inside a container. The value is dependent on each
        /// type of container. If the given value is incompatible with what the container expects, the container will
        /// simply ignore the value and act as if it was null (see remarks). The default value is null.
        /// </summary>
        /// <remarks>
        /// If this is null (the default value), the container will interpret this element as stated in the documentation
        /// corresponding to that type of container from <see cref="ContainerSizing"/> for the default value
        /// (e.g. for a <see cref="RowContainer"/> the documentation will be present in <see cref="RowContainerSizing"/>).
        /// </remarks>
        public ContainerSizing? ElementContainerSizing
        {
            get => _elementContainerSizing;
            set
            {
                if (value != _elementContainerSizing)
                {
                    SetElementContainerSizing(value);
                    ElementContainerSizingProperty.Value = value;
                }
            }
        }

        private ContainerSizing? _elementContainerSizing;
        public ObservableProperty<ContainerSizing> ElementContainerSizingProperty { get; private set; } = new();

        private void SetElementContainerSizing(ContainerSizing? value)
        {
            _elementContainerSizing = value;
            MarkLayoutDirty();
        }

        /// <summary>
        /// A function that is run directly when set. This is useful for binding properties or running any kind of logic
        /// at object creation, but after the constructor. The parameter is the object itself (this). See the example
        /// for more info.
        /// </summary>
        /// <example>
        /// new Element <br/>
        /// { <br/>
        ///     Option1 = value, <br/>
        ///     ... <br/>
        ///     InitializationFunction = (obj) => ... <br/>
        /// }
        /// </example>
        public Action<Element>? InitializationFunction
        {
            get => _initializationFunction;
            set
            {
                _initializationFunction = value;
                _initializationFunction?.Invoke(this);
            }
        }

        private Action<Element>? _initializationFunction;

        /// <summary>
        /// Represents the absolute coordinates of this element relative to the viewport. When the element is not
        /// inside the document, these bounds are not reliable (generally being outdated).
        /// </summary>
        public Rect Bounds { get; set; } = new();

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
                    MakeChildrenEnterDocument(Children);

                    if (_shouldRecalculateLayout)
                    {
                        MarkLayoutDirty();
                    }
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

        public bool IsInsideDocument => Document != null;

        /// <summary>
        /// If true, any child addition will be checked first to ensure there are no duplicates. See
        /// <see cref="ToggleDuplicateChildrenCheck"/> for more info.
        /// </summary>
        public bool IsCheckingForDuplicateChildren { get; private set; } = true;

        /// <summary>
        /// Fired when the element needs to be redrawn. Do NOT use this as a continuous consistent source of events (like
        /// a "game loop" that fires x times a second) because this only fires when it's necessary.
        /// </summary>
        public event DrawEventHandler? DrawEvent;

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


        public Element()
        {
            DrawEvent += Draw;
            EnterDocumentEvent += EnterDocument;
            ExitDocumentEvent += ExitDocument;
            LoadEvent += Loaded;

            //see ElementInputPartial
            PointerEnterEvent += PointerEnter;
            PointerExitEvent += PointerExit;
            PointerMoveEvent += PointerMove;
            PointerDownEvent += PointerDown;
            PointerUpEvent += PointerUp;
            MouseButtonEvent += MouseButton;
            MouseWheelEvent += MouseWheel;

            ChildLayoutChangedEvent += OnChildLayoutChanged;

            PositionProperty.ValueChangedEvent += SetPosition;
            BackgroundProperty.ValueChangedEvent += SetBackground;
            ClipPathProperty.ValueChangedEvent += SetClipPath;
            ClipTypeProperty.ValueChangedEvent += SetClipType;
            IdProperty.ValueChangedEvent += SetId;
            VisibleProperty.ValueChangedEvent += SetVisible;
            EnabledProperty.ValueChangedEvent += SetEnabled;
            ElementContainerSizingProperty.ValueChangedEvent += SetElementContainerSizing;

            Children.ItemInsertedEvent += OnChildInserted;
            Children.ItemRemovedEvent += OnChildRemoved;
            Children.ItemMovedEvent += OnChildMoved;
            Children.ListClearingEvent += OnChildrenListClearing;
        }

        ~Element()
        {
            DrawEvent = null;
            EnterDocumentEvent = null;
            ExitDocumentEvent = null;
            LoadEvent = null;

            //see ElementInputPartial
            PointerEnterEvent = null;
            PointerExitEvent = null;
            PointerMoveEvent = null;
            PointerDownEvent = null;
            PointerUpEvent = null;
            MouseButtonEvent = null;
            MouseWheelEvent = null;

            ChildLayoutChangedEvent = null;

            PositionProperty = null!;
            BackgroundProperty = null!;
            ClipPathProperty = null!;
            ClipTypeProperty = null!;
            IdProperty = null!;
            VisibleProperty = null!;
            EnabledProperty = null!;
            ElementContainerSizingProperty = null!;

            LayoutProperty = null!;

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
                Document?.Renderer.DrawRect(Bounds, Background);
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


        private bool _shouldRecalculateLayout = true;

        private void OnChildInserted(object? sender, ObservableListInsertEventArgs<Element> e)
        {
            if (IsCheckingForDuplicateChildren && Children.Count(el => el == e.Item) > 1)
            {
                throw new DuplicateElementException("Duplicate children are not allowed.");
            }

            e.Item.IsChildOfContainer = this is Container;
            e.Item._parent = this;
            e.Item.IndexInParent = e.Index;


            if (Document != null)
            {
                e.Item.Document = Document;
                if (e.Item._id != null)
                {
                    Document.AddToIdCache(e.Item);
                }

                if (_shouldRecalculateLayout)
                {
                    MarkLayoutDirty();
                }
            }
        }

        private void OnChildRemoved(object? sender, ObservableListRemoveEventArgs<Element> e)
        {
            e.Item.IsChildOfContainer = false;
            if (e.Item._id != null)
            {
                Document?.RemoveFromIdCache(e.Item._id);
            }

            if (Document != null)
            {
                e.Item.InvokeExitDocument();
            }

            //WARNING: Order is important! We first remove the child's connections to its parent, THEN clear all its
            //descendants and set finally set the document to null. Setting the document to null before clearing won't
            //fire ExitDocumentEvent, while clearing children first, then removing connections to its parent will result
            //in crashes because this element **already removed the child** from its children list, but the child still has
            //the reference to the parent, which will cause IndexOutOfRangeException and unexpected behavior in general
            e.Item._parent = null;
            e.Item.IndexInParent = -1;
            e.Item.Bounds = new Rect();

            e.Item.Children.Clear();
            e.Item._document = null;

            if (_shouldRecalculateLayout)
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
            _shouldRecalculateLayout = false;

            //will clear all children
            while (_children.Count > 0)
            {
                _children[0]._shouldRecalculateLayout = false;
                _children.RemoveAt(0);
            }

            _shouldRecalculateLayout = true;
            MarkLayoutDirty();
        }

        #region Internal invoke

        internal void InvokeDraw()
        {
            DrawEvent?.Invoke(this);
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

        #endregion //Internal invoke

        #region Internal event handlers

        private void MakeChildrenEnterDocument(ObservableList<Element> children)
        {
            _shouldRecalculateLayout = false;

            foreach (Element child in children)
            {
                child._shouldRecalculateLayout = false;
                child.Document = Document;
                child._shouldRecalculateLayout = true;
            }

            _shouldRecalculateLayout = true;
        }

        #endregion //Internal event handlers

        #region Public API

        protected virtual void Draw(object sender)
        {
            DrawBackground();
        }

        protected virtual void EnterDocument(object sender) { }
        protected virtual void ExitDocument(object sender) { }
        protected virtual void Loaded(object sender) { }

        /// <summary>
        /// Deep clones the element. The element will not belong to the document, but will have all the original properties
        /// cloned, except callbacks (like <see cref="OnDraw"/>) and assets (like <see cref="Image"/>).
        /// </summary>
        /// <returns>
        /// A new deep clone of the object that is not attached to the document, but has the properties of the original.
        /// </returns>
        public virtual Element Duplicate()
        {
            return new Element
            {
                Position = _position,
                Background = _background.Duplicate(),
                ClipPath = (ClipShape?)_clipPath?.Duplicate(),
                ClipType = _clipType,
                Visible = _visible,
                Enabled = _enabled,
                ElementContainerSizing = (ContainerSizing?)_elementContainerSizing?.Duplicate(),
                Layout = _layout
            };
        }

        /// <summary>
        /// This method is for special cases only! When you have to add a lot of elements at once, and you already ensured
        /// that you have no duplicates, disable the check before adding the children and enable it immediately afterward
        /// for performance reasons. Any element insertion will first check to see if the new element isn't already
        /// there and, if it is, it throws an <see cref="DuplicateElementException"/>. The check is active by default,
        /// and you should really not mess with it unless you know what you are doing. The case above is the only
        /// reason this method exists. See <see cref="IsCheckingForDuplicateChildren"/> to see the current state of
        /// this option.
        /// </summary>
        /// <remarks>
        /// If you disable this check, and you insert duplicate children, the whole element hierarchy might get corrupted,
        /// and you will get undefined behaviour.
        /// </remarks>
        /// <param name="shouldEnable">If true, enables the check; if false, disables it.</param>
        public void ToggleDuplicateChildrenCheck(bool shouldEnable)
        {
            IsCheckingForDuplicateChildren = shouldEnable;
        }

        public Element? GetParent()
        {
            return _parent;
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
                    return dimension.Value * (Document?.ContentScale ?? 1);
                case Unit.Pixels:
                    return dimension.Value;
                case Unit.Percent:
                    return dimension.Value * pixelDimensionForPercent / 100f;
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

        public void RequestRedraw()
        {
            Document?.MarkDirty();
        }

        /// <summary>
        /// It will notify the parent that this child modified its layout, and it will call <see cref="RecomputeLayout"/>
        /// for this element if it's the root. It is generally not necessary to call this directly, as changing the
        /// parameters will call this automatically if it's necessary.
        /// </summary>
        /// <remarks>
        /// If this element is not <see cref="Enabled"/> or not inside the document, it does nothing (the same for any
        /// child). If this element is the root, it will simply call <see cref="RecomputeLayout"/>.
        /// </remarks>
        public void MarkLayoutDirty()
        {
            if (!Enabled || !IsInsideDocument)
            {
                return;
            }

            if (this == Document?.Root)
            {
                RecomputeLayout(Document.ViewportSize, Document.ViewportSize, Point2D.Zero);
            }
            else
            {
                //notify parent
                _parent?.ChildLayoutChangedEvent?.Invoke(this, new ChildLayoutChangedEventArgs(IndexInParent));
            }

            Document?.MarkDirty();
        }

        #endregion //Public API
    }
}
