using System;
using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Events.Input.Pointer;
using CatUI.Data.Shapes;
using CatUI.Elements.Containers.Linear;
using CatUI.Utils;

namespace CatUI.Elements.Containers.Scroll
{
    /// <summary>
    /// A container that allows content scrolling. You should prefer using this container instead of creating custom
    /// scrolling mechanisms, unless, of course, this container doesn't satisfy your needs. Do NOT set the
    /// <see cref="Element.Children"/> directly or interfere in any way with the first child, as it is used internally.
    /// Modifying it without using <see cref="Content"/> might result in crashes or unexpected behavior in general.
    /// </summary>
    /// <remarks>
    /// <see cref="Element.Children"/> is controlled internally. You should never set or modify any children directly,
    /// but use convenience properties and methods instead. Direct child modification might cause crashes.
    /// </remarks>
    public class ScrollContainer : Container
    {
        /// <inheritdoc cref="Element.Ref"/>
        public new ObjectRef<ScrollContainer>? Ref
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

        private ObjectRef<ScrollContainer>? _ref;

        /// <summary>
        /// Represents the current scroll position. Setting this will trigger an immediate scroll to the specified
        /// position. You should generally not update this in code and instead let the user scroll, unless of course
        /// your use case requires it.
        /// </summary>
        /// <remarks>
        /// To set this, you use <see cref="ScrollTo"/>. Positive Y values will scroll down, positive X values will
        /// scroll right; negatives in the opposite direction.
        /// </remarks>
        public Point2D ScrollPosition
        {
            get => _scrollPosition;
            private set
            {
                SetScrollPosition(value);
                ScrollPositionProperty.Value = value;
            }
        }

        private Point2D _scrollPosition = Point2D.Zero;
        public ObservableProperty<Point2D> ScrollPositionProperty { get; private set; } = new(Point2D.Zero);

        private void SetScrollPosition(Point2D value)
        {
            float endWidth = InternalContentWrapper.Bounds.Width - InternalVisibleContentWrapper.Bounds.Width;
            float endHeight = InternalContentWrapper.Bounds.Height - InternalVisibleContentWrapper.Bounds.Height;

            Point2D allowedEndPoint = new(
                Math.Max(0, Dimension.PxToDp(endWidth, Document?.ContentScale ?? 1f)),
                Math.Max(0, Dimension.PxToDp(endHeight, Document?.ContentScale ?? 1f)));

            float x =
                //keep the old value if scrolling is disabled
                !IsHorizontalScrollEnabled
                    ? _scrollPosition.X
                    : ScrollPastLimits.Item1
                        ? value.X
                        : Math.Clamp(value.X, 0, allowedEndPoint.X);
            float y =
                !IsVerticalScrollEnabled
                    ? _scrollPosition.Y
                    : ScrollPastLimits.Item2
                        ? value.Y
                        : Math.Clamp(value.Y, 0, allowedEndPoint.Y);
            Point2D newValue = new(x, y);

            RepositionScrollBars(true);
            _scrollPosition = newValue;
            InternalContentWrapper.Position = new Dimension2(-newValue.X, -newValue.Y);
        }

        /// <summary>
        /// If true, the content can scroll horizontally. True by default.
        /// </summary>
        /// <remarks>
        /// This refers to the actual element capacity to scroll (i.e. if this is not enabled, even scrolling by
        /// code won't work) rather than the user ability to scroll, which is controlled by <see cref="IsUserScrollable"/>.
        /// When disabled, the scroll position for the horizontal axis will remain frozen. 
        /// </remarks>
        public bool IsHorizontalScrollEnabled
        {
            get => _isHorizontalScrollEnabled;
            set
            {
                SetIsHorizontalScrollEnabled(value);
                IsHorizontalScrollEnabledProperty.Value = value;
            }
        }

        private bool _isHorizontalScrollEnabled = true;
        public ObservableProperty<bool> IsHorizontalScrollEnabledProperty { get; private set; } = new(true);

        private void SetIsHorizontalScrollEnabled(bool value)
        {
            _isHorizontalScrollEnabled = value;
        }

        /// <summary>
        /// If true, the content can scroll vertically. True by default.
        /// </summary>
        /// <remarks>
        /// This refers to the actual element capacity to scroll (i.e. if this is not enabled, even scrolling by
        /// code won't work) rather than the user ability to scroll, which is controlled by <see cref="IsUserScrollable"/>.
        /// When disabled, the scroll position for the vertical axis will remain frozen. 
        /// </remarks>
        public bool IsVerticalScrollEnabled
        {
            get => _isVerticalScrollEnabled;
            set
            {
                SetIsVerticalScrollEnabled(value);
                IsVerticalScrollEnabledProperty.Value = value;
            }
        }

        private bool _isVerticalScrollEnabled = true;
        public ObservableProperty<bool> IsVerticalScrollEnabledProperty { get; private set; } = new(true);

        private void SetIsVerticalScrollEnabled(bool value)
        {
            _isVerticalScrollEnabled = value;
        }

        /// <summary>
        /// This indicates whether the user can scroll the content or not using input. It is sometimes useful to disable
        /// user scrolling when you want to scroll automatically from the code. True by default.
        /// </summary>
        /// <remarks>
        /// This refers only to the user ability to scroll, whereas <see cref="IsVerticalScrollEnabled"/> and
        /// <see cref="IsHorizontalScrollEnabled"/> refer to the actual element capacity to scroll (i.e. if the content
        /// scrolling is not enabled, even scrolling by code won't work).
        /// </remarks>
        public bool IsUserScrollable
        {
            get => _isUserScrollable;
            set
            {
                SetIsUserScrollable(value);
                IsUserScrollableProperty.Value = value;
            }
        }

        private bool _isUserScrollable = true;
        public ObservableProperty<bool> IsUserScrollableProperty { get; private set; } = new(true);

        private void SetIsUserScrollable(bool value)
        {
            _isUserScrollable = value;
        }

        /// <summary>
        /// Determines the behavior of scrolling past limits on both axes. It's a value tuple (i.e. (hor, vert)) where
        /// the first value specifies the scrolling past limits behavior on the horizontal axis and the second value
        /// specifies the scrolling past limits behavior on the vertical axis. False means scrolling past limits is
        /// not allowed, true means it's allowed. The default value is false for both axes (false, false). 
        /// </summary>
        public ValueTuple<bool, bool> ScrollPastLimits
        {
            get => _scrollPastLimits;
            set
            {
                SetScrollPastLimits(value);
                ScrollPastLimitsProperty.Value = value;
            }
        }

        private ValueTuple<bool, bool> _scrollPastLimits = (false, false);

        public ObservableProperty<ValueTuple<bool, bool>> ScrollPastLimitsProperty { get; private set; }
            = new((false, false));

        private void SetScrollPastLimits(ValueTuple<bool, bool> value)
        {
            _scrollPastLimits = value;
        }

        /// <summary>
        /// The actual container content. This is usually a <see cref="Container"/> like <see cref="ColumnContainer"/>,
        /// but it can be any element. Setting this to null will remove the existing content. This is the element
        /// that can be modified directly.
        /// </summary>
        public Element? Content
        {
            get => _content;
            set
            {
                if (_content == null)
                {
                    _content = value;
                    if (value != null)
                    {
                        InternalContentWrapper.Children.Insert(0, value);
                    }
                }
                else if (value == null)
                {
                    InternalContentWrapper.Children.RemoveAt(0);
                    _content = null;
                }
                else
                {
                    InternalContentWrapper.Children[0] = value;
                    _content = value;
                }
            }
        }

        private Element? _content;

        /// <summary>
        /// Returns the internal <see cref="RowContainer"/>. Directly modifying this might result in unexpected behavior.
        /// Try to use other properties as much as possible instead of direct modifications here.
        /// </summary>
        /// <remarks>
        /// The first child is a <see cref="ColumnContainer"/> that has in turn another 2 children: the first one is
        /// the content view with the <see cref="InternalContentWrapper"/>, the second one is the horizontal scroll bar
        /// (or a 0-size <see cref="Element"/> when the scroll bar is not visible); as the second child of this
        /// <see cref="RowContainer"/>, it's the vertical scroll bar (or a 0-size <see cref="Element"/> when the scroll
        /// bar is not visible).
        /// </remarks>
        public RowContainer InternalRowContainer { get; }

        /// <summary>
        /// Returns the internal content wrapper. This will be the parent of <see cref="Content"/>, so modifying its
        /// children directly might result in crashes and unexpected behavior. The scrolling behavior is simply updating
        /// the position of this element, so don't ever try to update the <see cref="Element.Position"/> manually for this
        /// element.
        /// </summary>
        /// <remarks>
        /// It always stretches at least to the size of the ScrollContainer minus the scroll bars, but when content
        /// actually overflows (so scrolling is actually used), it is larger than the ScrollContainer (it can stretch to
        /// infinity).
        /// </remarks>
        public Element InternalContentWrapper { get; }

        /// <summary>
        /// Represents the element that displays <see cref="InternalContentWrapper"/>. Do NOT remove this from the
        /// document. This is generally the size of the scroll container without the scroll bars.
        /// </summary>
        public Element InternalVisibleContentWrapper { get; }

        /// <summary>
        /// Represents the internal horizontal scroll bar. DO NOT remove this from the document. You can modify it,
        /// as by default it's a simple <see cref="HorizontalScrollBar"/>, but with a layout with fixed width of 100% and
        /// a fixed height of 20 dp.
        /// </summary>
        /// <remarks>
        /// When the scroll bar needs to be invisible, this will have its <see cref="Element.Enabled"/> set to false.
        /// </remarks>
        public HorizontalScrollBar InternalHorizontalScrollBar { get; } = new()
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(20)
        };

        /// <summary>
        /// Represents the internal vertical scroll bar. DO NOT remove this from the document. You can modify it,
        /// as by default it's a simple <see cref="VerticalScrollBar"/>, but with a layout with fixed width of 20 dp and
        /// a fixed height of 100%.
        /// </summary>
        /// <remarks>
        /// When the scroll bar needs to be invisible, this will have its <see cref="Element.Enabled"/> set to false.
        /// </remarks>
        public VerticalScrollBar InternalVerticalScrollBar { get; } = new()
        {
            Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight("100%")
        };

        public ScrollContainer(bool isHorizontalScrollEnabled = true, bool isVerticalScrollEnabled = true)
        {
            ObjectRef<Element> internalContentWrapperRef = new();
            ObjectRef<Element> internalVisibleContentWrapperRef = new();

            InternalRowContainer = new RowContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Children =
                [
                    new ColumnContainer
                    {
                        Layout = new ElementLayout().SetFixedHeight("100%"),
                        ElementContainerSizing = new RowContainerSizing(),
                        Children =
                        [
                            new Element
                            {
                                Ref = internalVisibleContentWrapperRef,
                                Layout = new ElementLayout().SetFixedWidth("100%"),
                                ElementContainerSizing = new ColumnContainerSizing(),
                                Children =
                                [
                                    new Element
                                    {
                                        Ref = internalContentWrapperRef,
                                        Layout =
                                            new ElementLayout()
                                                .SetMinMaxWidth("100%", Dimension.Unset)
                                                .SetMinMaxHeight("100%", Dimension.Unset)
                                    }
                                ]
                            },
                            InternalHorizontalScrollBar
                        ]
                    },
                    InternalVerticalScrollBar
                ]
            };

            InternalContentWrapper = internalContentWrapperRef.Value!;
            InternalContentWrapper.Ref = null;
            InternalVisibleContentWrapper = internalVisibleContentWrapperRef.Value!;
            InternalVisibleContentWrapper.Ref = null;

            Children.Add(InternalRowContainer);
            IsHorizontalScrollEnabled = isHorizontalScrollEnabled;
            IsVerticalScrollEnabled = isVerticalScrollEnabled;

            ScrollPositionProperty.ValueChangedEvent += SetScrollPosition;
            IsHorizontalScrollEnabledProperty.ValueChangedEvent += SetIsHorizontalScrollEnabled;
            IsVerticalScrollEnabledProperty.ValueChangedEvent += SetIsVerticalScrollEnabled;
            IsUserScrollableProperty.ValueChangedEvent += SetIsUserScrollable;
            ScrollPastLimitsProperty.ValueChangedEvent += SetScrollPastLimits;
        }

        ~ScrollContainer()
        {
            ScrollPositionProperty = null!;
            IsHorizontalScrollEnabledProperty = null!;
            IsVerticalScrollEnabledProperty = null!;
            IsUserScrollableProperty = null!;
            ScrollPastLimitsProperty = null!;
        }

        protected override void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheel(sender, e);

            if (IsUserScrollable)
            {
                ScrollPosition = new Point2D(ScrollPosition.X + e.DeltaX, ScrollPosition.Y + e.DeltaY);
            }
        }

        public override Size RecomputeLayout(
            Size parentSize,
            Size parentMaxSize,
            Point2D parentAbsolutePosition,
            float? parentEnforcedWidth = null,
            float? parentEnforcedHeight = null)
        {
            Size totalContentSize = new(
                Dimension.PxToDp(InternalContentWrapper.Bounds.Width, Document?.ContentScale ?? 1f),
                Dimension.PxToDp(InternalContentWrapper.Bounds.Height, Document?.ContentScale ?? 1f));
            Size visibleContentSize = new(
                Dimension.PxToDp(InternalVisibleContentWrapper.Bounds.Width, Document?.ContentScale ?? 1f),
                Dimension.PxToDp(InternalVisibleContentWrapper.Bounds.Height, Document?.ContentScale ?? 1f));

            //if the scroll position makes the content overshoot its bounds, we get an undesired effect; this prevents
            //that by setting the scroll position at the maximum value that doesn't make the content overshoot its bounds
            if (!ScrollPastLimits.Item1 && ScrollPosition.X + visibleContentSize.Width > totalContentSize.Width)
            {
                ScrollPosition = new Point2D(totalContentSize.Width - visibleContentSize.Width, ScrollPosition.Y);
            }

            if (!ScrollPastLimits.Item2 && ScrollPosition.Y + visibleContentSize.Height > totalContentSize.Height)
            {
                ScrollPosition = new Point2D(ScrollPosition.X, totalContentSize.Height - visibleContentSize.Height);
            }

            //TODO: this should be before the calculations above, but for some reason it completely breaks the layout;
            //this needs to be fixed
            Size result = base.RecomputeLayout(
                parentSize,
                parentMaxSize,
                parentAbsolutePosition,
                parentEnforcedWidth,
                parentEnforcedHeight);
            RepositionScrollBars(true, true);
            return result;
        }

        #region Public API

        /// <summary>
        /// Sets <see cref="ScrollPosition"/> to the specified position if possible, according to the scrolling behavior,
        /// from properties like <see cref="IsHorizontalScrollEnabled"/>, <see cref="IsVerticalScrollEnabled"/> or
        /// <see cref="ScrollPastLimits"/>, so the value given here might not be the same as the final
        /// <see cref="ScrollPosition"/>.
        /// </summary>
        /// <param name="position">
        /// The position you want to scroll to. Remember: positive Y values will scroll down, positive X values will
        /// scroll right; negatives in the opposite direction.
        /// </param>
        public void ScrollTo(Point2D position)
        {
            ScrollPosition = position;
        }

        #endregion

        private void RepositionScrollBars(bool isPositionChanged = false, bool isDimensionChanged = false)
        {
            if (isPositionChanged)
            {
                InternalHorizontalScrollBar.CurrentValue = ScrollPosition.X;
                InternalVerticalScrollBar.CurrentValue = ScrollPosition.Y;
            }

            if (isDimensionChanged)
            {
                InternalHorizontalScrollBar.ContentDimension =
                    Dimension.PxToDp(InternalContentWrapper.Bounds.Width, Document?.ContentScale ?? 1f);
                InternalVerticalScrollBar.ContentDimension =
                    Dimension.PxToDp(InternalContentWrapper.Bounds.Height, Document?.ContentScale ?? 1f);

                InternalHorizontalScrollBar.VisibleContentDimension =
                    Dimension.PxToDp(InternalVisibleContentWrapper.Bounds.Width, Document?.ContentScale ?? 1f);
                InternalVerticalScrollBar.VisibleContentDimension =
                    Dimension.PxToDp(InternalVisibleContentWrapper.Bounds.Height, Document?.ContentScale ?? 1f);
            }
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        /// <remarks>Will also duplicate the <see cref="Content"/> if one is given.</remarks>
        public override ScrollContainer Duplicate()
        {
            return new ScrollContainer
            {
                ScrollPosition = ScrollPosition,
                IsHorizontalScrollEnabled = IsHorizontalScrollEnabled,
                IsVerticalScrollEnabled = IsVerticalScrollEnabled,
                IsUserScrollable = IsUserScrollable,
                ScrollPastLimits = ScrollPastLimits,
                Content = Content?.Duplicate(),
                //
                State = State,
                Position = Position,
                Background = Background.Duplicate(),
                ClipPath = (ClipShape?)ClipPath?.Duplicate(),
                ClipType = ClipType,
                Visible = Visible,
                Enabled = Enabled,
                ElementContainerSizing = (ContainerSizing?)ElementContainerSizing?.Duplicate(),
                Layout = Layout
            };
        }
    }
}
