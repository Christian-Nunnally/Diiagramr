using DiiagramrAPI.Editor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Service
{
    public class NodeNameAdornernment : Adorner
    {
        private readonly double MarginAroundLabel = 4.0;
        private readonly double MarginFromNode = Diagram.NodeBorderWidth + 5;
        private readonly Border border;
        private readonly TextBlock label;
        private readonly VisualCollection visualChildren;

        public NodeNameAdornernment(UIElement adornedElement, Node adornedNode) : base(adornedElement)
        {
            if (adornedNode == null)
            {
                return;
            }

            AdornedNode = adornedNode;
            visualChildren = new VisualCollection(this);
            var text = AdornedNode.Name + " Node";

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

        public Node AdornedNode { get; set; }
        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = border.Width;
            double height = border.Height;

            double x = (AdornedNode.Width / 2.0) - (width / 2.0);
            double y = -height - MarginFromNode;
            border.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
