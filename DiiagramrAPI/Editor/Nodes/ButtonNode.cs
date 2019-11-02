using DiiagramrModel;

namespace DiiagramrAPI.Editor.Nodes
{
    public class ButtonNode : Node
    {
        private TypedTerminal<bool> _outputTerminal;

        public string Value { get; set; }
        public string Result { get; set; }
        public bool IsPressed { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Button");
            _outputTerminal = setup.OutputTerminal<bool>("Trigger", Direction.South);
        }

        public void Trigger()
        {
            _outputTerminal.Data = true;
            _outputTerminal.Data = false;
            IsPressed = true;
        }

        public void Unpress()
        {
            IsPressed = false;
        }
    }
}
