using Diiagramr.Model;
using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class DiagramCallNodeViewModel : PluginNode
    {
        public DiagramModel DiagramModel { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeName("Diagram");
            setup.NodeSize(40, 40);
        }
    }
}
