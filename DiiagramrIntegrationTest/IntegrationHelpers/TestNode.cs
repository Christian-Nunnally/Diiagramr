using Diiagramr.PluginNodeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diiagramr.ViewModel.Diagram;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public class TestNode : PluginNode
    {
        public Terminal<int> InputTerminal { get; set; }
        
        public Terminal<int> OutputTerminal { get; set; }

        public int Value { get; set; }

        public override void SetupNode(NodeSetup setup)
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
