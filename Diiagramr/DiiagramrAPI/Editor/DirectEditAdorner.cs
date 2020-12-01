using DiiagramrAPI.Editor.Diagrams;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// An adorner that allows users to directly edit the data on a <see cref="Terminal"/>.
    /// </summary>
    public abstract class DirectEditAdorner : Adorner
    {
        protected readonly VisualCollection visualChildren;
        protected FrameworkElement _mainUiElement;

        /// <summary>
        /// Creates a new instance of <see cref="DirectEditAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element the adorner should attach to.</param>
        /// <param name="adornedTerminal">The terminal the adorner can edit the data of.</param>
        public DirectEditAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement)
        {
            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);
        }

        /// <summary>
        /// The terminal this adorner can edit the data of.
        /// </summary>
        public Terminal AdornedTerminal { get; set; }

        /// <summary>
        /// Whether the type of the adorned terminal is editable with this class.
        /// </summary>
        public abstract bool IsDirectlyEditableType { get; }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => visualChildren.Count;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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