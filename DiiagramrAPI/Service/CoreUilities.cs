namespace DiiagramrAPI.Service
{
    public static class CoreUilities
    {
        public static double RoundToNearest(double value, double multiple)
        {
            var rem = value % multiple;
            var result = value - rem;
            if (rem > multiple / 2.0)
            {
                result += multiple;
            }

            return result;
        }
    }
}
