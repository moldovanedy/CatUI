using System;
using CatUI.Data;
using CatUI.Data.Brushes;

namespace CatUI.Elements.Transitions.PredefinedTweeners
{
    public class BrushTweener
    {
        public static Tween.PropertyAnimator<Color> ColorTweener =>
            (
                ObservableProperty<Color> property,
                double interpolationValue,
                Color startValue,
                Color finalValue,
                bool isRelative) =>
            {
                Color actualValue =
                    isRelative
                        ? Color.Lerp(startValue, startValue + finalValue, (float)interpolationValue)
                        : Color.Lerp(startValue, finalValue, (float)interpolationValue);

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<ColorBrush> ColorBrushTweener =>
            (
                ObservableProperty<ColorBrush> property,
                double interpolationValue,
                ColorBrush? startValue,
                ColorBrush finalValue,
                bool isRelative) =>
            {
                startValue ??= new ColorBrush(0);
                Color actualColor =
                    isRelative
                        ? Color.Lerp(startValue.Color, startValue.Color + finalValue.Color, (float)interpolationValue)
                        : Color.Lerp(startValue.Color, finalValue.Color, (float)interpolationValue);

                property.Value = new ColorBrush(actualColor);
            };

        //TODO: this is pretty inflexible/hard to maintain and it should be much easier to animate colors
        /// <summary>
        /// A generic brush tweener that animates brushes. Note that the generic of <c>property</c>, as well as
        /// <c>startValue</c> and <c>endValue</c> MUST all be the same brush type (e.g. <see cref="ColorBrush"/>),
        /// otherwise an <see cref="ArgumentException"/> will be thrown.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the generic of <c>property</c>, <c>startValue</c>, and <c>endValue</c> are of a different brush
        /// type.
        /// </exception>
        public static Tween.PropertyAnimator<IBrush> GenericBrushTweener =>
            (
                ObservableProperty<IBrush> property,
                double interpolationValue,
                IBrush? startValue,
                IBrush finalValue,
                bool isRelative) =>
            {
                if (property.Value is ColorBrush)
                {
                    if (startValue is ColorBrush startColorBrush && finalValue is ColorBrush finalColorBrush)
                    {
                        Color actualColor =
                            isRelative
                                ? Color.Lerp(
                                    startColorBrush.Color,
                                    startColorBrush.Color + finalColorBrush.Color,
                                    (float)interpolationValue)
                                : Color.Lerp(
                                    startColorBrush.Color,
                                    finalColorBrush.Color,
                                    (float)interpolationValue);

                        property.Value = new ColorBrush(actualColor);
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Mismatch in brush types: property is ColorBrush, but either {nameof(startValue)} or {nameof(finalValue)} are of another brush type. Changing brush types in tweens is not allowed.",
                            nameof(startValue));
                    }
                }
            };
    }
}
