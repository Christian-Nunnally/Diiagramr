using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace VisualDrop
{
    public class NormalizeNode : Node
    {
        private float _maxValue;
        private float _minValue;

        public NormalizeNode()
        {
            Width = 30;
            Height = 30;
            Name = "Normalize";
        }

        [OutputTerminal(Direction.South)]
        public float Output { get; set; }

        [InputTerminal(Direction.North)]
        public void Input(float value)
        {
            if (value > _maxValue)
            {
                _maxValue = value;
            }
            else if (value < _minValue)
            {
                _minValue = value;
            }
            _maxValue *= .9999f;
            _minValue /= .9999f;
            var newOutput = (value - _minValue) / (_maxValue - _minValue);
            if (newOutput > 1) newOutput = 1;
            if (newOutput < 0) newOutput = 0;
            Output = newOutput;
        }
    }
}