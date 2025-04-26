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
    public class VerticalScrollBar : ScrollBarBase
    {
        public Button UpButtonElement
        {
            get => MinusButtonElement;
            set => MinusButtonElement = value;
        }

        private static Button DefaultUpButton =>
            new(iconElement:
                new GeometricPathElement(
                    "M 2 15 L 10 5 L 18 15",
                    outlineBrush: new ColorBrush(new Color(0xFF_FF_FF)))
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x42_A5_F5))
            };

        public Button DownButtonElement
        {
            get => PlusButtonElement;
            set => PlusButtonElement = value;
        }

        private static Button DefaultDownButton =>
            new(iconElement:
                new GeometricPathElement(
                    "M 2 5 L 10 15 L 18 5",
                    outlineBrush: new ColorBrush(new Color(0xFF_FF_FF)))
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x42_A5_F5))
            };

        public VerticalScrollBar()
            : base(Orientation.Vertical, DefaultUpButton, DefaultDownButton)
        {
            InternalContainer = new ColumnContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
            };
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        public override VerticalScrollBar Duplicate()
        {
            VerticalScrollBar el = new()
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
