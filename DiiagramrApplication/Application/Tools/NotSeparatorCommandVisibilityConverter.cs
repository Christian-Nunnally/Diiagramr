using DiiagramrAPI.Application.ShellCommands;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Diiagramr.Application.Tools
{
    /// <summary>
    /// Converts commands to true and separator commands to false.
    /// </summary>
    public class NotSeparatorCommandVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type _, object __, CultureInfo ___)
            => value is SeparatorCommand ? Visibility.Collapsed : Visibility.Visible;

        /// <inheritdoc/>
        public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            => throw new NotImplementedException();
    }
}