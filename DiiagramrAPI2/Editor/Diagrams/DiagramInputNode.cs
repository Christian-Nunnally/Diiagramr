using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    [HideFromNodeSelector]
    public class DiagramInputNode : IoNode
    {
        private object diagramOutput;

        public DiagramInputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Input";
        }

        [OutputTerminal(Direction.South)]
        public object DiagramOutput
        {
            get => diagramOutput;
            set
            {
                diagramOutput = value;
            }
        }
    }
}