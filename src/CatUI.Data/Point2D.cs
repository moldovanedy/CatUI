namespace CatUI.Data
{
    public struct Point2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point2D()
        {
            this.X = 0;
            this.Y = 0;
        }

        public Point2D(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Point2D Zero
        {
            get
            {
                return new Point2D();
            }
        }
    }
}
