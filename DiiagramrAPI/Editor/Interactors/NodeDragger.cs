using DiiagramrAPI.Application.Commands;
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
    /// Allows the user to drag nodes by clicking and dragging thier border.
    /// </summary>
    public class NodeDragger : DiagramInteractor
    {
        private readonly ITransactor _transactor;
        private IEnumerable<Node> _draggingNodes;
        private IReversableCommand _moveNodesToStartPointCommand;

        /// <summary>
        /// Creates a new instance of <see cref="NodeDragger"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns a <see cref="ITransactor"/> to tractact the drags through.</param>
        public NodeDragger(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
        }

        /// <summary>
        /// The location of the mouse the last time this interaction was processed.
        /// </summary>
        public Point PreviousMouseLocation { get; set; }

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
            if (interaction.ViewModelUnderMouse is Node node
                && !interaction.IsCtrlKeyPressed)
            {
                var mouseX = interaction.MousePosition.X;
                var mouseY = interaction.MousePosition.Y;
                var diagram = interaction.Diagram;
                if (IsMouseOverNodeBorder(node, mouseX, mouseY, diagram))
                {
                    Mouse.SetCursor(Cursors.SizeAll);
                    return interaction.Type == InteractionType.LeftMouseDown;
                }
            }
            else
            {
                Mouse.SetCursor(Cursors.Arrow);
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
            _draggingNodes = interaction.Diagram.Nodes.Where(n => n.IsSelected).ToArray();
            _moveNodesToStartPointCommand = new MoveNodesToCurrentPositionCommand(_draggingNodes);
            PreviousMouseLocation = interaction.MousePosition;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in _draggingNodes)
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                }
            }

            interaction.Diagram.ShowSnapGrid = false;
            var doCommand = new MoveNodesToCurrentPositionCommand(_draggingNodes);
            _transactor.Transact(doCommand, _moveNodesToStartPointCommand, _draggingNodes);
        }

        private static bool IsMouseOverNodeBorder(Node node, double mouseX, double mouseY, Diagram diagram)
        {
            var nodeLeft = diagram.GetViewPointFromDiagramPointX(node.X + Diagram.NodeBorderWidth);
            var nodeTop = diagram.GetViewPointFromDiagramPointY(node.Y + Diagram.NodeBorderWidth);
            var nodeRight = diagram.GetViewPointFromDiagramPointX(node.X + node.Width + Diagram.NodeBorderWidth);
            var nodeBottom = diagram.GetViewPointFromDiagramPointY(node.Y + node.Height + Diagram.NodeBorderWidth);
            return mouseX > nodeRight
                || mouseX < nodeLeft
                || mouseY > nodeBottom
                || mouseY < nodeTop;
        }

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition)
        {
            var deltaX = mousePosition.X - PreviousMouseLocation.X;
            var deltaY = mousePosition.Y - PreviousMouseLocation.Y;
            foreach (var otherNode in diagram.Nodes.Where(n => n.IsSelected))
            {
                otherNode.X += deltaX / diagram.Zoom;
                otherNode.Y += deltaY / diagram.Zoom;
            }

            PreviousMouseLocation = mousePosition;
            diagram.UpdateDiagramBoundingBox();
            diagram.ShowSnapGrid = true;
        }
    }
}