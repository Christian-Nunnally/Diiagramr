using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Nodes
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

        [NodeSettingAttribute]
        public float Value { get; set; }

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

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Number");
            setup.EnableResize();
            _outputTerminal = setup.OutputTerminal<float>("Number", Direction.South);
            _outputTerminal.Data = Value;
        }
    }
}