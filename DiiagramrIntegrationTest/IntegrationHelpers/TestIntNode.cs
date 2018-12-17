using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestIntNode : PluginNode
    {
        private int _value;

        public Terminal<int> OutputTerminal { get; set; }

        [PluginNodeSetting]
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        protected override void SetupNode(NodeSetup setup)
        {
            OutputTerminal = setup.OutputTerminal<int>("testOut", Direction.East);
        }

        public void SetValue(int value)
        {
            OutputTerminal.Data = value;
            Value = value;
        }
    }
}