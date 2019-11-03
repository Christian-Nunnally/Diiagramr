using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class DivideNode : Node
    {
        private float _a;
        private float _b;

        [OutputTerminal(nameof(Result), Direction.South)]
        public string Result { get; set; }

        [InputTerminal("A", Direction.North)]
        public void InputA(float a)
        {
            _a = a;
            Output(_a / _b, nameof(Result));
        }

        [InputTerminal("B", Direction.North)]
        public void InputB(float b)
        {
            _b = b;
            Output(_a / _b, nameof(Result));
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Divide");
        }
    }
}