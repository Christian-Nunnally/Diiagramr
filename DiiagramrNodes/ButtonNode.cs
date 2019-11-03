using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class ButtonNode : Node
    {
        [OutputTerminal(nameof(IsPressed), Direction.South)]
        public bool IsPressed { get; set; }

        public void Press()
        {
            Output(true, nameof(IsPressed));
        }

        public void Unpress()
        {
            Output(true, nameof(IsPressed));
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Button");
        }
    }
}