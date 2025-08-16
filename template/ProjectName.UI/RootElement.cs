using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Text;
using CatUI.Utils;

namespace ProjectName.UI
{
    //ColumnContainer arranges its children in a vertical stack, you can derive (almost) any CatUI element, making the
    //creation of custom and reusable elements very easy
    public class RootElement : ColumnContainer
    {
        //you can use an ObjectRef<Element> to acces an element in another context (see below)
        private readonly ObjectRef<Label>? _labelRef = new();

        //called when this element is added to the window's document
        protected override void EnterDocument(object sender)
        {
            //set the root theme
            ThemeOverride = ThemeSetup.GetTheme();

            //set this element's children
            Children =
            [
                new Label(
                    "Hello World from CatUI!")
                {
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(20),
                    TextAlignment = TextAlignmentType.Center,
                    FontSize = 16,
                    Background = new ColorBrush(new Color(0xff_00_ff))
                },
                new Label("")
                {
                    Ref = _labelRef,
                    Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight(20),
                    TextAlignment = TextAlignmentType.Center,
                    Background = new ColorBrush(new Color(0xff_ff_00))
                }
            ];

            //you can access the text block here!
            _labelRef!.Value!.Text = "Current time: ";
        }
    }
}
