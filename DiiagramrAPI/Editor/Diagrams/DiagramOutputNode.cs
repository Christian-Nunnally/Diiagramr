using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [HideFromNodeSelector]
    public class DiagramOutputNode : IoNode
    {
        public DiagramOutputNode()
        {
            Width = 30;
            Height = 30;
            Name = "Output";
        }

        public event Action<object> DataChanged;

        [InputTerminal("Output Data", Direction.North)]
        private void OutputData(object data)
        {
            DataChanged?.Invoke(data);
        }
    }
}