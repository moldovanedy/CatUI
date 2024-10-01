namespace CatUI.Data
{
    public struct Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size()
        {
            Width = 0;
            Height = 0;
        }
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}
