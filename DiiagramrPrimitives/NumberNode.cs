using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    public class NumberNode : Node
    {
        public NumberNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Number";
            ResizeEnabled = true;
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public int Number { get; set; } = 64;

        public string StringValue
        {
            get => Number.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Number = result;
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