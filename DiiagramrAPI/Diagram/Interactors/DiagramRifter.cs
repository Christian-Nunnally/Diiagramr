using DiiagramrAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramRifter : DiagramInteractor
    {
        private const int RiftVisualEndCapSize = 4;
        private const int MinimimDistanceToStartRift = 5;
        private double _lastRiftDeltaX;
        private double _lastRiftDeltaY;
        public Point RiftStartDiagramPoint { get; set; }
        private RiftMode Mode { get; set; }
        private IEnumerable<Node> NodesBeingRifted { get; set; }

        public double RiftWidth { get; set; }
        public double RiftWidthPlus5 => RiftWidth + RiftVisualEndCapSize;
        public double RiftWidthMinus5 => RiftWidth - RiftVisualEndCapSize;

        public double RiftHeight { get; set; }
        public double RiftHeightPlus5 => RiftHeight + RiftVisualEndCapSize;
        public double RiftHeightMinus5 => RiftHeight - RiftVisualEndCapSize;

        public bool IsModeHorizontial => Mode == RiftMode.Right || Mode == RiftMode.Left;
        public bool IsModeVertical => Mode == RiftMode.Down || Mode == RiftMode.Up;

        private enum RiftMode
        {
            Left,
            Right,
            Up,
            Down,
            None
        }

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

        private void ProcessMouseMoved(Diagram diagram, Point mousePosition, double riftDeltaX, double riftDeltaY)
        {
            if (Mode == RiftMode.None)
            {
                CheckIfRiftShouldStart(riftDeltaX, riftDeltaY);
            }
            if (Mode != RiftMode.None)
            {
                Rift(diagram, riftDeltaX, riftDeltaY);
                diagram.UpdateDiagramBoundingBox();
                SetRiftSize(mousePosition);
            }
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

        private void Rift(Diagram diagram, double riftDeltaX, double riftDeltaY)
        {
            if (NodesBeingRifted == null)
            {
                NodesBeingRifted = GetNodesToRift(diagram).ToList();
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
                    NodesBeingRifted.ForEach(n => n.X += deltaSinceLastMoveX);
                    break;
                case RiftMode.Right:
                    NodesBeingRifted.ForEach(n => n.X += deltaSinceLastMoveX);
                    break;
                case RiftMode.Up:
                    NodesBeingRifted.ForEach(n => n.Y += deltaSinceLastMoveY);
                    break;
                case RiftMode.Down:
                    NodesBeingRifted.ForEach(n => n.Y += deltaSinceLastMoveY);
                    break;
                case RiftMode.None:
                    break;
            }
            _lastRiftDeltaX = riftDeltaX;
            _lastRiftDeltaY = riftDeltaY;
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
            }
            return Enumerable.Empty<Node>();
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

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.IsShiftKeyPressed
                && interaction.ViewModelUnderMouse is Diagram;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

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
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            Mode = RiftMode.None;
            _lastRiftDeltaX = 0;
            _lastRiftDeltaY = 0;

            if (!interaction.IsCtrlKeyPressed)
            {
                foreach (var node in NodesBeingRifted)
                {
                    node.X = interaction.Diagram.SnapToGrid(node.X);
                    node.Y = interaction.Diagram.SnapToGrid(node.Y);
                }
            }

            NodesBeingRifted = null;
        }
    }
}
