using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class AddNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal1;
        private Terminal<int> _inputTerminal2;
        private Terminal<int> _outputTerminal;

        public int Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Add Node");
            setup.EnableResize();
            _inputTerminal1 = setup.InputTerminal<int>("Input", Direction.East);
            _inputTerminal2 = setup.InputTerminal<int>("Input", Direction.West);
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void Action(object o)
        {
            if (o != null)
            {
                InputTerminalOnDataChanged((int)o);
            }
        }

        private void InputTerminalOnDataChanged(int data)
        {
            _outputTerminal.Data = Value;
            Value = _inputTerminal1.Data + _inputTerminal2.Data;

            foreach (var dynamicTerminal in DynamicTerminalViewModels)
            {
                Value += (int)(dynamicTerminal.Data ?? 0);
            }
        }
    }
}