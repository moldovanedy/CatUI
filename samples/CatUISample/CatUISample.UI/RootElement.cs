using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Elements.Containers;

namespace CatUISample.UI
{
    public class RootElement : RowContainer
    {
        public override void EnterDocument(object sender)
        {
            Document!.BackgroundColor = new Color(0x21_21_21);

            Children =
            [
                //sidebar
                new ColumnContainer
                {
                    Layout = new ElementLayout().SetFixedWidth(250).SetFixedHeight("100%"),
                    Background = new ColorBrush(new Color(0x31_31_31))
                }
            ];
        }
    }
}
