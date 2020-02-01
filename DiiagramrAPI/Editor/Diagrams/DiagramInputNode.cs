using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    [HideFromNodeSelector]
    public class DiagramInputNode : IoNode
    {
        public DiagramInputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Input";
        }

        [OutputTerminal(Direction.South)]
        public object DiagramInput { get; set; }
    }
}