using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Nodes
{
    public class IndexNode : Node
    {
        private TypedTerminal<byte[]> _arrayTerminal;
        private TypedTerminal<float> _valueTerminal;

        [NodeSettingAttribute]
        public int Index { get; set; }

        public string StringValue
        {
            get => Index.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    if (result >= 0)
                    {
                        Index = result;
                        ArrayTerminalDataChanged(_arrayTerminal.Data);
                    }
                }
            }
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

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Index");
            setup.EnableResize();
            _arrayTerminal = setup.InputTerminal<byte[]>("Array", Direction.North);
            _arrayTerminal.DataChanged += ArrayTerminalDataChanged;

            _valueTerminal = setup.OutputTerminal<float>("Value", Direction.South);
            _valueTerminal.Data = Index;
        }

        private void ArrayTerminalDataChanged(byte[] data)
        {
            if (data != null)
            {
                if (Index < data.Length)
                {
                    _valueTerminal.Data = data[Index];
                }
            }
        }
    }
}