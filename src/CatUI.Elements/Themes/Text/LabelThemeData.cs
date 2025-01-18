using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Themes.Text
{
    public class LabelThemeData : TextElementThemeData
    {
        public LabelThemeData()
        {
        }

        public LabelThemeData(string forState) : base(forState)
        {
        }

        public override LabelThemeData GetDefaultData(string state)
        {
            var data = new LabelThemeData();
            TextElementThemeData baseDefaults = base.GetDefaultData(state);
            data.Background = baseDefaults.Background;
            data.CornerRadius = baseDefaults.CornerRadius;

            data.FillBrush = baseDefaults.FillBrush;
            data.OutlineBrush = baseDefaults.OutlineBrush;
            data.FontSize = baseDefaults.FontSize;
            data.LineHeight = baseDefaults.LineHeight;

            return data;
        }
    }
}
