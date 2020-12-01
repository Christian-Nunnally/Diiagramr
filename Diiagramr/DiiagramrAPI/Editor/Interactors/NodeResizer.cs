using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to resize resizeable nodes by dragging near the node border.
    /// </summary>
    public class NodeResizer : DiagramInteractor
    {
        private const double ResizeBorderMargin = 6;
        private readonly ITransactor _transactor;
        private IEnumerable<Node> _resizingNodes;
        private MoveNodesToCurrentPositionCommand _undoPositionAdjustmentCommand;
        private ResizeNodesToCurrentSizeCommand _undoResizeCommand;

        /// <summary>
        /// Creates a new instance of <see cref="NodeResizer"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns the <see cref="ITransactor"/> that should transact the resize action.</param>
        public NodeResizer(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
            Weight = 0.5;
        }

        private enum ResizeMode
        {
            Right,
            Top,
            Left,
            Bottom,
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft,
        }

        /// <summary>
        /// Gets or sets the position of the mouse to save for the next time this interaction is processed.
        /// </summary>
        public Point PreviousMouseLocation { get; set; }

        private ResizeMode Mode { get; set; }

        /// <summary>
        /// Computes the distance from a point to a line.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="lineStart">The start point of the line.</param>
        /// <param name="lineStop">The end point of the line.</param>
        /// <returns></returns>
        public static double DistanceFromPointToLine(Point point, Point lineStart, Point lineStop)
        {
            var lineHeight = lineStop.X - lineStart.X;
            var lineWidth = lineStop.Y - lineStart.Y;
            var nominator = Math.Abs(lineHeight * (lineStart.Y - point.Y) - (lineStart.X - point.X) * lineWidth);
            var denominator = Math.Sqrt(Math.Pow(lineHeight, 2) + Math.Pow(lineWidth, 2));
            return nominator / denominator;
        }

        /// <summary>
        /// Computes the distance between two points.
        /// </summary>
        /// <param name="point">The first point.</param>
        /// <param name="otherPoint">The second point.</param>
        /// <returns></returns>
        public static double DistanceFromPoint(Point point, Point otherPoint)
        {
            return Math.Sqrt(Math.Pow(otherPoint.X - point.X, 2) + Math.Pow(otherPoint.Y - point.Y, 2));
        }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var diagram = interaction.Diagram;
                var mousePosition = interaction.MousePosition;
                ProcessMouseMoved(diagram, mousePosition);
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            if ((interaction.Type == InteractionType.LeftMouseDown
              || interaction.Type == InteractionType.MouseMoved)
              && interaction.ViewModelUnderMouse is Node node)
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

                if (mousePosition.X > absoluteLeft && mousePosition.X < absoluteRight)
                {
                    if (mousePosition.Y > absoluteTop && mousePosition.Y < absoluteBottom)
                    {
                        if (scaledMargin > DistanceFromPoint(mousePosition, topLeftCorner))
                        {
                            Mode = ResizeMode.TopLeft;
                            Mouse.SetCursor(Cursors.SizeNWSE);
                        }
                        else if (scaledMargin > DistanceFromPoint(mousePosition, topRightCorner))
                        {
                            Mode = ResizeMode.TopRight;
                            Mouse.SetCursor(Cursors.SizeNESW);
                        }
                        else if (scaledMargin > DistanceFromPoint(mousePosition, bottomLeftCorner))
                        {
                            Mode = ResizeMode.BottomLeft;
                            Mouse.SetCursor(Cursors.SizeNESW);
                        }
                        else if (scaledMargin > DistanceFromPoint(mousePosition, bottomRightCorner))
                        {
                            Mode = ResizeMode.BottomRight;
                            Mouse.SetCursor(Cursors.SizeNWSE);
                        }
                        else if (scaledMargin > DistanceFromPointToLine(mousePosition, topLeftCorner, topRightCorner))
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
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            PreviousMouseLocation = interaction.MousePosition;
            _resizingNodes = interaction.Diagram.Nodes.Where(n => n.IsSelected).ToArray();
            _undoResizeCommand = new ResizeNodesToCurrentSizeCommand(_resizingNodes);
            _undoPositionAdjustmentCommand = new MoveNodesToCurrentPositionCommand(_resizingNodes);
            interaction.Diagram.ShowSnapGrid = true;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in _resizingNodes)
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                    node.Width = interaction.Diagram.SnapToGrid(node.Width);
                    node.Height = interaction.Diagram.SnapToGrid(node.Height);
                }
            }

            interaction.Diagram.ShowSnapGrid = false;
            Mouse.SetCursor(Cursors.Arrow);
            var resizeCommand = new ResizeNodesToCurrentSizeCommand(_resizingNodes);
            var positionAdjustmentCommand = new MoveNodesToCurrentPositionCommand(_resizingNodes);
            _transactor.Transact(resizeCommand, _undoResizeCommand);
            _transactor.Transact(positionAdjustmentCommand, _undoPositionAdjustmentCommand);
        }

        private static void MoveTop(Diagram diagram, double deltaY, Node node)
        {
            var heightChange = deltaY / diagram.Zoom;
            node.Height -= heightChange;
            node.Y += heightChange;
        }

        private static void MoveLeft(Diagram diagram, double deltaX, Node node)
        {
            var widthChange = deltaX / diagram.Zoom;
            node.Width -= widthChange;
            node.X += widthChange;
        }

        private static void MoveRight(Diagram diagram, double deltaX, Node node)
        {
            var widthChange = deltaX / diagram.Zoom;
            node.Width += widthChange;
        }

        private static void MoveBottom(Diagram diagram, double deltaY, Node node)
        {
            var heightChange = deltaY / diagram.Zoom;
            node.Height += heightChange;
        }

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition)
        {
            var deltaX = mousePosition.X - PreviousMouseLocation.X;
            var deltaY = mousePosition.Y - PreviousMouseLocation.Y;
            foreach (var node in diagram.Nodes.Where(n => n.IsSelected))
            {
                switch (Mode)
                {
                    case ResizeMode.Right:
                        MoveRight(diagram, deltaX, node);
                        break;

                    case ResizeMode.Top:
                        MoveTop(diagram, deltaY, node);
                        break;

                    case ResizeMode.Left:
                        MoveLeft(diagram, deltaX, node);
                        break;

                    case ResizeMode.Bottom:
                        MoveBottom(diagram, deltaY, node);
                        break;

                    case ResizeMode.TopLeft:
                        MoveTop(diagram, deltaY, node);
                        MoveLeft(diagram, deltaX, node);
                        break;

                    case ResizeMode.TopRight:
                        MoveTop(diagram, deltaY, node);
                        MoveRight(diagram, deltaX, node);
                        break;

                    case ResizeMode.BottomLeft:
                        MoveBottom(diagram, deltaY, node);
                        MoveLeft(diagram, deltaX, node);
                        break;

                    case ResizeMode.BottomRight:
                        MoveBottom(diagram, deltaY, node);
                        MoveRight(diagram, deltaX, node);
                        break;
                }
            }

            PreviousMouseLocation = mousePosition;
            diagram.UpdateDiagramBoundingBox();
        }
    }
}