using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    public class NumberNode : Node
    {
        public NumberNode()
        {
            Width = 30;
            Height = 30;
            Name = "Number";
            ResizeEnabled = true;
        }

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
    }
}