using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class NodeResizer : DiagramInteractor
    {
        private const double ResizeBorderMargin = 2;
        public Point PreviousMouseLocation { get; set; }
        private ResizeMode Mode { get; set; }

        private enum ResizeMode
        {
            Right,
            Top,
            Left,
            Bottom,
        }

        public NodeResizer()
        {
            Weight = 0.5;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var deltaX = interaction.MousePosition.X - PreviousMouseLocation.X;
                var deltaY = interaction.MousePosition.Y - PreviousMouseLocation.Y;
                foreach (var node in interaction.Diagram.NodeViewModels.Where(n => n.IsSelected))
                {
                    if (Mode == ResizeMode.Right)
                    {
                        var widthChange = deltaX / interaction.Diagram.Zoom;
                        node.Width += widthChange;
                    }
                    else if (Mode == ResizeMode.Top)
                    {
                        var heightChange = deltaY / interaction.Diagram.Zoom;
                        node.Height -= heightChange;
                        node.Y += heightChange;
                    }
                    else if (Mode == ResizeMode.Left)
                    {
                        var widthChange = deltaX / interaction.Diagram.Zoom;
                        node.Width -= widthChange;
                        node.X += widthChange;
                    }
                    else if (Mode == ResizeMode.Bottom)
                    {
                        var heightChange = deltaY / interaction.Diagram.Zoom;
                        node.Height += heightChange;
                    }
                }
                PreviousMouseLocation = interaction.MousePosition;
                interaction.Diagram.UpdateDiagramBoundingBox();
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            if ((interaction.Type == InteractionType.LeftMouseDown
              || interaction.Type == InteractionType.MouseMoved)
              && interaction.ViewModelMouseIsOver is Node node)
            {
                if (!node.ResizeEnabled)
                {
                    return false;
                }
                var mousePosition = interaction.MousePosition;
                var diagram = interaction.Diagram;
                var absoluteLeft = diagram.GetViewPointFromDiagramPointX(node.X + Diagram.NodeBorderWidth);
                var absoluteTop = diagram.GetViewPointFromDiagramPointY(node.Y + Diagram.NodeBorderWidth);
                var absoluteRight = diagram.GetViewPointFromDiagramPointX(node.X + node.Width + Diagram.NodeBorderWidth);
                var absoluteBottom = diagram.GetViewPointFromDiagramPointY(node.Y + node.Height + Diagram.NodeBorderWidth);
                var topLeftCorner = new Point(absoluteLeft, absoluteTop);
                var topRightCorner = new Point(absoluteRight, absoluteTop);
                var bottomLeftCorner = new Point(absoluteLeft, absoluteBottom);
                var bottomRightCorner = new Point(absoluteRight, absoluteBottom);
                var scaledMargin = ResizeBorderMargin * diagram.Zoom;

                if (scaledMargin > DistanceFromPointToLine(mousePosition, topLeftCorner, topRightCorner))
                {
                    Mode = ResizeMode.Top;
                    Mouse.SetCursor(Cursors.SizeNS);
                }
                else if (scaledMargin > DistanceFromPointToLine(mousePosition, topRightCorner, bottomRightCorner))
                {
                    Mode = ResizeMode.Right;
                    Mouse.SetCursor(Cursors.SizeWE);
                }
                else if (scaledMargin > DistanceFromPointToLine(mousePosition, bottomRightCorner, bottomLeftCorner))
                {
                    Mode = ResizeMode.Bottom;
                    Mouse.SetCursor(Cursors.SizeNS);
                }
                else if (scaledMargin > DistanceFromPointToLine(mousePosition, bottomLeftCorner, topLeftCorner))
                {
                    Mode = ResizeMode.Left;
                    Mouse.SetCursor(Cursors.SizeWE);
                }
                else
                {
                    return false;
                }
                return interaction.Type == InteractionType.LeftMouseDown;
            }
            return false;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            PreviousMouseLocation = interaction.MousePosition;
            interaction.Diagram.ShowSnapGrid = true;
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in interaction.Diagram.NodeViewModels.Where(n => n.IsSelected))
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                    node.Width = interaction.Diagram.SnapToGrid(node.Width);
                    node.Height = interaction.Diagram.SnapToGrid(node.Height);
                }
            }
            interaction.Diagram.ShowSnapGrid = false;
            Mouse.SetCursor(Cursors.Arrow);
        }

        public static double DistanceFromPointToLine(Point point, Point lineStart, Point lineStop)
        {
            var lineHeight = lineStop.X - lineStart.X;
            var lineWidth = lineStop.Y - lineStart.Y;
            var nominator = Math.Abs(lineHeight * (lineStart.Y - point.Y) - (lineStart.X - point.X) * lineWidth);
            var denominator = Math.Sqrt(Math.Pow(lineHeight, 2) + Math.Pow(lineWidth, 2));
            return nominator / denominator;
        }
    }
}
