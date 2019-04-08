namespace DiiagramrAPI.Diagram.Nodes
{
    public class AddNode : Node
    {
        private TypedTerminal<float> _inputTerminal1;
        private TypedTerminal<float> _inputTerminal2;
        private TypedTerminal<float> _outputTerminal;

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Add");
            _inputTerminal1 = setup.InputTerminal<float>("X", Direction.North);
            _inputTerminal2 = setup.InputTerminal<float>("Y", Direction.North);
            _outputTerminal = setup.OutputTerminal<float>("X + Y", Direction.South);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(float data)
        {
            var result = _inputTerminal1.Data + _inputTerminal2.Data;
            _outputTerminal.Data = result;
        }
    }
}
