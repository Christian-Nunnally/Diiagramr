using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class AddNode : Node
    {
        public int A { get; set; }

        [OutputTerminal(nameof(Result), Direction.West)]
        public int Result { get; set; }

        [InputTerminal("A", Direction.East)]
        public void Input(int number)
        {
            A = number;
            Output(A, nameof(Result));
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Add");
        }
    }
}