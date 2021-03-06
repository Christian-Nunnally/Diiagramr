﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiiagramrApplication.Converters
{
    /// <summary>
    /// Converts a value to a <see cref="Visibility.Visible"/> and null to <see cref="Visibility.Hidden"/>.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}