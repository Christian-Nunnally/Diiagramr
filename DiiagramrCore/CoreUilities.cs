namespace DiiagramrCore
{
    using System;
    using System.Windows.Media;

    public static class CoreUilities
    {
        public static double RoundToNearest(double value, double multiple)
        {
            return value > 0 ?
                Math.Round(value / multiple) * multiple :
                Math.Round(Math.Abs(value) / multiple) * -multiple;
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R / 255.0f;
            float green = color.G / 255.0f;
            float blue = color.B / 255.0f;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = ((1f - red) * correctionFactor) + red;
                green = ((1f - green) * correctionFactor) + green;
                blue = ((1f - blue) * correctionFactor) + blue;
            }

            return Color.FromArgb(color.A, (byte)(red * 255.0), (byte)(green * 255.0), (byte)(blue * 255.0));
        }
    }
}
