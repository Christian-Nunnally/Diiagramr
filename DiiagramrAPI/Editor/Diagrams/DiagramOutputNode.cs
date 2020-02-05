using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
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
        public void OutputData(object data)
        {
            DataChanged?.Invoke(data);
        }
    }
}