using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    [Help("Provides data from an input terminal on a diagram node that represents the diagram this node is on.")]
    public class DiagramInputNode : IoNode
    {
        public DiagramInputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Input";
        }

        [Help("The data coming from an input terminal on a diagram node that represents the diagram this terminal is on.")]
        [OutputTerminal(Direction.South)]
        public object DiagramInput { get; set; }
    }
}