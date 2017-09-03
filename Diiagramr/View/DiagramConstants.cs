using System.Windows;

namespace Diiagramr.View
{
    public static class DiagramConstants
    {
        public const double NodeBorderWidth = 10;

        public const double TerminalDiameter = 2 * NodeBorderWidth;

        public static Thickness NodeBorderThickness = new Thickness(NodeBorderWidth);

        public static Thickness NodeSelectionBorderThickness = new Thickness(NodeBorderWidth - 1);

        public static CornerRadius TerminalCornerRadius = new CornerRadius(2);
    }
}
