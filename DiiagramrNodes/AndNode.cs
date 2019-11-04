using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class AndNode : Node
    {
        private bool _a;
        private bool _b;

        public AndNode()
        {
            Width = 30;
            Height = 30;
            Name = "And";
        }

        [OutputTerminal(nameof(Result), Direction.South)]
        public bool Result { get; set; }

        [InputTerminal("A", Direction.North)]
        public void InputA(bool a)
        {
            _a = a;
            Output(_a && _b, nameof(Result));
        }

        [InputTerminal("B", Direction.North)]
        public void InputB(bool b)
        {
            _b = b;
            Output(_a && _b, nameof(Result));
        }
    }
}