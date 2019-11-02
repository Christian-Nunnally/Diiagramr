namespace DiiagramrAPI.Editor
{
    internal static class TerminalAdornerHelpers
    {
        private const double MarginFromTerminal = 5.0;
        private const double XAdjustmentBecauseTheTerminalVisualIsRotated = (Diagram.NodeBorderWidth - (Terminal.TerminalWidth / 2));

        public static double GetVisualXBasedOnTerminalDirection(double width, float direction)
        {
            switch (direction)
            {
                case 90:
                    return MarginFromTerminal + Terminal.TerminalWidth + XAdjustmentBecauseTheTerminalVisualIsRotated;

                case 270:
                    return -width - MarginFromTerminal - XAdjustmentBecauseTheTerminalVisualIsRotated;

                default:
                    return (Terminal.TerminalWidth / 2) - (width / 2);
            }
        }

        public static double GetVisualYBasedOnTerminalDirection(double height, float direction)
        {
            switch (direction)
            {
                case 0:
                    return -MarginFromTerminal - height;

                case 180:
                    return MarginFromTerminal + Terminal.TerminalHeight;

                default:
                    return (Terminal.TerminalHeight / 2) - (height / 2);
            }
        }
    }
}