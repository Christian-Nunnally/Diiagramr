using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    /// <summary>
    /// A simple button on the diagram.
    /// </summary>
    [Help("A simple button that can be used to interact with parts of the diagram.")]
    public class ButtonNode : Node
    {
        /// <summary>
        /// Creates a new instance of <see cref="ButtonNode"/>.
        /// </summary>
        public ButtonNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Button";
            ResizeEnabled = true;
        }

        /// <summary>
        /// The mode of the button.
        /// </summary>
        [NodeSetting]
        [InputTerminal(Direction.East)]
        [Help("When true pressing the button will toggle the output. When false the button will act like a momentary switch.")]
        public bool ToggleMode { get; set; }

        /// <summary>
        /// Whether the button is pressed or not.
        /// </summary>
        [NodeSetting]
        [OutputTerminal(Direction.South)]
        [Help("The on/off state of the button.")]
        public bool IsPressed { get; set; }

        /// <summary>
        /// Occurs when the button is pressed.
        /// </summary>
        public void ButtonMouseDown()
        {
            IsPressed = !ToggleMode || !IsPressed;
        }

        /// <summary>
        /// Occurs when the button is released.
        /// </summary>
        public void ButtonMouseUp()
        {
            IsPressed = !ToggleMode || IsPressed;
        }
    }
}