using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
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