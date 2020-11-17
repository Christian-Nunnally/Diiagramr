using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to create space on the diagram by scooting all nodes on one side of an axis tangent to that axis.
    /// </summary>
    public class DiagramRifter : DiagramInteractor
    {
        private const int MinimimDistanceToStartRift = 5;
        private const int RiftVisualEndCapSize = 4;
        private readonly ITransactor _transactor;
        private double _lastRiftDeltaX;
        private double _lastRiftDeltaY;
        private IEnumerable<Node> _nodesBeingRifted;
        private MoveNodesToCurrentPositionCommand _undoRiftCommand;

        /// <summary>
        /// Creates a new instance of <see cref="DiagramRifter"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns a <see cref="ITransactor"/> to tractact the node moves through.</param>
        public DiagramRifter(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
            Weight = 0.5;
        }

        private enum RiftMode
        {
            Left,
            Right,
            Up,
            Down,
            None,
        }

        /// <summary>
        /// Gets whether the user is rifting horizontially.
        /// </summary>
        public bool IsModeHorizontial => Mode == RiftMode.Right || Mode == RiftMode.Left;

        /// <summary>
        /// Gets whether the user is rifting vertically.
        /// </summary>
        public bool IsModeVertical => Mode == RiftMode.Down || Mode == RiftMode.Up;

        /// <summary>
        /// Gets or sets the amount of vertical rift.
        /// </summary>
        public double RiftHeight { get; set; }

        /// <summary>
        /// Gets the amount of vertical rift plus the size of the visual end cap.
        /// </summary>
        public double RiftHeightMinus5 => RiftHeight - RiftVisualEndCapSize;

        /// <summary>
        /// Gets the amount of vertical rift minus the size of the visual end cap.
        /// </summary>
        public double RiftHeightPlus5 => RiftHeight + RiftVisualEndCapSize;

        /// <summary>
        /// Gets or sets the point on the diagram to start the rift.
        /// </summary>
        public Point RiftStartDiagramPoint { get; set; }

        /// <summary>
        /// Gets or sets the amount of horizontial rift.
        /// </summary>
        public double RiftWidth { get; set; }

        /// <summary>
        /// Gets the amount of horizontial rift plus the size of the visual end cap.
        /// </summary>
        public double RiftWidthMinus5 => RiftWidth - RiftVisualEndCapSize;

        /// <summary>
        /// Gets the amount of horizontial rift minus the size of the visual end cap.
        /// </summary>
        public double RiftWidthPlus5 => RiftWidth + RiftVisualEndCapSize;

        /// <summary>
        /// Gets the mode of the rift.
        /// </summary>
        private RiftMode Mode { get; set; }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            var diagramMousePoint = diagram.GetDiagramPointFromViewPoint(mousePosition);
            var riftDeltaX = diagramMousePoint.X - RiftStartDiagramPoint.X;
            var riftDeltaY = diagramMousePoint.Y - RiftStartDiagramPoint.Y;
            if (interaction.Type == InteractionType.MouseMoved)
            {
                ProcessMouseMoved(diagram, mousePosition, riftDeltaX, riftDeltaY);
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.IsShiftKeyPressed
                && !interaction.IsAltKeyPressed
                && !interaction.IsCtrlKeyPressed
                && interaction.ViewModelUnderMouse is Diagram;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            RiftStartDiagramPoint = diagram.GetDiagramPointFromViewPoint(mousePosition);
            Mode = RiftMode.None;
            X = mousePosition.X;
            Y = mousePosition.Y;
            RiftWidth = 0;
            RiftHeight = 0;
            _undoRiftCommand = new MoveNodesToCurrentPositionCommand(_nodesBeingRifted);
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            Mode = RiftMode.None;
            _lastRiftDeltaX = 0;
            _lastRiftDeltaY = 0;
            if (_nodesBeingRifted == null)
            {
                return;
            }

            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in _nodesBeingRifted)
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                }
            }

            var doRiftCommand = new MoveNodesToCurrentPositionCommand(_nodesBeingRifted);
            _transactor.Transact(doRiftCommand, _undoRiftCommand, null);
            _nodesBeingRifted = null;
        }

        private void CheckIfRiftShouldStart(double riftDeltaX, double riftDeltaY)
        {
            if (Math.Abs(riftDeltaX) > MinimimDistanceToStartRift)
            {
                Mode = riftDeltaX > 0
                    ? RiftMode.Right
                    : RiftMode.Left;
            }
            else if (Math.Abs(riftDeltaY) > MinimimDistanceToStartRift)
            {
                Mode = riftDeltaY > 0
                    ? RiftMode.Down
                    : RiftMode.Up;
            }
        }

        private IEnumerable<Node> GetNodesToRift(Diagram diagram)
        {
            switch (Mode)
            {
                case RiftMode.Left:
                    return diagram.Nodes.Where(n => n.X + n.Width < RiftStartDiagramPoint.X);

                case RiftMode.Right:
                    return diagram.Nodes.Where(n => n.X > RiftStartDiagramPoint.X);

                case RiftMode.Up:
                    return diagram.Nodes.Where(n => n.Y + n.Height < RiftStartDiagramPoint.Y);

                case RiftMode.Down:
                    return diagram.Nodes.Where(n => n.Y > RiftStartDiagramPoint.Y);

                case RiftMode.None:
                    break;
            }

            return Enumerable.Empty<Node>();
        }

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition, double riftDeltaX, double riftDeltaY)
        {
            if (Mode == RiftMode.None)
            {
                CheckIfRiftShouldStart(riftDeltaX, riftDeltaY);
            }
            else
            {
                Rift(diagram, riftDeltaX, riftDeltaY);
                diagram.UpdateDiagramBoundingBox();
                SetRiftSize(mousePosition);
            }
        }

        private void Rift(Diagram diagram, double riftDeltaX, double riftDeltaY)
        {
            if (_nodesBeingRifted == null)
            {
                _nodesBeingRifted = GetNodesToRift(diagram).ToList();
            }

            RiftNodes(riftDeltaX, riftDeltaY);
        }

        private void RiftNodes(double riftDeltaX, double riftDeltaY)
        {
            var deltaSinceLastMoveX = riftDeltaX - _lastRiftDeltaX;
            var deltaSinceLastMoveY = riftDeltaY - _lastRiftDeltaY;
            switch (Mode)
            {
                case RiftMode.Left:
                    _nodesBeingRifted.ForEach(n => n.X += deltaSinceLastMoveX);
                    break;

                case RiftMode.Right:
                    _nodesBeingRifted.ForEach(n => n.X += deltaSinceLastMoveX);
                    break;

                case RiftMode.Up:
                    _nodesBeingRifted.ForEach(n => n.Y += deltaSinceLastMoveY);
                    break;

                case RiftMode.Down:
                    _nodesBeingRifted.ForEach(n => n.Y += deltaSinceLastMoveY);
                    break;

                case RiftMode.None:
                    break;
            }

            _lastRiftDeltaX = riftDeltaX;
            _lastRiftDeltaY = riftDeltaY;
        }

        private void SetRiftSize(Point mousePosition)
        {
            if (IsModeVertical)
            {
                RiftHeight = mousePosition.Y - Y;
            }
            else if (IsModeHorizontial)
            {
                RiftWidth = mousePosition.X - X;
            }
        }
    }
}