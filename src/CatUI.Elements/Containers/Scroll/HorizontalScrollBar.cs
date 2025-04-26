using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Shapes;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Shapes;

namespace CatUI.Elements.Containers.Scroll
{
    public class HorizontalScrollBar : ScrollBarBase
    {
        public Button LeftButtonElement
        {
            get => MinusButtonElement;
            set => MinusButtonElement = value;
        }

        private static Button DefaultLeftButton =>
            new(iconElement:
                new GeometricPathElement(
                    "M 15 2 L 5 10 L 15 18",
                    outlineBrush: new ColorBrush(new Color(0xFF_FF_FF)))
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x42_A5_F5))
            };

        public Button RightButtonElement
        {
            get => PlusButtonElement;
            set => PlusButtonElement = value;
        }

        private static Button DefaultRightButton =>
            new(iconElement:
                new GeometricPathElement(
                    "M 5 2 L 15 10 L 5 18",
                    outlineBrush: new ColorBrush(new Color(0xFF_FF_FF)))
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x42_A5_F5))
            };

        public HorizontalScrollBar()
            : base(Orientation.Horizontal, DefaultLeftButton, DefaultRightButton)
        {
            InternalContainer = new RowContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
            };
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        public override HorizontalScrollBar Duplicate()
        {
            HorizontalScrollBar el = new()
            {
                ShouldDisplayButtons = ShouldDisplayButtons,
                RepositionBehavior = RepositionBehavior,
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

            el.ToggleDuplicateChildrenCheck(false);
            foreach (Element child in Children)
            {
                el.Children.Add(child.Duplicate());
            }
            el.ToggleDuplicateChildrenCheck(true);

            return el;
        }
    }
}
