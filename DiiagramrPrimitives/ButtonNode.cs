using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class ButtonNode : Node
    {
        public ButtonNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Button";
            ResizeEnabled = true;
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public bool IsPressed { get; set; }

        public void ButtonMouseDown()
        {
            IsPressed = true;
        }

        public void ButtonMouseUp()
        {
            IsPressed = false;
        }
    }
}