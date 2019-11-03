using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrNodes
{
    public class NumberNode : Node
    {
        [NodeSetting]
        [OutputTerminal(nameof(Number), Direction.South)]
        public int Number { get; set; }

        public string StringValue
        {
            get => Number.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Output(result, nameof(Number));
                }
            }
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && View != null)
            {
                (sender as FrameworkElement)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(30, 30);
            setup.NodeName("Number");
            setup.EnableResize();
        }
    }
}