using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Shapes
{
    public interface IGeometricShape
    {
        public IBrush FillBrush { get; set; }
        public IBrush OutlineBrush { get; set; }
        public OutlineParams OutlineParameters { get; set; }
    }
}