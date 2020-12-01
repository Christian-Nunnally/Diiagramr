using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiiagramrApplication.Converters
{
    /// <summary>
    /// Converts empty collections to <see cref="Visibility.Visible"/> and non empty collection to <see cref="Visibility.Hidden"/>.
    /// </summary>
    public class InvertedCollectionToVisibilityConverter : IValueConverter
    {
        private readonly CollectionToVisibilityConverter converter = new CollectionToVisibilityConverter();

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)converter.Convert(value, targetType, parameter, culture) == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}