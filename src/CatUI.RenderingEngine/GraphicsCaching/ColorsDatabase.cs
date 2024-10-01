using System.Collections.Generic;
using SkiaSharp;

namespace CatUI.RenderingEngine.GraphicsCaching
{
    public static class ColorsDatabase
    {
        private static readonly List<SKColor> _colors = new List<SKColor>();

        public static SKColor Get(int index)
        {
            return _colors[index];
        }

        public static bool TryGet(int index, out SKColor? color)
        {
            if (index < _colors.Count && index >= 0)
            {
                color = _colors[index];
                return true;
            }
            else
            {
                color = null;
                return false;
            }
        }

        public static void Add(SKColor color)
        {
            _colors.Add(color);
        }

        public static void PurgeCache()
        {
            _colors.Clear();
        }
    }
}
