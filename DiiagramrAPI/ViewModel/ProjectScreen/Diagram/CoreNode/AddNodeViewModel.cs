using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class AddNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal1;
        private Terminal<int> _inputTerminal2;
        private Terminal<int> _outputTerminal;

        public int Value { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(120, 120);
            setup.NodeName("Add Node");
            setup.EnableResize();
            setup.RegisterDynamicTerminalMethod("add", Action);
            _inputTerminal1 = setup.InputTerminal<int>("Input", Direction.East);
            _inputTerminal2 = setup.InputTerminal<int>("Input", Direction.East);
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);

            setup.InputTerminal<string>("Input", Direction.North);
            setup.OutputTerminal<string>("Output", Direction.North);

            setup.InputTerminal<object>("Input", Direction.West);
            setup.OutputTerminal<object>("Output", Direction.West);

            _inputTerminal1.DataChanged += InputTerminalOnDataChanged;
            _inputTerminal2.DataChanged += InputTerminalOnDataChanged;
        }

        private void Action(object o)
        {
            if (o != null) InputTerminalOnDataChanged((int)o);
        }

        private void InputTerminalOnDataChanged(int data)
        {
            Value = _inputTerminal1.Data + _inputTerminal2.Data;

            foreach (var dynamicTerminal in DynamicTerminalViewModels)
            {
                Value += (int)(dynamicTerminal.Data ?? 0);
            }

            _outputTerminal.Data = Value;
        }

        public void AddTerminal()
        {
            CreateDynamicTerminal("input", typeof(int), Direction.North, TerminalKind.Input, "add");
        }
    }
}