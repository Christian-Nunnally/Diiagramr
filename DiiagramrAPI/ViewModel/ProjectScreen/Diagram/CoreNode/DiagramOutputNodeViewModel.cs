﻿using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class DiagramOutputNodeViewModel : IoNode
    {
        public Terminal<object> InputTerminal;

        public event TerminalDataChangedDelegate<object> DataChanged;

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Output");
            InputTerminal = setup.InputTerminal<object>("Data in", Direction.North);
            InputTerminal.DataChanged += InputTerminalOnDataChanged;
        }

        private void InputTerminalOnDataChanged(object data)
        {
            DataChanged?.Invoke(data);
        }
    }
}