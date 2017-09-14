﻿using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class DemoNodeViewModel : PluginNode
    {
        private Terminal<int> _inputTerminal;
        private Terminal<int> _outputTerminal;

        public override string Name => "Demo Node";

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            _inputTerminal = setup.InputTerminal<int>("Input", Direction.North);
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);
        }
    }
}