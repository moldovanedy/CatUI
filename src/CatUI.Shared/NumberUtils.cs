namespace CatUI.Shared
{
    public static class NumberUtils
    {
        public static float Lerp(float start, float end, float t)
        {
            return (start * (1 - t)) + (end * t);
        }

        public static double Lerp(double start, double end, double t)
        {
            return (start * (1 - t)) + (end * t);
        }

        public static float Remap(
            float value,
            float previousMin,
            float previousMax,
            float desiredMin,
            float desiredMax)
        {
            return desiredMin + (
                (value - previousMin) *
                (desiredMax - desiredMin) /
                (previousMax - previousMin));
        }

        public static double Remap(
            double value,
            double previousMin,
            double previousMax,
            double desiredMin,
            double desiredMax)
        {
            return desiredMin + (
                (value - previousMin) *
                (desiredMax - desiredMin) /
                (previousMax - previousMin));
        }

        public static int Remap(
            int value,
            int previousMin,
            int previousMax,
            int desiredMin,
            int desiredMax)
        {
            return desiredMin + (
                (value - previousMin) *
                (desiredMax - desiredMin) /
                (previousMax - previousMin));
        }
    }
}