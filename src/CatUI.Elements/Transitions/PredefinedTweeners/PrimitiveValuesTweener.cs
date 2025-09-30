using CatUI.Data;

namespace CatUI.Elements.Transitions.PredefinedTweeners
{
    public class PrimitiveValuesTweener
    {
        public static Tween.PropertyAnimator<int> IntTweener =>
            (
                ObservableProperty<int> property,
                double interpolationValue,
                int startValue,
                int finalValue,
                bool isRelative) =>
            {
                int actualValue;
                if (isRelative)
                {
                    actualValue = (int)(
                        (startValue * (1.0 - interpolationValue))
                      + ((startValue + finalValue) * interpolationValue));
                }
                else
                {
                    actualValue = (int)((startValue * (1.0 - interpolationValue)) + (finalValue * interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<uint> UintTweener =>
            (
                ObservableProperty<uint> property,
                double interpolationValue,
                uint startValue,
                uint finalValue,
                bool isRelative) =>
            {
                uint actualValue;
                if (isRelative)
                {
                    actualValue = (uint)(
                        (startValue * (1.0 - interpolationValue))
                      + ((startValue + finalValue) * interpolationValue));
                }
                else
                {
                    actualValue = (uint)((startValue * (1.0 - interpolationValue)) + (finalValue * interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<long> LongTweener =>
            (
                ObservableProperty<long> property,
                double interpolationValue,
                long startValue,
                long finalValue,
                bool isRelative) =>
            {
                long actualValue;
                if (isRelative)
                {
                    actualValue = (long)(
                        (startValue * (1.0 - interpolationValue))
                      + ((startValue + finalValue) * interpolationValue));
                }
                else
                {
                    actualValue = (long)((startValue * (1.0 - interpolationValue)) + (finalValue * interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<ulong> UlongTweener =>
            (
                ObservableProperty<ulong> property,
                double interpolationValue,
                ulong startValue,
                ulong finalValue,
                bool isRelative) =>
            {
                ulong actualValue;
                if (isRelative)
                {
                    actualValue = (ulong)(
                        (startValue * (1.0 - interpolationValue))
                      + ((startValue + finalValue) * interpolationValue));
                }
                else
                {
                    actualValue =
                        (ulong)((startValue * (1.0 - interpolationValue)) + (finalValue * interpolationValue));
                }

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<float> FloatTweener =>
            (
                ObservableProperty<float> property,
                double interpolationValue,
                float startValue,
                float finalValue,
                bool isRelative) =>
            {
                float actualValue =
                    isRelative
                        ? float.Lerp(startValue, startValue + finalValue, (float)interpolationValue)
                        : float.Lerp(startValue, finalValue, (float)interpolationValue);

                property.Value = actualValue;
            };

        public static Tween.PropertyAnimator<double> DoubleTweener =>
            (
                ObservableProperty<double> property,
                double interpolationValue,
                double startValue,
                double finalValue,
                bool isRelative) =>
            {
                double actualValue =
                    isRelative
                        ? double.Lerp(startValue, startValue + finalValue, interpolationValue)
                        : double.Lerp(startValue, finalValue, interpolationValue);

                property.Value = actualValue;
            };
    }
}
