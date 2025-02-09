using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Elements.Containers;

namespace CatUISample.UI
{
    public class RootElement : HBoxContainer
    {
        public override void EnterDocument(object sender)
        {
            Document!.BackgroundColor = new Color(0x21_21_21);

            Children =
            [
                //sidebar
                new VBoxContainer(250) { Background = new ColorBrush(new Color(0x31_31_31)) }
            ];
        }
    }
}
