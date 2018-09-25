using DiiagramrAPI.PluginNodeApi;
using System.Windows;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class PointNodeViewModel : PluginNode
    {
        private Terminal<int> _outputTerminal;

        [PluginNodeSetting]
        public Point Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.NodeName("Number Node");
            _outputTerminal = setup.OutputTerminal<int>("Output", Direction.South);
        }
    }
}