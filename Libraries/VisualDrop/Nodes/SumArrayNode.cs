using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace VisualDrop
{
    public class SumArrayNode : Node
    {
        private float[] _inputArray;

        public SumArrayNode()
        {
            Width = 30;
            Height = 30;
            Name = "Sum Array";
        }

        [OutputTerminal(Direction.South)]
        public float Sum { get; set; }

        [InputTerminal(Direction.North)]
        public float[] InputArray
        {
            get => _inputArray;
            set
            {
                _inputArray = value;
                if (value != null)
                {
                    float sum = 0;
                    foreach (var data in value)
                    {
                        sum += data;
                    }

                    Sum = sum;
                }
                else
                {
                    Sum = 0;
                }
            }
        }
    }
}