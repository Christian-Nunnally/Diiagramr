using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Diagram.CoreNode
{
    public class NumberNodeViewModel : PluginNode
    {
        private Terminal<int> _outputTerminal;
        private int _value;

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

        public void Add1()
        {
            _outputTerminal.Data = Value + 1;
            Value++;
        }

        public void Sub1()
        {
            _outputTerminal.Data = Value - 1;
            Value--;
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Number Node");
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);
        }
    }
}
