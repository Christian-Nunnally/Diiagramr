using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Nodes
{
    public class NumberNode : Node
    {
        private TypedTerminal<float> _outputTerminal;

        public string StringValue
        {
            get => _outputTerminal.Data.ToString();

            set
            {
                if (float.TryParse(value, out float result))
                {
                    Value = result;
                    _outputTerminal.Data = Value;
                }
            }
        }

        [NodeSetting]
        public float Value { get; set; }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Number");
            setup.EnableResize();
            _outputTerminal = setup.OutputTerminal<float>("Number", Direction.South);
            _outputTerminal.Data = Value;
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (View != null)
                {
                    (sender as FrameworkElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }
}
