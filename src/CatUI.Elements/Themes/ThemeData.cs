using System;

namespace CatUI.Elements.Themes
{
    public abstract class ThemeData
    {
        public string ForState { get; }

        public ThemeData(string forState)
        {
            ForState = forState;
        }

        public ThemeData()
        {
            ForState = "normal";
        }
    }
}