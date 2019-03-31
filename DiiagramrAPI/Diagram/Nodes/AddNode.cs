namespace DiiagramrAPI.Diagram.Nodes
{
    public class AddNode : Node
    {
        private TypedTerminal<float> _inputTerminal1;
        private TypedTerminal<float> _inputTerminal2;
        private TypedTerminal<float> _outputTerminal;

        public string Value { get; set; }
        public string Result { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Add");
            setup.EnableResize();
            _inputTerminal1 = setup.InputTerminal<float>("X", Direction.East);
            _inputTerminal2 = setup.InputTerminal<float>("Y", Direction.West);
            _outputTerminal = setup.OutputTerminal<float>("X + Y", Direction.South);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(float data)
        {
            var result = _inputTerminal1.Data + _inputTerminal2.Data;
            Value = $"{_inputTerminal1.Data.ToString("0.0")} + {_inputTerminal2.Data.ToString("0.0")}";
            Result = $"= {result.ToString("0.0")}";
            _outputTerminal.Data = result;
        }
    }
}
