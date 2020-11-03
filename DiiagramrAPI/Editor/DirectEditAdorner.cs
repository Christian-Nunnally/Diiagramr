using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    public abstract class DirectEditAdorner : Adorner
    {
        protected readonly VisualCollection visualChildren;
        protected FrameworkElement _mainUiElement;

        public DirectEditAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement)
        {
            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);
        }

        public Terminal AdornedTerminal { get; set; }

        public abstract bool IsDirectlyEditableType { get; }

        protected override int VisualChildrenCount => visualChildren.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_mainUiElement == null)
            {
                return Size.Empty;
            }

            double width = _mainUiElement.Width;
            double height = _mainUiElement.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            _mainUiElement.Arrange(new Rect(x, y, width, height));
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