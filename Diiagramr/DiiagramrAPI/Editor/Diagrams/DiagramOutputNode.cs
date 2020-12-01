using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A node that represents an output from an entire diagram.
    /// </summary>
    [Help("Provides data to an output terminal on a diagram node that represents the diagram this node is on.")]
    public class DiagramOutputNode : IoNode
    {
        /// <summary>
        /// Creates a new instance of <see cref="DiagramOutputNode"/>.
        /// </summary>
        public DiagramOutputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Output";
        }

        /// <summary>
        /// Event that is fired when the data on this output node changes.
        /// </summary>
        public event Action<object> DataChanged;

        /// <summary>
        /// The input terminal for pushing data out through this output node.
        /// </summary>
        [InputTerminal(Direction.North)]
        public object OutputData
        {
            get => null;
            set => DataChanged?.Invoke(value);
        }
    }
}