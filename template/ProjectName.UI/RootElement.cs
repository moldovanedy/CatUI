using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Enums;
using CatUI.Elements.Containers;
using CatUI.Elements.Text;
using CatUI.Utils;

namespace ProjectName.UI
{
    //VBoxContainer arranges its children in a vertical stack, you can derive (almost) any CatUI element, making the
    //creation of custom and reusable elements very easy
    public class RootElement : VBoxContainer
    {
        //you can use an ObjectRef<Element> to acces an element in another context (see below)
        private readonly ObjectRef<Label>? _labelRef = new();

        //called when this element is added to the window's document
        public override void EnterDocument(object sender)
        {
            //set this element's children
            Children =
            [
                new Label(
                    "Hello World from CatUI!",
                    preferredWidth: "100%",
                    preferredHeight: 20)
                {
                    TextAlignment = TextAlignmentType.Center,
                    FontSize = 16,
                    Background = new ColorBrush(new Color(0xff_00_ff))
                },
                new Label("", preferredWidth: "100%")
                {
                    Ref = _labelRef,
                    TextAlignment = TextAlignmentType.Center,
                    Background = new ColorBrush(new Color(0xff_ff_00))
                }
            ];

            //you can access the label here!
            _labelRef!.Value!.Text = "Current time: ";
        }
    }
}
