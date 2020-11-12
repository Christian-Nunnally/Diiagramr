using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [Help("Provides data to an output terminal on a diagram node that represents the diagram this node is on.")]
    public class DiagramOutputNode : IoNode
    {
        public DiagramOutputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Output";
        }

        public event Action<object> DataChanged;

        [InputTerminal(Direction.North)]
        public object OutputData
        {
            get => null;
            set => DataChanged?.Invoke(value);
        }
    }
}