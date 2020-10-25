using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class InputSwitcherNode : Node
    {
        private object _cachedInput1;
        private object _cachedInput2;
        private object _cachedInput3;

        public InputSwitcherNode() : base()
        {
            Width = 90;
            Height = 30;
            Name = "Input Switcher";
            ResizeEnabled = true;
        }

        public int InputIndex { get; private set; }

        [OutputTerminal(Direction.South)]
        public object Output { get; set; }

        [InputTerminal(Direction.North)]
        public void Input1(object value)
        {
            if (InputIndex == 0)
            {
                Output = value;
                _cachedInput1 = value;
            }
        }

        [InputTerminal(Direction.North)]
        public void Input2(object value)
        {
            if (InputIndex == 1)
            {
                Output = value;
                _cachedInput2 = value;
            }
        }

        [InputTerminal(Direction.North)]
        public void Input3(object value)
        {
            if (InputIndex == 2)
            {
                Output = value;
                _cachedInput3 = value;
            }
        }

        [InputTerminal(Direction.West)]
        public void SwitchOutput(bool value)
        {
            if (value)
            {
                InputIndex = (InputIndex + 1) % 3;
                switch (InputIndex)
                {
                    case 0:
                        Output = _cachedInput1;
                        break;

                    case 1:
                        Output = _cachedInput2;
                        break;

                    case 2:
                        Output = _cachedInput3;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}