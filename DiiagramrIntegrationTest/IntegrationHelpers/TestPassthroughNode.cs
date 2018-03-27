using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestPassthroughNode : PluginNode
    {
        public Terminal<int> InputTerminal { get; set; }

        public Terminal<int> OutputTerminal { get; set; }

        public int Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            InputTerminal = setup.InputTerminal<int>("testIn", Direction.West);
            OutputTerminal = setup.OutputTerminal<int>("testOut", Direction.East);
            InputTerminal.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(int data)
        {
            Value = data + 1;
            OutputTerminal.Data = data + 1;
        }
    }
}