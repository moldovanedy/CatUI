using System;

namespace CatUI.Elements.Transitions
{
    public class Easing
    {
        /// <summary>
        /// Defines a custom easing function that can provide more advanced customization compared to the ones from
        /// <see cref="EasingType"/>.
        /// </summary>
        /// <param name="progress">
        /// A value that goes linearly from 0 to 1 indicating the transition progress. This is the division of the
        /// elapsed time to the total transition time.
        /// </param>
        /// <returns>
        /// The interpolation value, where 0 means the starting value of the property you want to animate, and 1 means
        /// the end value. You can also return a value outside the [0, 1] range.
        /// </returns>
        public delegate double CustomEasing(double progress);

        public EasingType Type { get; set; }

        /// <summary>
        /// This is only relevant when <see cref="Type"/> is <see cref="EasingType.Custom"/>. If the type is
        /// <see cref="EasingType.Custom"/>, <see cref="GetValue"/> will always return 1.
        /// </summary>
        /// <seealso cref="CustomEasing"/>
        public CustomEasing? CustomEasingFunction { get; set; }

        public Easing(EasingType type = EasingType.Linear)
        {
            Type = type;
        }

        public Easing(CustomEasing customEasing)
        {
            Type = EasingType.Custom;
            CustomEasingFunction = customEasing;
        }

        /// <summary>
        /// The most important function. Given the animation progress (going linearly from 0 to 1), returns the
        /// interpolation value, where 0 means the start value of the animated property, while 1 means the end value.
        /// The returned value can also fall outside the [0, 1] range.
        /// </summary>
        /// <param name="prog">
        /// A value that goes linearly from 0 to 1 indicating the transition progress. This is the division of the
        /// elapsed time to the total transition time.
        /// </param>
        /// <returns>
        /// The interpolation value, where 0 means the starting value of the property you want to animate, and 1 means
        /// the end value. You can also return a value outside the [0, 1] range.
        /// </returns>
        public double GetValue(double prog)
        {
            const double EPSILON = 0.001;

            //these are needed for EasingType.Back* types
            const double C1 = 1.70158;
            const double C2 = C1 * 1.525;
            const double C3 = C1 + 1;

            //most formulas are taken from https://easings.net/
            switch (Type)
            {
                case EasingType.Linear:
                    return prog;

                case EasingType.SineIn:
                    return 1 - Math.Cos(prog * Math.PI / 2.0);
                case EasingType.SineOut:
                    return Math.Sin(prog * Math.PI / 2.0);
                case EasingType.SineInOut:
                    return -(Math.Cos(Math.PI * prog) - 1.0) / 2.0;

                case EasingType.QuadraticIn:
                    return prog * prog;
                case EasingType.QuadraticOut:
                    return 1.0 - ((1.0 - prog) * (1.0 - prog));
                case EasingType.QuadraticInOut:
                    return
                        prog < 0.5
                            ? 2.0 * prog * prog
                            : 1.0 - (Math.Pow((-2.0 * prog) + 2.0, 2.0) / 2.0);

                case EasingType.CubicIn:
                    return prog * prog * prog;
                case EasingType.CubicOut:
                    return 1.0 - Math.Pow(1.0 - prog, 3.0);
                case EasingType.CubicInOut:
                    return
                        prog < 0.5
                            ? 4.0 * prog * prog * prog
                            : 1.0 - (Math.Pow((-2.0 * prog) + 2.0, 3.0) / 2.0);

                case EasingType.QuarticIn:
                    return prog * prog * prog * prog;
                case EasingType.QuarticOut:
                    return 1.0 - Math.Pow(1.0 - prog, 4.0);
                case EasingType.QuarticInOut:
                    return
                        prog < 0.5
                            ? 8.0 * prog * prog * prog * prog
                            : 1.0 - (Math.Pow((-2.0 * prog) + 2.0, 4.0) / 2.0);

                case EasingType.ExponentialIn:
                    return prog < EPSILON ? 0.0 : Math.Pow(2.0, (10.0 * prog) - 10.0);
                case EasingType.ExponentialOut:
                    return
                        Math.Abs(prog - 1.0) < EPSILON
                            ? 1.0
                            : 1.0 - Math.Pow(2.0, -10.0 * prog);
                case EasingType.ExponentialInOut:
                    {
                        if (prog < EPSILON)
                        {
                            return 0;
                        }

                        if (Math.Abs(prog - 1.0) < EPSILON)
                        {
                            return 1;
                        }

                        if (prog < 0.5)
                        {
                            return Math.Pow(2.0, (20.0 * prog) - 10.0) / 2.0;
                        }

                        return (2.0 - Math.Pow(2.0, (-20.0 * prog) + 10.0)) / 2.0;
                    }

                case EasingType.CircularIn:
                    return 1.0 - Math.Sqrt(1.0 - Math.Pow(prog, 2.0));
                case EasingType.CircularOut:
                    return Math.Sqrt(1.0 - Math.Pow(prog - 1.0, 2.0));
                case EasingType.CircularInOut:
                    return
                        prog < 0.5
                            ? (1.0 - Math.Sqrt(1.0 - Math.Pow(2.0 * prog, 2.0))) / 2.0
                            : (Math.Sqrt(1.0 - Math.Pow((-2.0 * prog) + 2.0, 2.0)) + 1.0) / 2.0;

                case EasingType.BackIn:
                    return (C3 * prog * prog * prog) - (C1 * prog * prog);
                case EasingType.BackOut:
                    return 1.0 + (C3 * Math.Pow(prog - 1.0, 3.0)) + (C1 * Math.Pow(prog - 1.0, 2.0));
                case EasingType.BackInOut:
                    return
                        prog < 0.5
                            ? Math.Pow(2.0 * prog, 2.0) * (((C2 + 1.0) * 2.0 * prog) - C2) / 2.0
                            : ((Math.Pow((2.0 * prog) - 2.0, 2.0) * (((C2 + 1.0) * ((prog * 2.0) - 2.0)) + C2)) + 2.0) /
                              2.0;

                case EasingType.Custom:
                    return CustomEasingFunction?.Invoke(prog) ?? 1;
                default:
                    return double.NaN;
            }
        }


        public enum EasingType
        {
            /// <summary>
            /// No easing; values change at a constant rate of speed. This is the default value.
            /// </summary>
            Linear = 0,

            SineIn = 0b01,
            SineOut = 0b10,
            SineInOut = 0b11,

            QuadraticIn = 0b01_00,
            QuadraticOut = 0b10_00,
            QuadraticInOut = 0b11_00,

            CubicIn = 0b01_00_00,
            CubicOut = 0b10_00_00,
            CubicInOut = 0b11_00_00,

            QuarticIn = 0b01_00_00_00,
            QuarticOut = 0b10_00_00_00,
            QuarticInOut = 0b11_00_00_00,

            ExponentialIn = 0b01 << 8,
            ExponentialOut = 0b10 << 8,
            ExponentialInOut = 0b11 << 8,

            CircularIn = 0b01_00 << 8,
            CircularOut = 0b10_00 << 8,
            CircularInOut = 0b11_00 << 8,

            BackIn = 0b01_00_00 << 8,
            BackOut = 0b10_00_00 << 8,
            BackInOut = 0b11_00_00 << 8,

            /// <summary>
            /// A custom easing function, defined by <see cref="Easing.CustomEasingFunction"/>. If that is null,
            /// <see cref="GetValue"/> will always return 1.
            /// </summary>
            Custom = int.MaxValue
        }
    }
}
