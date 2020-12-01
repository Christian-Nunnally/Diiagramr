using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Node adorner that shows the name of the node.
    /// </summary>
    public class NodeNameAdorner : Adorner
    {
        private readonly Border border;
        private readonly TextBlock textBlock;
        private readonly double marginAroundLabel = 4.0;
        private readonly double marginFromNode = Diagram.NodeBorderWidth + 6;
        private readonly VisualCollection visualChildren;

        /// <summary>
        /// Creates a new instance of <see cref="NodeNameAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element to adorn.</param>
        /// <param name="adornedNode">The node whose name to display.</param>
        public NodeNameAdorner(UIElement adornedElement, Node adornedNode)
            : base(adornedElement)
        {
            if (adornedNode == null)
            {
                return;
            }

            AdornedNode = adornedNode;
            visualChildren = new VisualCollection(this);
            var text = AdornedNode.Name + " Node";

            textBlock = new TextBlock
            {
                Text = text,
                FontSize = 12,
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

        /// <summary>
        /// The node being adorned.
        /// </summary>
        public Node AdornedNode { get; set; }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => visualChildren.Count;

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = border.Width;
            double height = border.Height;

            double x = (AdornedNode.Width / 2.0) - (width / 2.0);
            double y = -height - marginFromNode;
            border.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }
    }
}