using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class DiagramCallNodeViewModel : PluginNodeViewModel
    {

        public override string Name { get; }

        public EDiagram EDiagram { get; set; }

        public override void SetupNode(NodeSetup setup)
        {
            throw new System.NotImplementedException();
        }
    }
}
