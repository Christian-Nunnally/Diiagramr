using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.ViewModel.ProjectScreen.Diagram
{
    public class TerminalDataProbeAdorner : Adorner
    {
        private VisualCollection visualChildren;
        private Border border;
        private TextBlock label;
        public TerminalViewModel AdornedTerminal { get; set; }

        public TerminalDataProbeAdorner(UIElement adornedElement, TerminalViewModel adornedTerminal) : base(adornedElement)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            AdornedTerminal = adornedTerminal;

            visualChildren = new VisualCollection(this);

            label = new TextBlock
            {
                IsHitTestVisible = false,
                Width = 80,
                Height = 50,
                Text = AdornedTerminal.Data?.ToString(),
                Margin = new Thickness(0),
                FontSize = 10,
                LineStackingStrategy = LineStackingStrategy.MaxHeight,
                TextWrapping = TextWrapping.Wrap
            };
            border = new Border
            {
                IsHitTestVisible = false,
                Background = Brushes.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gray,
                Padding = new Thickness(0),
                MaxWidth = 200,
                Child = label
            };
            visualChildren.Add(border);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            double y = TerminalViewModel.TerminalDiameter;
            double width = label.DesiredSize.Width + 2.0;
            double height = label.DesiredSize.Height + 2.0;
            border.Arrange(new Rect(x, y, 80, 50));
            return finalSize;
        }

        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
