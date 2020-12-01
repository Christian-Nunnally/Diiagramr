using DiiagramrAPI.Application.ShellCommands;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Diiagramr.Application.Tools
{
    /// <summary>
    /// Converts commands to false and separator commands to true.
    /// </summary>
    public class SeparatorCommandVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type _, object __, CultureInfo ___)
            => value is SeparatorCommand ? Visibility.Visible : Visibility.Collapsed;

        /// <inheritdoc/>
        public object ConvertBack(object _, Type __, object ___, CultureInfo ____)
            => throw new NotImplementedException();
    }
}