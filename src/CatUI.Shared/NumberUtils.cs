namespace CatUI.Shared
{
    public static class NumberUtils
    {
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