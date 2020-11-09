using DiiagramrAPI.Application.ShellCommands;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Diiagramr.Application.Tools
{
    public class NotSeparatorCommandVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type _, object __, CultureInfo ___)
            => value is SeparatorCommand ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            => throw new NotImplementedException();
    }

    public class SeparatorCommandVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type _, object __, CultureInfo ___)
            => value is SeparatorCommand ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            => throw new NotImplementedException();
    }
}