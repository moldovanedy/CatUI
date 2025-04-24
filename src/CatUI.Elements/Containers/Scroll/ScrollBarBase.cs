using System;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Shapes;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Scroll
{
    public abstract class ScrollBarBase : Element
    {
        #region Properties

        /// <summary>
        /// Specifies whether to have the buttons at the end of the scroll bar. If set to false, the buttons will be
        /// disabled, but will still be inside the document, as this simply changes the <see cref="Element.Enabled"/>
        /// property for the buttons. The default value is true.
        /// </summary>
        public bool ShouldDisplayButtons
        {
            get => _shouldDisplayButtons;
            set
            {
                SetShouldDisplayButtons(value);
                ShouldDisplayButtonsProperty.Value = value;
            }
        }

        private bool _shouldDisplayButtons = true;
        public ObservableProperty<bool> ShouldDisplayButtonsProperty { get; private set; } = new(true);

        private void SetShouldDisplayButtons(bool value)
        {
            _shouldDisplayButtons = value;
            MinusButtonElement.Enabled = value;
            PlusButtonElement.Enabled = value;
        }

        /// <summary>
        /// Represents the behavior of sudden scrolls when the user clicks on the scroll bar free area instead of the
        /// thumb. The default value is <see cref="RepositionBehaviorType.GoToPosition"/>.
        /// </summary>
        public RepositionBehaviorType RepositionBehavior
        {
            get => _repositionBehavior;
            set
            {
                SetRepositionBehavior(value);
                RepositionBehaviorProperty.Value = value;
            }
        }

        private RepositionBehaviorType _repositionBehavior = RepositionBehaviorType.GoToPosition;

        public ObservableProperty<RepositionBehaviorType> RepositionBehaviorProperty { get; private set; }
            = new(RepositionBehaviorType.GoToPosition);

        private void SetRepositionBehavior(RepositionBehaviorType value)
        {
            _repositionBehavior = value;
        }

        #endregion


        #region Functional

        /// <summary>
        /// Represents the dimension of the visible part of the content on the scroll axis. Setting this will
        /// reposition the scroll bar thumb size and position. Generally set when the scroll container is resized.
        /// </summary>
        public float VisibleContentDimension
        {
            get => _visibleContentDimension;
            set
            {
                _visibleContentDimension = value;
                RecalculateThumbSizeAndPosition();
            }
        }

        private float _visibleContentDimension;

        /// <summary>
        /// Represents the dimension of the content on the scroll axis. Must be greater than 0. Setting this will
        /// reposition the scroll bar thumb size and position. 
        /// </summary>
        public float ContentDimension
        {
            get => _contentDimension;
            set
            {
                _contentDimension = Math.Max(value, 0);
                RecalculateThumbSizeAndPosition();
            }
        }

        private float _contentDimension;

        /// <summary>
        /// Represents the current position of the scroll bar thumb. Will be clamped between 0 and
        /// <see cref="ContentDimension"/> minus <see cref="VisibleContentDimension"/>. Setting this will reposition
        /// the scroll bar thumb size and position. This will also be set whenever the user scrolls.
        /// </summary>
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Math.Clamp(value, 0, Math.Max(0, ContentDimension - VisibleContentDimension));
                RecalculateThumbSizeAndPosition();
                ValueChangedEvent?.Invoke(this, _currentValue);
            }
        }

        private float _currentValue;

        /// <summary>
        /// Fired when <see cref="CurrentValue"/> changes, either by your code or by the user. 
        /// </summary>
        public event ScrollBarValueChangedEventHandler? ValueChangedEvent;

        #endregion


        #region Internal elements

        public LinearContainerBase InternalContainer
        {
            get => _internalContainer;
            protected set
            {
                _internalContainer = value;
                if (Children.Count > 0)
                {
                    Children[0] = value;
                }
                else
                {
                    Children.Add(value);
                }

                InternalContent.ItemInsertedEvent += OnChildInserted;
                InternalContent.ItemRemovedEvent += OnChildRemoved;
                InternalContent.ItemMovedEvent += OnChildMoved;
                InternalContent.ListClearingEvent += OnChildrenListClearing;

                value.Children.AddRange(InternalContent);
            }
        }

        private LinearContainerBase _internalContainer = new ColumnContainer();

        protected Element MinusButtonElement
        {
            get => _minusButtonElement!;
            set
            {
                int idx = _minusButtonElement?.IndexInParent ?? -1;
                _minusButtonElement = value;

                //InternalContent will be null in constructor
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (idx != -1 && InternalContent != null)
                {
                    InternalContent[idx] = value;
                }
            }
        }

        private Element? _minusButtonElement;

        protected Element PlusButtonElement
        {
            get => _plusButton!;
            set
            {
                int idx = _plusButton?.IndexInParent ?? -1;
                _plusButton = value;

                //InternalContent will be null in constructor
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (idx != -1 && InternalContent != null)
                {
                    InternalContent[idx] = value;
                }
            }
        }

        private Element? _plusButton;

        /// <summary>
        /// Represents the place where the scroll thumb is free to move. This usually occupies the entire width or height
        /// of the <see cref="InternalContainer"/> (uses container sizing with a growth factor of 1). This always has a
        /// child <see cref="ThumbElement"/> that should never be removed, but you can add other children to this one.
        /// </summary>
        public Element ScrollTrackElement
        {
            get => _scrollTrackElement!;
            set
            {
                int idx = _scrollTrackElement?.IndexInParent ?? -1;
                _scrollTrackElement = value;

                //InternalContent will be null in constructor
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (idx != -1 && InternalContent != null)
                {
                    InternalContent[idx] = value;
                }
            }
        }

        private Element? _scrollTrackElement;

        /// <summary>
        /// The element that is dragged for the scroll and indicates the scroll position. You can set this to any element.
        /// Never set <see cref="Element.Position"/> or <see cref="Element.Layout"/> of this element, as this is
        /// controlled internally and will update on scroll.
        /// </summary>
        public Element ThumbElement
        {
            get => _thumbElement;
            set
            {
                int idx = _thumbElement.IndexInParent;
                _thumbElement = value;
                ScrollTrackElement.Children[idx] = value;
            }
        }

        private Element _thumbElement = new RectangleElement { FillBrush = new ColorBrush(new Color(0x64_B5_F6)) };

        /// <summary>
        /// Contains the minus/plus buttons (if present) and the scroller track. You can add elements to this
        /// but never remove the original ones. If you want modifications to the elements, use properties like
        /// <see cref="ScrollTrackElement"/> instead of directly modifying from here.
        /// </summary>
        public ObservableList<Element> InternalContent { get; private set; }

        #endregion

        private readonly Orientation _scrollOrientation;

        protected ScrollBarBase(Orientation scrollOrientation, Button minusButtonElement, Button plusButtonElement)
        {
            _scrollOrientation = scrollOrientation;
            MinusButtonElement = minusButtonElement;
            PlusButtonElement = plusButtonElement;

            ThumbElement.Layout = new ElementLayout();
            if (scrollOrientation == Orientation.Horizontal)
            {
                ThumbElement.Layout!.SetFixedWidth("50%").SetFixedHeight("100%");
            }
            else
            {
                ThumbElement.Layout!.SetFixedWidth("100%").SetFixedHeight("50%");
            }

            ScrollTrackElement = new Element
            {
                Layout = scrollOrientation == Orientation.Horizontal
                    ? new ElementLayout().SetFixedHeight("100%").SetMinMaxWidth(0, "100%", true)
                    : new ElementLayout().SetFixedWidth("100%").SetMinMaxHeight(0, "100%", true),
                ElementContainerSizing = scrollOrientation == Orientation.Horizontal
                    ? new RowContainerSizing()
                    : new ColumnContainerSizing(),
                Children = [ThumbElement]
            };

            InternalContent =
            [
                minusButtonElement,
                ScrollTrackElement,
                plusButtonElement
            ];

            ShouldDisplayButtonsProperty.ValueChangedEvent += SetShouldDisplayButtons;
            RepositionBehaviorProperty.ValueChangedEvent += SetRepositionBehavior;
        }

        ~ScrollBarBase()
        {
            InternalContent = null!;

            ShouldDisplayButtonsProperty = null!;
            RepositionBehaviorProperty = null!;
        }

        private void RecalculateThumbSizeAndPosition()
        {
            //this should generally never happen
            if (ThumbElement.Layout == null)
            {
                ThumbElement.Layout = new ElementLayout();
                if (_scrollOrientation == Orientation.Horizontal)
                {
                    ThumbElement.Layout.SetFixedHeight("100%");
                }
                else
                {
                    ThumbElement.Layout.SetFixedWidth("100%");
                }
            }

            if (ContentDimension <= VisibleContentDimension)
            {
                if (_scrollOrientation == Orientation.Horizontal)
                {
                    ThumbElement.Layout.SetFixedWidth("100%");
                }
                else
                {
                    ThumbElement.Layout.SetFixedHeight("100%");
                }

                ThumbElement.Position = new Dimension2(0, 0);
                return;
            }

            float percentage = VisibleContentDimension / ContentDimension * 100;
            if (_scrollOrientation == Orientation.Horizontal)
            {
                ThumbElement.Layout.SetFixedWidth(new Dimension(percentage, Unit.Percent));
            }
            else
            {
                ThumbElement.Layout.SetFixedHeight(new Dimension(percentage, Unit.Percent));
            }

            float x = 0, y = 0;
            if (_scrollOrientation == Orientation.Horizontal)
            {
                x = CurrentValue / ContentDimension * 100;
            }
            else
            {
                y = CurrentValue / ContentDimension * 100;
            }

            ThumbElement.Position = new Dimension2(new Dimension(x, Unit.Percent), new Dimension(y, Unit.Percent));
        }

        private void OnChildInserted(object? sender, ObservableListInsertEventArgs<Element> e)
        {
            InternalContainer.Children.Add(e.Item);
        }

        private void OnChildRemoved(object? sender, ObservableListRemoveEventArgs<Element> e)
        {
            InternalContainer.Children.Remove(e.Item);
        }

        private void OnChildMoved(object? sender, ObservableListMoveEventArgs<Element> e)
        {
            InternalContainer.Children.Move(e.Item, e.NewIndex);
        }

        private void OnChildrenListClearing(object? sender, EventArgs e)
        {
            InternalContainer.Children.Clear();
        }
    }


    public delegate void ScrollBarValueChangedEventHandler(object sender, float value);

    /// <summary>
    /// Represents the behavior of sudden scrolls when the user clicks on the scroll bar free area instead of the thumb.
    /// </summary>
    public enum RepositionBehaviorType
    {
        /// <summary>
        /// Will scroll a "page" on the side of click relative to the thumb (e.g. when clicking above the thumb,
        /// it will scroll a page up). This is generally the old behavior.
        /// </summary>
        ScrollPage = 0,

        /// <summary>
        /// Will go to the position where it is clicked. This is generally the modern UI behavior.
        /// </summary>
        GoToPosition = 1
    }
}
