using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (adornedTerminal == null) return;
            AdornedTerminal = adornedTerminal;

            visualChildren = new VisualCollection(this);

            label = new TextBlock();
            label.IsHitTestVisible = false;
            label.Width = 80;
            label.Height = 50;
            label.Text = AdornedTerminal.Data?.ToString();
            label.Margin = new Thickness(0);
            label.FontSize = 10;
            label.LineStackingStrategy = LineStackingStrategy.MaxHeight;
            label.TextWrapping = TextWrapping.Wrap;
            border = new Border();
            border.IsHitTestVisible = false;
            border.Background = Brushes.White;
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.Padding = new Thickness(0);
            border.MaxWidth = 200;
            border.Child = label;
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

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
