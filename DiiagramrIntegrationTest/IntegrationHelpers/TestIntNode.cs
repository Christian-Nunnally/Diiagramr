using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestIntNode : PluginNode
    {
        public Terminal<int> OutputTerminal { get; set; }

        [PluginNodeSetting]
        public int Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            OutputTerminal = setup.OutputTerminal<int>("testOut", Direction.East);
        }

        public void SetValue(int value)
        {
            Value = value;
            OutputTerminal.Data = value;
        }
    }
}