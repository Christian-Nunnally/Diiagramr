using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiiagramrApplication.Converters
{
    /// <summary>
    /// Converts empty collections to <see cref="Visibility.Hidden"/> and non empty collection to <see cref="Visibility.Visible"/>.
    /// </summary>
    public class CollectionToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ICollection collection && collection.Count != 0
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}