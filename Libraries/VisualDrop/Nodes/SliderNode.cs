using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace VisualDrop
{
    public class SliderNode : Node
    {
        public SliderNode()
        {
            Width = 90;
            Height = 30;
            Name = "Slider";
        }

        [OutputTerminal(Direction.South)]
        public float Output { get; set; }

        [InputTerminal(Direction.West)]
        public float Min { get; set; } = 0f;

        [InputTerminal(Direction.East)]
        public float Max { get; set; } = 1f;
    }
}