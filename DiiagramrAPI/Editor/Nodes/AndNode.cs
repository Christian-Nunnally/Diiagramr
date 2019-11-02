using DiiagramrModel;

namespace DiiagramrAPI.Editor.Nodes
{
    public class AndNode : Node
    {
        private TypedTerminal<bool> _inputTerminal1;
        private TypedTerminal<bool> _inputTerminal2;
        private TypedTerminal<bool> _outputTerminal;

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("And");
            _inputTerminal1 = setup.InputTerminal<bool>("A", Direction.North);
            _inputTerminal2 = setup.InputTerminal<bool>("B", Direction.North);
            _outputTerminal = setup.OutputTerminal<bool>("A ^ B", Direction.South);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(bool data)
        {
            var result = _inputTerminal1.Data && _inputTerminal2.Data;
            _outputTerminal.Data = result;
        }
    }
}
