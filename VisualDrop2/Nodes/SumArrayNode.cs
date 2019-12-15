using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace VisualDrop
{
    public class SumArrayNode : Node
    {
        public SumArrayNode()
        {
            Width = 30;
            Height = 30;
            Name = "Sum Array";
        }

        [OutputTerminal(Direction.South)]
        public float Sum { get; set; }

        [InputTerminal(Direction.North)]
        public void ArrayChanged(byte[] array)
        {
            if (array != null)
            {
                float sum = 0;
                foreach (var data in array)
                {
                    sum += data;
                }

                Sum = sum;
            }
        }
    }
}