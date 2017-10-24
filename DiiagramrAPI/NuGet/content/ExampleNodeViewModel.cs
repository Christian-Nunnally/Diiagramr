using DiiagramrAPI.PluginNodeApi;

namespace <YOUR NAMESPACE>
{
    public class ExampleNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal;
        private Terminal<int> _outputTermina;

        public int ExampleValue { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.NodeName("Example Node");
            _inputTerminal = setup.InputTerminal<int>("Input", Direction.North);
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);

            _inputTerminal.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(int data)
        {
            ExampleValue = data;
            _outputTerminal.Data = Value;
        }
    }
}