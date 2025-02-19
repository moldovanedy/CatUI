using CatUI.Data.Enums;

namespace CatUI.Data
{
    public readonly struct OutlineParams
    {
        /// <summary>
        /// Specifies the width of the outline. Default is 1.
        /// </summary>
        public float OutlineWidth { get; } = 1;

        /// <summary>
        /// Controls how the ends of the lines are treated. Default is <see cref="LineCapType.Butt"/>.
        /// </summary>
        public LineCapType LineCap { get; } = LineCapType.Butt;

        /// <summary>
        /// Controls how the lines are joined. Default is <see cref="LineJoinType.Miter"/>.
        /// </summary>
        public LineJoinType LineJoin { get; } = LineJoinType.Miter;

        /// <summary>
        /// Controls the limit of the line joins' extension when <see cref="LineJoin"/> is set to 
        /// <see cref="LineJoinType.Miter"/>. Default is 10.
        /// </summary>
        public float MiterLimit { get; } = 10;

        public OutlineParams() { }

        public OutlineParams(
            float outlineWidth = 1,
            LineCapType lineCap = LineCapType.Butt,
            LineJoinType lineJoin = LineJoinType.Miter,
            float miterLimit = 10)
        {
            OutlineWidth = outlineWidth;
            LineCap = lineCap;
            LineJoin = lineJoin;
            MiterLimit = miterLimit;
        }

        /// <inheritdoc cref="CatObject.Duplicate"/>
        public OutlineParams Duplicate()
        {
            return new OutlineParams(1, LineCap, LineJoin, MiterLimit);
        }
    }
}
