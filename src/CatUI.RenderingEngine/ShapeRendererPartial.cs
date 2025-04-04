using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Managers;
using SkiaSharp;

namespace CatUI.RenderingEngine
{
    public partial class Renderer
    {
        /// <summary>
        /// Draws a rect directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw a filled rectangle, for only drawing the outline (aka stroke), use
        /// <see cref="DrawRectOutline"/>.
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="fillBrush">The brush to use to paint the rect.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless
        /// of the measuring unit.
        /// </param>
        public void DrawRect(Rect rect, IBrush fillBrush, CornerInset? roundedCorners = null)
        {
            SKPaint paint = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint);

            if (roundedCorners != null && roundedCorners.HasNonTrivialValues)
            {
                var roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);
                Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                Canvas?.DrawRect(rect, paint);
            }
        }

        /// <summary>
        /// Draws a rect outline (outline) directly on the canvas. The corners values are interpreted as pixels regardless of the measuring unit.
        /// Leaving corners to default will draw a sharp rectangle, with no rounded corners.
        /// </summary>
        /// <remarks>
        /// This method can only draw an outlined (stroke) rectangle, for drawing the rectangle as filled, use
        /// <see cref="DrawRect"/>.
        /// </remarks>
        /// <param name="rect">The direct pixel measurements of the rect.</param>
        /// <param name="outlineBrush">The brush to use to paint the rect.</param>
        /// <param name="outlineParams">The parameters that define the outline.</param>
        /// <param name="roundedCorners">
        /// Optionally provide the details for rounded corners. The corners values are interpreted as pixels regardless
        /// of the measuring unit.
        /// </param>
        public void DrawRectOutline(Rect rect, IBrush outlineBrush, OutlineParams outlineParams,
            CornerInset? roundedCorners = null)
        {
            SKPaint paint = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint, PaintMode.Outline, outlineParams: outlineParams);

            if (roundedCorners != null && roundedCorners.HasNonTrivialValues)
            {
                var roundRect = new SKRoundRect();
                SKPoint[] radii = SetupRectCorners(roundedCorners);
                roundRect.SetRectRadii(rect, radii);

                Canvas?.DrawRoundRect(roundRect, paint);
            }
            else
            {
                Canvas?.DrawRect(rect, paint);
            }
        }

        public void DrawEllipse(Point2D center, float rx, float ry, IBrush fillBrush)
        {
            SKPaint paint = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(paint);
            Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
        }

        public void DrawEllipseOutline(Point2D center, float rx, float ry, IBrush outlineBrush,
            OutlineParams outlineParams)
        {
            SKPaint paint = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(
                paint,
                PaintMode.Outline,
                outlineParams: outlineParams);

            Canvas?.DrawOval(center.X, center.Y, rx, ry, paint);
        }

        public void DrawPath(SKPath skiaPath, IBrush fillBrush)
        {
            if (fillBrush.IsSkippable)
            {
                return;
            }

            SKPaint fillPaint = fillBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(
                fillPaint);

            Canvas?.DrawPath(skiaPath, fillPaint);
        }

        public void DrawPathOutline(SKPath skiaPath, IBrush outlineBrush, OutlineParams outlineParams)
        {
            if (outlineBrush.IsSkippable || outlineParams.OutlineWidth == 0)
            {
                return;
            }

            SKPaint outlinePaint = outlineBrush.ToSkiaPaint();
            PaintManager.ModifyPaint(
                outlinePaint,
                PaintMode.Outline,
                outlineParams: outlineParams);

            Canvas?.DrawPath(skiaPath, outlinePaint);
        }


        private static SKPoint[] SetupRectCorners(CornerInset roundedCorners)
        {
            SKPoint[] radii = new SKPoint[4];

            if (roundedCorners.TopLeftEllipse.IsUnset())
            {
                if (roundedCorners.TopLeftRadius.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.TopLeftRadius.Value, roundedCorners.TopLeftRadius.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.TopLeftEllipse.X.Value,
                        roundedCorners.TopLeftEllipse.Y.Value);
            }

            if (roundedCorners.TopRightEllipse.IsUnset())
            {
                if (roundedCorners.TopRightRadius.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.TopRightRadius.Value, roundedCorners.TopRightRadius.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.TopRightEllipse.X.Value,
                        roundedCorners.TopRightEllipse.Y.Value);
            }

            if (roundedCorners.BottomRightEllipse.IsUnset())
            {
                if (roundedCorners.BottomRightRadius.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.BottomRightRadius.Value,
                        roundedCorners.BottomRightRadius.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.BottomRightEllipse.X.Value,
                        roundedCorners.BottomRightEllipse.Y.Value);
            }

            if (roundedCorners.BottomLeftEllipse.IsUnset())
            {
                if (roundedCorners.BottomLeftRadius.IsUnset())
                {
                    radii[0] = new SKPoint(0, 0);
                }
                else
                {
                    radii[0] = new SKPoint(roundedCorners.BottomLeftRadius.Value,
                        roundedCorners.BottomLeftRadius.Value);
                }
            }
            else
            {
                radii[0] =
                    new SKPoint(
                        roundedCorners.BottomLeftEllipse.X.Value,
                        roundedCorners.BottomLeftEllipse.Y.Value);
            }

            return radii;
        }
    }
}
