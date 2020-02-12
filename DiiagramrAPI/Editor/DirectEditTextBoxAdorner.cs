using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    public class DirectEditTextBoxAdorner : Adorner
    {
        private readonly TextBox textBox;
        private readonly VisualCollection visualChildren;

        public DirectEditTextBoxAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);

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
            visualChildren.Add(textBox);
        }

        public Terminal AdornedTerminal { get; set; }

        public string DirectEditTextBoxText
        {
            get => AdornedTerminal.Data?.ToString() ?? string.Empty;
            set => AdornedTerminal.Data = CoerceStringToType(value);
        }

        public bool IsBoolType => AdornedTerminal.TerminalModel.Type == typeof(bool);

        public bool IsCharType => AdornedTerminal.TerminalModel.Type == typeof(char);

        public bool IsDirectlyEditableType => IsIntType || IsFloatType || IsStringType || IsCharType;

        public bool IsEnumType => AdornedTerminal.TerminalModel.Type.IsEnum;

        public bool IsFloatType => AdornedTerminal.TerminalModel.Type == typeof(float);

        public bool IsIntType => AdornedTerminal.TerminalModel.Type == typeof(int);

        public bool IsStringType => AdornedTerminal.TerminalModel.Type == typeof(string);

        protected override int VisualChildrenCount => visualChildren.Count;

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

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = textBox.Width;
            double height = textBox.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            textBox.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
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

        private double GetRelativeXBasedOnTerminalDirection(double width)
        {
            var direction = AdornedTerminal.TerminalRotation;
            return TerminalAdornerHelpers.GetVisualXBasedOnTerminalDirection(width, direction);
        }

        private double GetRelativeYBasedOnTerminalDirection(double height)
        {
            var direction = AdornedTerminal.TerminalRotation;
            return TerminalAdornerHelpers.GetVisualYBasedOnTerminalDirection(height, direction);
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