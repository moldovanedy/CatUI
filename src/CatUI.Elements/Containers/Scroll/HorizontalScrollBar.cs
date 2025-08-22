using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
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
                    StyleClass = "HorizontalScrollBar::PlusButton::Graphic",
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                StyleClass = "HorizontalScrollBar::PlusButton",
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x8C_8C_8C))
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
                    StyleClass = "HorizontalScrollBar::MinusButton::Graphic",
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
                })
            {
                StyleClass = "HorizontalScrollBar::MinusButton",
                Layout = new ElementLayout().SetFixedWidth(20).SetFixedHeight(20),
                Background = new ColorBrush(new Color(0x8C_8C_8C))
            };

        public HorizontalScrollBar()
            : base(Orientation.Horizontal, DefaultLeftButton, DefaultRightButton)
        {
            InternalContainer = new RowContainer
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%")
            };
        }

        public HorizontalScrollBar(HorizontalScrollBar other) : base(other)
        {
            //no need to duplicate plus/minus buttons, the base implementation will do it
            InternalContainer = (LinearContainerBase)other.InternalContainer.Duplicate();
        }

        /// <inheritdoc cref="Element.Duplicate"/>
        public override HorizontalScrollBar Duplicate()
        {
            var el = new HorizontalScrollBar(this);
            DuplicateChildrenUtil(el);
            return el;
        }
    }
}
