using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Service
{
    public class ToolTipAdorner : Adorner
    {
        private readonly double MarginAroundLabel = 4.0;
        private readonly double MarginFromTerminal = 5.0;
        private Border border;
        private TextBlock label;
        private VisualCollection visualChildren;

        public ToolTipAdorner(UIElement adornedElement, TerminalViewModel adornedTerminal) : base(adornedElement)
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

            label = new TextBlock
            {
                IsHitTestVisible = false,
                Text = text,
                Margin = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                LineStackingStrategy = LineStackingStrategy.MaxHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            border = new Border
            {
                IsHitTestVisible = false,
                Background = Brushes.AliceBlue,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gray,
                Padding = new Thickness(0),
                MaxWidth = 200,
                Child = label,
                Width = label.DesiredSize.Width + MarginAroundLabel * 2,
                Height = label.DesiredSize.Height + MarginAroundLabel * 2
            };
            visualChildren.Add(border);
        }

        public TerminalViewModel AdornedTerminal { get; set; }
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
    }
}
