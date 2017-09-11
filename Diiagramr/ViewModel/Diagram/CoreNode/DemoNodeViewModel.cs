namespace Diiagramr.ViewModel.Diagram
{
    public class DemoNodeViewModel : PluginNodeViewModel
    {
        public override string Name => "Demo Node";

        public override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.InputTerminal("Input", typeof(int), Direction.None, o => null);
            setup.OutputTerminal("Output", typeof(int), Direction.None);
            setup.InputTerminal("Input", typeof(int), Direction.None, o => null);
            setup.OutputTerminal("Output", typeof(int), Direction.None);
        }
    }
}