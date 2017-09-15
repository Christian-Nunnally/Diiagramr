using System;
using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class AddNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal1;
        private Terminal<int> _inputTerminal2;
        private Terminal<int> _outputTerminal;

        public int Value { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.NodeName("Add Node");
            _inputTerminal1 = setup.InputTerminal<int>("Input", Direction.East);
            _inputTerminal2 = setup.InputTerminal<int>("Input", Direction.West);
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(int data)
        {
            Value = _inputTerminal1.Data + _inputTerminal2.Data;
            _outputTerminal.Data = Value;
        }
    }
}