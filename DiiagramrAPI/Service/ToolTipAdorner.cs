using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DiiagramrAPI.Service
{
    public class ToolTipAdorner : Adorner
    {
        private VisualCollection visualChildren;
        private Border border;
        private TextBlock label;
        private readonly double MarginFromTopOfTerminal = 5.0;

        public TerminalViewModel AdornedTerminal { get; set; }
        public ToolTipAdorner(UIElement adornedElement, TerminalViewModel adornedTerminal) : base(adornedElement)
        {
            if (adornedTerminal == null) return;
            AdornedTerminal = adornedTerminal;

            visualChildren = new VisualCollection(this);

            label = new TextBlock();
            label.IsHitTestVisible = false;
            label.Text = AdornedTerminal.Name;
            label.Margin = new Thickness(0);
            label.FontSize = 12;
            label.LineStackingStrategy = LineStackingStrategy.MaxHeight;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            border = new Border();
            border.IsHitTestVisible = false;
            border.Background = Brushes.AliceBlue;
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Gray;
            border.Padding = new Thickness(0);
            border.MaxWidth = 200;
            border.Child = label;
            border.Width = 80;
            border.Height = 20;
            visualChildren.Add(border);

            var widthAnimation = new DoubleAnimation(0, 80, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            // border.BeginAnimation(Label.WidthProperty, widthAnimation);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = border.Width;
            double height = border.Height;
            double x = (TerminalViewModel.TerminalDiameter / 2) - (width / 2);
            double y = -MarginFromTopOfTerminal - height;
            border.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
