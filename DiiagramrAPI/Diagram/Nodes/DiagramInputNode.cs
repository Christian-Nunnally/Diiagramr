namespace DiiagramrAPI.Diagram.Nodes
{
    public class DiagramInputNode : IoNode
    {
        public TypedTerminal<object> OutputTerminal;

        public void TerminalDataChanged(object data)
        {
            OutputTerminal.Data = data;
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Input");
            OutputTerminal = setup.OutputTerminal<object>("Data out", Direction.South);
        }
    }
}
