using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class MultiplyNode : Node
    {
        private float _a;
        private float _b;

        public MultiplyNode()
        {
            Width = 30;
            Height = 30;
            Name = "Multiply";
        }

        [OutputTerminal(nameof(Result), Direction.South)]
        public string Result { get; set; }

        [InputTerminal("A", Direction.North)]
        public void InputA(float a)
        {
            _a = a;
            Output(_a * _b, nameof(Result));
        }

        [InputTerminal("B", Direction.North)]
        public void InputB(float b)
        {
            _b = b;
            Output(_a * _b, nameof(Result));
        }
    }
}