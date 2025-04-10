using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers;
using CatUI.Elements.Text;

namespace CatUISample.UI.Pages
{
    public class MainPage : ColumnContainer
    {
        public MainPage()
        {
            Layout = new ElementLayout().SetFixedWidth("100%");
        }

        protected override void EnterDocument(object sender)
        {
            base.EnterDocument(sender);

            Children =
            [
                new TextBlock("CatUI Sample", TextAlignmentType.Center)
                {
                    Layout = new ElementLayout().SetMinMaxWidth(0, "100%", true),
                    FontSize = 32,
                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                }
            ];
        }
    }
}
