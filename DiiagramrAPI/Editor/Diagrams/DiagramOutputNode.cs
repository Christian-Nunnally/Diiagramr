using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [HideFromNodeSelector]
    public class DiagramOutputNode : IoNode
    {
        public event Action<object> DataChanged;

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Output");
        }

        [InputTerminal("Output Data", Direction.North)]
        private void OutputData(object data)
        {
            DataChanged?.Invoke(data);
        }
    }
}