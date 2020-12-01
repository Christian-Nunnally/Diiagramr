using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// An adorner that allows users to directly edit enum type data on a <see cref="Terminal"/>.
    /// </summary>
    public class DirectEditEnumAdorner : DirectEditAdorner
    {
        private readonly StackPanel _stackPanel;
        private readonly Border _border;

        /// <summary>
        /// Creates a new instance of <see cref="DirectEditEnumAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element the adorner should attach to.</param>
        /// <param name="adornedTerminal">The terminal the adorner can edit the data of.</param>
        public DirectEditEnumAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement, adornedTerminal)
        {
            if (adornedTerminal == null || !adornedTerminal.Model.Type.IsEnum)
            {
                return;
            }

            var options = Enum.GetNames(adornedTerminal.Model.Type);
            var data = adornedTerminal.Model?.Data;
            var currentValue = data != null
                ? Enum.Parse(adornedTerminal.Model.Type, data.ToString())
                : null;

            _border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(16, 16, 16)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 96, 96)),
                BorderThickness = new Thickness(1),
                Height = options.Length * 25,
            };

            _stackPanel = new StackPanel
            {
                Height = options.Length * 25,
            };
            var maxWidth = 0.0;
            foreach (var option in options)
            {
                var optionLabel = new Label
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                    Height = 25,
                    Width = 100,
                    Content = option,
                };
                optionLabel.MouseEnter += OptionLabelMouseEnter;
                optionLabel.MouseLeave += OptionLabelMouseLeave;
                optionLabel.MouseDown += OptionLabelMouseDown;
                optionLabel.Measure(new Size(3000, 3000));
                var desiredOptionWidth = optionLabel.DesiredSize.Width;
                if (desiredOptionWidth > maxWidth)
                {
                    maxWidth = desiredOptionWidth;
                }
                _stackPanel.Children.Add(optionLabel);
            }
            _stackPanel.Width = maxWidth;
            _border.Width = maxWidth;
            _mainUiElement = _border;
            _border.Child = _stackPanel;
            _border.MouseDown += OnBorderMouseDown;
            visualChildren.Add(_border);
        }

        /// <inheritdoc/>
        public override bool IsDirectlyEditableType => AdornedTerminal?.Model.Type.IsEnum ?? false;

        private void OnBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            AdornedTerminal.SetAdorner(null);
        }

        private void OptionLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label label)
            {
                label.Background = new SolidColorBrush(Color.FromRgb(8, 25, 150));
                AdornedTerminal.Data = Enum.Parse(AdornedTerminal.Model.Type, label.Content.ToString());
                AdornedTerminal.SetAdorner(null);
            }
        }

        private void OptionLabelMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Label label)
            {
                label.Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
            }
        }

        private void OptionLabelMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Label label)
            {
                label.Background = new SolidColorBrush(Color.FromRgb(16, 16, 16));
            }
        }
    }
}