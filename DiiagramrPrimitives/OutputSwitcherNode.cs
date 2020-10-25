using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class OutputSwitcherNode : Node
    {
        public OutputSwitcherNode() : base()
        {
            Width = 90;
            Height = 30;
            Name = "Output Switcher";
            ResizeEnabled = true;
        }

        public int OutputIndex { get; set; }

        [OutputTerminal(Direction.South)]
        public object Output1 { get; set; }

        [OutputTerminal(Direction.South)]
        public object Output2 { get; set; }

        [OutputTerminal(Direction.South)]
        public object Output3 { get; set; }

        [InputTerminal(Direction.West)]
        public void SwitchOutput(bool value)
        {
            if (value)
            {
                OutputIndex = (OutputIndex + 1) % 3;
            }
        }

        [InputTerminal(Direction.North)]
        public void Input(object value)
        {
            switch (OutputIndex)
            {
                case 0:
                    Output1 = value;
                    break;

                case 1:
                    Output2 = value;
                    break;

                case 2:
                    Output3 = value;
                    break;

                default:
                    break;
            }
        }
    }
}