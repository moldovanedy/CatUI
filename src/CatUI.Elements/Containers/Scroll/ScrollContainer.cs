using CatUI.Data;
using CatUI.Data.Containers;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
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
        /// The actual container content. This is usually a <see cref="Container"/> like <see cref="ColumnContainer"/>,
        /// but it can be any element. Setting this to null will remove the existing content.
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

                //this will control the scrolling
                //InternalContentWrapper.Position = new Dimension2(0, -200);
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

        public ScrollContainer()
        {
            InternalContentWrapper = new Element
            {
                Layout =
                    new ElementLayout()
                        .SetMinMaxWidth("100%", Dimension.Unset)
                        .SetMinMaxHeight("100%", Dimension.Unset)
            };

            InternalRowContainer = new RowContainer
            {
                Id = "First",
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
                                Layout = new ElementLayout().SetFixedWidth("100%"),
                                ElementContainerSizing = new ColumnContainerSizing(),
                                Children = [InternalContentWrapper]
                            },
                            //horizontal scroll bar
                            new Element()
                        ]
                    },
                    //vertical scroll bar
                    new Element()
                ]
            };

            Children.Add(InternalRowContainer);
        }

        public override ScrollContainer Duplicate()
        {
            return new ScrollContainer
            {
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
