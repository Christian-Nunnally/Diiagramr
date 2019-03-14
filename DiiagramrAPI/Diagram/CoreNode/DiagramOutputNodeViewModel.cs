using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Diagram.CoreNode
{
    public class DiagramOutputNodeViewModel : IoNode
    {
        public Terminal<object> InputTerminal;

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