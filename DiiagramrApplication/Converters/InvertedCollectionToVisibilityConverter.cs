using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiiagramrApplication.Converters
{
    public class InvertedCollectionToVisibilityConverter : IValueConverter
    {
        private readonly CollectionToVisibilityConverter converter = new CollectionToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)converter.Convert(value, targetType, parameter, culture) == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}