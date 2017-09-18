using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class NumberNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal;
        private Terminal<int> _outputTerminal;

        [PluginNodeSetting]
        public int Value { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.NodeName("Number Node");
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);
        }

        public void Add1()
        {
            Value++;
            _outputTerminal.Data = Value;
        }

        public void Sub1()
        {
            Value--;
            _outputTerminal.Data = Value;
        }
    }
}