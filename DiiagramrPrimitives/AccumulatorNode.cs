using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// Node that adds its input to its output.
    /// </summary>
    [Help("Whenever the value of the input terminal changes, this node adds the input to the output. Triggering the reset terminal will set the output back to 0")]
    public class AccumulatorNode : Node
    {
        private float _input;

        /// <summary>
        /// Creates a new instance of <see cref="AccumulatorNode"/>.
        /// </summary>
        public AccumulatorNode()
        {
            Width = 30;
            Height = 30;
            Name = "Accumulator";
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        [Help("The sum of all values that have come in to the input since the last time the reset terminal was triggered.")]
        public float Result { get; set; }

        [InputTerminal(Direction.East)]
        [Help("Resets the value of the result terminal to 0 when triggered.")]
        public bool ResetResult { get => false; set => Result = 0; }

        [InputTerminal(Direction.North)]
        [Help("Adds this value to the current value on the result terminal.")]
        public float Input
        {
            get => _input;
            set
            {
                _input = value;
                Result += _input;
            }
        }
    }
}