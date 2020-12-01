using DiiagramrModel;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A node that represents an input to an entire diagram.
    /// </summary>
    [Help("Provides data from an input terminal on a diagram node that represents the diagram this node is on.")]
    public class DiagramInputNode : IoNode
    {
        /// <summary>
        /// Creates a new instance of <see cref="DiagramInputNode"/>.
        /// </summary>
        public DiagramInputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Input";
        }

        /// <summary>
        /// The output terminal for getting the input data out on the diagram.
        /// </summary>
        [Help("The data coming from an input terminal on a diagram node that represents the diagram this terminal is on.")]
        [OutputTerminal(Direction.South)]
        public object DiagramInput { get; set; }
    }
}