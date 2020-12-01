using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// Simple node that scales its input by a specific amount.
    /// </summary>
    [Help("Scales the input by the factor specified by the scale factor input terminal.")]
    public class ScaleNode : Node
    {
        private float _value;
        private float _factor;

        /// <summary>
        /// Creates a new instance of <see cref="ScaleNode"/>.
        /// </summary>
        public ScaleNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Scale";
        }

        /// <summary>
        /// The string to display in the node.
        /// </summary>
        public string ViewLabelText { get; set; }

        [Help("The input value to scale by the factor specified by the scale factor input terminal.")]
        [InputTerminal(Direction.North)]
        public float Value
        {
            get => _value;
            set
            {
                ScaledValue = value * _factor;
                _value = value;
            }
        }

        [Help("The scale factor to multiply the input terminal by when setting the value on the scaled value terminal.")]
        [InputTerminal(Direction.West)]
        public float Factor
        {
            get => _factor;
            set
            {
                ViewLabelText = $"x{value}";
                ScaledValue = value * _value;
                _factor = value;
            }
        }

        [Help("The value of the input terminal scaled by the value on the factor terminal.")]
        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public float ScaledValue { get; set; }
    }
}