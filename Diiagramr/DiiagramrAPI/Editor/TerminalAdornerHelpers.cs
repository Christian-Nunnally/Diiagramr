using DiiagramrAPI.Editor.Diagrams;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Helpers to arrange terminal adorners since terminal can be rotated in four different directions.
    /// </summary>
    internal static class TerminalAdornerHelpers
    {
        private const double MarginFromTerminal = 5.0;
        private const double XAdjustmentBecauseTheTerminalVisualIsRotated = Diagram.NodeBorderWidth - (Terminal.TerminalWidth / 2);

        /// <summary>
        /// Gets the correct X position of a terminal adorner given the width of the node and the direction the terminal is facing.
        /// </summary>
        /// <param name="width">The width of the node.</param>
        /// <param name="direction">The direction the terminal is facing.</param>
        /// <returns>A good X position for a terminal adorner.</returns>
        public static double GetVisualXBasedOnTerminalDirection(double width, float direction) => direction switch
        {
            90 => MarginFromTerminal + Terminal.TerminalWidth + XAdjustmentBecauseTheTerminalVisualIsRotated,
            270 => -width - MarginFromTerminal - XAdjustmentBecauseTheTerminalVisualIsRotated,
            _ => (Terminal.TerminalWidth / 2) - (width / 2),
        };

        /// <summary>
        /// Gets the correct Y position of a terminal adorner given the height of the node and the direction the terminal is facing.
        /// </summary>
        /// <param name="height">The height of the node.</param>
        /// <param name="direction">The direction the terminal is facing.</param>
        /// <returns>A good Y position for a terminal adorner.</returns>
        public static double GetVisualYBasedOnTerminalDirection(double height, float direction) => direction switch
        {
            0 => -MarginFromTerminal - height,
            180 => MarginFromTerminal + Terminal.TerminalHeight,
            _ => (Terminal.TerminalHeight / 2) - (height / 2),
        };
    }
}