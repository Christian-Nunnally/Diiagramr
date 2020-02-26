using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    public class TerminalToolTipAdorner : Adorner
    {
        private readonly double marginAroundLabel = 3.0;
        private readonly Border border;
        private readonly TextBlock textBlock;
        private readonly VisualCollection visualChildren;

        public TerminalToolTipAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);
            var text = AdornedTerminal.Data != null && AdornedTerminal.Data.ToString().Length < 8
                ? AdornedTerminal.Name + " = " + AdornedTerminal.Data.ToString()
                : AdornedTerminal.Name;

            textBlock = new TextBlock
            {
                Text = text,
                FontSize = 10,
                Margin = new Thickness(0),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Verdana"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                LineStackingStrategy = LineStackingStrategy.MaxHeight,
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                Foreground = new SolidColorBrush(Color.FromRgb(178, 178, 178)),
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            border = new Border
            {
                Child = textBlock,
                IsHitTestVisible = false,
                Padding = new Thickness(0),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(1),
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                Width = textBlock.DesiredSize.Width + marginAroundLabel * 2,
                MaxWidth = 200,
                Height = textBlock.DesiredSize.Height + marginAroundLabel * 2,
            };
            visualChildren.Add(border);
        }

        public Terminal AdornedTerminal { get; set; }

        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = border.Width;
            double height = border.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            border.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
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
    }
}