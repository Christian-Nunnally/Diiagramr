using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrNodes
{
    public class ButtonNode : Node
    {
        public ButtonNode()
        {
            Width = 30;
            Height = 30;
            Name = "Button";
        }

        [OutputTerminal(nameof(IsPressed), Direction.South)]
        public bool IsPressed { get; set; }

        public void Press()
        {
            Output(true, nameof(IsPressed));
        }

        public void Unpress()
        {
            Output(true, nameof(IsPressed));
        }
    }
}