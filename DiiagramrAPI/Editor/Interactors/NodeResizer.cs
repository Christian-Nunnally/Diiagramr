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
    public class NodeResizer : DiagramInteractor
    {
        private const double ResizeBorderMargin = 4;
        private readonly ITransactor _transactor;
        private IEnumerable<Node> _resizingNodes;
        private MoveNodesToCurrentPositionCommand _undoPositionAdjustmentCommand;
        private ResizeNodesToCurrentSizeCommand _undoResizeCommand;

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
        }

        public Point PreviousMouseLocation { get; set; }

        private ResizeMode Mode { get; set; }

        public static double DistanceFromPointToLine(Point point, Point lineStart, Point lineStop)
        {
            var lineHeight = lineStop.X - lineStart.X;
            var lineWidth = lineStop.Y - lineStart.Y;
            var nominator = Math.Abs(lineHeight * (lineStart.Y - point.Y) - (lineStart.X - point.X) * lineWidth);
            var denominator = Math.Sqrt(Math.Pow(lineHeight, 2) + Math.Pow(lineWidth, 2));
            return nominator / denominator;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                var diagram = interaction.Diagram;
                var mousePosition = interaction.MousePosition;
                ProcessMouseMoved(diagram, mousePosition);
            }
        }

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
                }
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
            _resizingNodes = interaction.Diagram.Nodes.Where(n => n.IsSelected).ToArray();
            _undoResizeCommand = new ResizeNodesToCurrentSizeCommand(_resizingNodes);
            _undoPositionAdjustmentCommand = new MoveNodesToCurrentPositionCommand(_resizingNodes);
            interaction.Diagram.ShowSnapGrid = true;
        }

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
            _transactor.Transact(resizeCommand, _undoResizeCommand, _resizingNodes);
            _transactor.Transact(positionAdjustmentCommand, _undoPositionAdjustmentCommand, _resizingNodes);
        }

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition)
        {
            var deltaX = mousePosition.X - PreviousMouseLocation.X;
            var deltaY = mousePosition.Y - PreviousMouseLocation.Y;
            foreach (var node in diagram.Nodes.Where(n => n.IsSelected))
            {
                if (Mode == ResizeMode.Right)
                {
                    var widthChange = deltaX / diagram.Zoom;
                    node.Width += widthChange;
                }
                else if (Mode == ResizeMode.Top)
                {
                    var heightChange = deltaY / diagram.Zoom;
                    node.Height -= heightChange;
                    node.Y += heightChange;
                }
                else if (Mode == ResizeMode.Left)
                {
                    var widthChange = deltaX / diagram.Zoom;
                    node.Width -= widthChange;
                    node.X += widthChange;
                }
                else if (Mode == ResizeMode.Bottom)
                {
                    var heightChange = deltaY / diagram.Zoom;
                    node.Height += heightChange;
                }
            }

            PreviousMouseLocation = mousePosition;
            diagram.UpdateDiagramBoundingBox();
        }
    }
}