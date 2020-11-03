using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    public class DirectEditTextBoxAdorner : DirectEditAdorner
    {
        private readonly TextBox textBox;

        public DirectEditTextBoxAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement, adornedTerminal)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            textBox = new TextBox
            {
                IsHitTestVisible = true,
                Margin = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(16, 16, 16)),
                Foreground = new SolidColorBrush(Color.FromRgb(178, 178, 178)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 96, 96)),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Width = 50,
                Height = 30,
            };
            textBox.Loaded += TextBoxLoaded;
            Binding binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DirectEditTextBoxText)),
                Mode = BindingMode.TwoWay,
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
            textBox.KeyDown += KeyDownHandler;
            textBox.LostFocus += LostFocusHandler;
            textBox.LostKeyboardFocus += LostFocusHandler;
            textBox.AutoWordSelection = true;
            _mainUiElement = textBox;
            visualChildren.Add(textBox);
        }

        public string DirectEditTextBoxText
        {
            get => AdornedTerminal.Data?.ToString() ?? string.Empty;
            set => AdornedTerminal.Data = CoerceStringToType(value);
        }

        public bool IsCharType => AdornedTerminal.Model.Type == typeof(char);

        public override bool IsDirectlyEditableType => IsIntType || IsFloatType || IsStringType || IsCharType;

        public bool IsFloatType => AdornedTerminal.Model.Type == typeof(float);

        public bool IsIntType => AdornedTerminal.Model.Type == typeof(int);

        public bool IsStringType => AdornedTerminal.Model.Type == typeof(string);

        public object CoerceStringToType(string data)
        {
            if (IsFloatType && float.TryParse(data, out float parsedFloat))
            {
                return parsedFloat;
            }
            else if (IsCharType && char.TryParse(data, out char parsedChar))
            {
                return parsedChar;
            }
            else if (IsIntType && int.TryParse(data, out int parseResult))
            {
                return parseResult;
            }
            else if (IsStringType)
            {
                return data;
            }
            else
            {
                return AdornedTerminal.Data;
            }
        }

        private void FocusTextBox()
        {
            textBox.Focus();
            Keyboard.Focus(textBox);
        }

        private void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            FocusTextBox();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdornedTerminal.SetAdorner(null);
            }
        }

        private void LostFocusHandler(object sender, RoutedEventArgs e)
        {
            AdornedTerminal.SetAdorner(null);
        }
    }
}