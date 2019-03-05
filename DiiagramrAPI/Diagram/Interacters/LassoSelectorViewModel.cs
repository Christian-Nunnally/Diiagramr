using System;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interacters
{
    public class LassoSelectorViewModel : DiagramInteracter
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

        public override bool ShouldInteractionStart(InteractionEventArguments interaction)
        {
            return false;
        }

        public override bool ShouldInteractionStop(InteractionEventArguments interaction)
        {
            return interaction.Interaction == InteractionType.KeyUp 
                && (interaction.Key == Key.LeftCtrl || interaction.Key == Key.RightCtrl);
        }
    }
}
