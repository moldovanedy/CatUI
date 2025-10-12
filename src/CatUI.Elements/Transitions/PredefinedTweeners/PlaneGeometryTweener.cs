using CatUI.Data;

namespace CatUI.Elements.Transitions.PredefinedTweeners
{
    public class PlaneGeometryTweener
    {
        public static Tween.PropertyAnimator<Point2D> Point2DTweener =>
            (
                ObservableProperty<Point2D> property,
                double interpolationValue,
                Point2D startValue,
                Point2D finalValue,
                bool isRelative) =>
            {
                Point2D actualValue;
                if (isRelative)
                {
                    actualValue = new Point2D(
                        float.Lerp(startValue.X, startValue.X + finalValue.X, (float)interpolationValue),
                        float.Lerp(startValue.Y, startValue.Y + finalValue.Y, (float)interpolationValue));
                }
                else
                {
                    actualValue = new Point2D(
                        float.Lerp(startValue.X, finalValue.X, (float)interpolationValue),
                        float.Lerp(startValue.Y, finalValue.Y, (float)interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<Size> SizeTweener =>
            (
                ObservableProperty<Size> property,
                double interpolationValue,
                Size startValue,
                Size finalValue,
                bool isRelative) =>
            {
                Size actualValue;
                if (isRelative)
                {
                    actualValue = new Size(
                        float.Lerp(
                            startValue.Width,
                            startValue.Width + finalValue.Width,
                            (float)interpolationValue),
                        float.Lerp(
                            startValue.Height,
                            startValue.Height + finalValue.Height,
                            (float)interpolationValue));
                }
                else
                {
                    actualValue = new Size(
                        float.Lerp(startValue.Width, finalValue.Width, (float)interpolationValue),
                        float.Lerp(startValue.Height, finalValue.Height, (float)interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<Rect> RectTweener =>
            (
                ObservableProperty<Rect> property,
                double interpolationValue,
                Rect startValue,
                Rect finalValue,
                bool isRelative) =>
            {
                Rect actualValue;
                if (isRelative)
                {
                    actualValue = new Rect(
                        float.Lerp(startValue.X, startValue.X + finalValue.X, (float)interpolationValue),
                        float.Lerp(startValue.Y, startValue.Y + finalValue.Y, (float)interpolationValue),
                        float.Lerp(
                            startValue.Width,
                            startValue.Width + finalValue.Width,
                            (float)interpolationValue),
                        float.Lerp(
                            startValue.Height,
                            startValue.Height + finalValue.Height,
                            (float)interpolationValue));
                }
                else
                {
                    actualValue = new Rect(
                        float.Lerp(startValue.X, finalValue.X, (float)interpolationValue),
                        float.Lerp(startValue.Y, finalValue.Y, (float)interpolationValue),
                        float.Lerp(startValue.Width, finalValue.Width, (float)interpolationValue),
                        float.Lerp(startValue.Height, finalValue.Height, (float)interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<Dimension> DimensionTweener =>
            (
                ObservableProperty<Dimension> property,
                double interpolationValue,
                Dimension startValue,
                Dimension finalValue,
                bool isRelative) =>
            {
                Dimension actualValue;
                if (isRelative)
                {
                    actualValue = new Dimension(
                        float.Lerp(startValue.Value, startValue.Value + finalValue.Value, (float)interpolationValue),
                        startValue.MeasuringUnit);
                }
                else
                {
                    actualValue = new Dimension(
                        float.Lerp(startValue.Value, finalValue.Value, (float)interpolationValue),
                        startValue.MeasuringUnit);
                }

                property.Value = actualValue;
            };
    }
}
