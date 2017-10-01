using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel.Diagram.CoreNode
{
    public class DiagramInputNodeViewModel : IoNode
    {
        public Terminal<object> OutputTerminal;

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Input");
            OutputTerminal = setup.OutputTerminal<object>("Data out", Direction.South);
        }

        public void TerminalDataChanged(object data)
        {
            OutputTerminal.Data = data;
        }
    }
}