using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    [HideFromNodeSelector]
    public class DiagramInputNode : IoNode
    {
        private object diagramOutput;

        [OutputTerminal("External Data", Direction.South)]
        public object DiagramOutput
        {
            get => diagramOutput;
            set
            {
                diagramOutput = value;
            }
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Input");
        }
    }
}