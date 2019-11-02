using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;

namespace DiiagramrAPI.Editor.Nodes
{
    [HideFromNodeSelector]
    public class DiagramOutputNode : IoNode
    {
        public TypedTerminal<object> InputTerminal;

        public event TerminalDataChangedDelegate<object> DataChanged
        {
            add
            {
                _dataChanged += value;
                value.Invoke(InputTerminal.Data);
            }
            remove
            {
                _dataChanged -= value;
            }
        }

        private event TerminalDataChangedDelegate<object> _dataChanged;

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Output");
            InputTerminal = setup.InputTerminal<object>("Data in", Direction.North);
            InputTerminal.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(object data)
        {
            _dataChanged?.Invoke(data);
        }
    }
}
