using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Containers;
using CatUI.Elements.Text;

namespace CatUISample.UI.Pages.Layout
{
    public class RowContainersExamples : ColumnContainer
    {
        public RowContainersExamples()
        {
            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(100);
        }

        protected override void EnterDocument(object sender)
        {
            base.EnterDocument(sender);

            Children =
            [
                new TextBlock("RowContainer examples", TextAlignmentType.Center)
                {
                    Layout =
                        new ElementLayout()
                            .SetMinMaxWidth(0, "100%", true)
                            .SetMinMaxHeight(32, 40),
                    FontSize = 32,
                    TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                }
            ];
        }
    }
}
