using System;
using System.Linq;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to wire nodes together by dragging a selection box around a group of nodes.
    /// The <see cref="LassoNodeWirer"/> will try to figure out a logical way to connect the group of nodes together.
    /// </summary>
    public class LassoNodeWirer : DiagramInteractor
    {
        private double _endX;
        private double _endY;
        private double _startX;
        private double _startY;

        /// <summary>
        /// Gets whether the selection box label should be visible.
        /// </summary>
        public bool IsSelectorBigEnoughToDisplayHelpLabel => Height > 20 && Width > 60;

        /// <summary>
        /// The height of the selection box.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// The width of the selection box.
        /// </summary>
        public double Width { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                SetEnd(interaction.MousePosition.X, interaction.MousePosition.Y);
            }
        }

        public void SetEnd(double x, double y)
        {
            _endX = x;
            _endY = y;
            SetRectangleBounds();
        }

        public void SetStart(double x, double y)
        {
            _startX = x;
            _startY = y;
            SetRectangleBounds();
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.IsAltKeyPressed
                && !interaction.IsCtrlKeyPressed
                && !interaction.IsShiftKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            SetStart(interaction.MousePosition.X, interaction.MousePosition.Y);
            SetEnd(interaction.MousePosition.X, interaction.MousePosition.Y);
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var left = diagram.GetDiagramPointFromViewPointX(X);
            var top = diagram.GetDiagramPointFromViewPointY(Y);
            var right = diagram.GetDiagramPointFromViewPointX(X + Width);
            var bottom = diagram.GetDiagramPointFromViewPointY(Y + Height);
            var nodesToWire = diagram.Nodes.Where(node =>
                 node.X > left && node.X + node.Width < right
                 && node.Y > top && node.Y + node.Height < bottom).ToList();
            var autoWirer = new NodeAutoWirer();
            autoWirer.AutoWireNodes(diagram, nodesToWire);
        }

        private void SetRectangleBounds()
        {
            X = Math.Min(_startX, _endX);
            Y = Math.Min(_startY, _endY);
            Width = Math.Abs(_startX - _endX);
            Height = Math.Abs(_startY - _endY);
        }
    }
}