using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrPrimitives
{
    public class AccumulatorNode : Node
    {
        public AccumulatorNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Accumulator";
            ResizeEnabled = true;
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public int Result { get; set; } = 64;

        public string StringValue
        {
            get => Result.ToString();

            set
            {
                if (int.TryParse(value, out int result))
                {
                    Result = result;
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