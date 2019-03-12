using System;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class LassoSelectorViewModel : DiagramInteractor
    {
        private double _startX;
        private double _startY;

        private double _endX;
        private double _endY;

        public double Width { get; set; }
        public double Height { get; set; }

        private void SetRectangleBounds()
        {
            X = Math.Min(_startX, _endX);
            Y = Math.Min(_startY, _endY);
            Width = Math.Abs(_startX - _endX);
            Height = Math.Abs(_startY - _endY);
        }

        public void SetStart(double x, double y)
        {
            _startX = x;
            _startY = y;
            SetRectangleBounds();
        }

        public void SetEnd(double x, double y)
        {
            _endX = x;
            _endY = y;
            SetRectangleBounds();
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown && interaction.IsCtrlKeyPressed;
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
            var left = diagram.GetPointRelativeToPanAndZoomX(X);
            var top = diagram.GetPointRelativeToPanAndZoomY(Y);
            var right = diagram.GetPointRelativeToPanAndZoomX(X + Width);
            var bottom = diagram.GetPointRelativeToPanAndZoomY(Y + Height);

            foreach (var node in diagram.NodeViewModels)
            {
                if (node.X > left
                 && node.X + node.Width < right
                 && node.Y > top
                 && node.Y + node.Height < bottom)
                {
                    node.IsSelected = true;
                }
            }
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                SetEnd(interaction.MousePosition.X, interaction.MousePosition.Y);
            }
        }
    }
}
