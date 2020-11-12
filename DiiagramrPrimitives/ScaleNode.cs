using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class ScaleNode : Node
    {
        private float _value;
        private float _scaleFactor;

        public ScaleNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Scale";
        }

        public string ViewLabelText { get; set; }

        [InputTerminal(Direction.North)]
        public float Value
        {
            get => _value;
            set
            {
                ScaledValue = value * _scaleFactor;
                _value = value;
            }
        }

        [InputTerminal(Direction.West)]
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                ViewLabelText = $"x{value}";
                ScaledValue = value * _value;
                _scaleFactor = value;
            }
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public float ScaledValue { get; set; }
    }
}