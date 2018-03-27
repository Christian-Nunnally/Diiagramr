using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class NumberNodeViewModel : PluginNode
    {
        private Terminal<int> _outputTerminal;

        [PluginNodeSetting]
        public int Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
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