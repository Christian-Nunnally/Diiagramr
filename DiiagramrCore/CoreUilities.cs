using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace DiiagramrCore
{
    /// <summary>
    /// A collection of super reusable functions used across multiple projects.
    /// </summary>
    public static class CoreUilities
    {
        /// <summary>
        /// Rounds the given value to the nearest given multiple.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <param name="multiple">The multiple to round to.</param>
        /// <returns><paramref name="value"/> rounded to the nearest <paramref name="multiple"/>.</returns>
        public static double RoundToNearest(double value, double multiple)
        {
            return value > 0 ?
                Math.Round(value / multiple) * multiple :
                Math.Round(Math.Abs(value) / multiple) * -multiple;
        }

        /// <summary>
        /// Adjusts the brightness of the given color by the given precent.
        /// </summary>
        /// <param name="color">The color to adjust.</param>
        /// <param name="correctionFactor">The amount to adjust the brightness by.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts an <see cref="IEnumerable"/> to a <see cref="List"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="items">The enumerable.</param>
        /// <returns>The enumerable as a list.</returns>
        public static IList ConvertToList(this IEnumerable items, Type targetType)
        {
            var method = typeof(CoreUilities).GetMethod(
                "ConvertToList",
                new[] { typeof(IEnumerable) });
            var generic = method.MakeGenericMethod(targetType);
            return (IList)generic.Invoke(null, new[] { items });
        }

        /// <summary>
        /// Opens the given URL in the default browser.
        /// </summary>
        /// <param name="url"></param>
        public static void GoToSite(string url)
        {
            Process.Start("cmd", $"/C start {url}");
        }
    }
}