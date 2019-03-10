using DiiagramrAPI.Diagram;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Service
{
    public class DirectEditTextBoxAdorner : Adorner
    {
        private readonly double MarginFromTerminal = 5.0;
        private readonly TextBox textBox;
        private VisualCollection visualChildren;
        private string _directEditTextBoxText;

        public bool IsBoolType => AdornedTerminal.TerminalModel.Type == typeof(bool);
        public bool IsCharType => AdornedTerminal.TerminalModel.Type == typeof(char);
        public bool IsDirectlyEditableType => IsIntType || IsFloatType || IsStringType || IsCharType;
        public bool IsFloatType => AdornedTerminal.TerminalModel.Type == typeof(float);
        public bool IsIntType => AdornedTerminal.TerminalModel.Type == typeof(int);
        public bool IsStringType => AdornedTerminal.TerminalModel.Type == typeof(string);

        public DirectEditTextBoxAdorner(UIElement adornedElement, TerminalViewModel adornedTerminal) : base(adornedElement)
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
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Width = 50,
                Height = 25
            };
            Binding binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(DirectEditTextBoxText)),
                Mode = BindingMode.TwoWay
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
            textBox.KeyDown += KeyDownHandler;
            visualChildren.Add(textBox);
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdornedTerminal.Adorner = null;
            }
        }

        public TerminalViewModel AdornedTerminal { get; set; }
        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = textBox.Width;
            double height = textBox.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            textBox.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        private double GetRelativeXBasedOnTerminalDirection(double width)
        {
            switch (AdornedTerminal.TerminalRotation)
            {
                case 90:
                    return MarginFromTerminal + TerminalViewModel.TerminalDiameter;

                case 270:
                    return -width - MarginFromTerminal;

                default:
                    return (TerminalViewModel.TerminalDiameter / 2) - (width / 2);
            }
        }

        private double GetRelativeYBasedOnTerminalDirection(double height)
        {
            switch (AdornedTerminal.TerminalRotation)
            {
                case 0:
                    return -MarginFromTerminal - height;

                case 180:
                    return MarginFromTerminal + TerminalViewModel.TerminalDiameter;

                default:
                    return (TerminalViewModel.TerminalDiameter / 2) - (height / 2);
            }
        }

        public string DirectEditTextBoxText
        {
            get => AdornedTerminal.Data?.ToString() ?? string.Empty;

            set
            {
                if (IsIntType)
                {
                    if (int.TryParse(value, out int parseResult))
                    {
                        _directEditTextBoxText = value;
                        AdornedTerminal.Data = parseResult;
                    }
                }
                else if (IsFloatType)
                {
                    if (float.TryParse(value, out float parseResult))
                    {
                        _directEditTextBoxText = value;
                        AdornedTerminal.Data = parseResult;
                    }
                }
                else if (IsStringType)
                {
                    _directEditTextBoxText = value;
                    AdornedTerminal.Data = value;
                }
                else if (IsCharType)
                {
                    if (char.TryParse(value, out char parseResult))
                    {
                        _directEditTextBoxText = value;
                        AdornedTerminal.Data = parseResult;
                    }
                }
            }
        }
    }
}
