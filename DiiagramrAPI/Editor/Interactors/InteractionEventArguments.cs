using Stylet;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    public enum InteractionType
    {
        LeftMouseDown,
        LeftMouseUp,
        RightMouseDown,
        RightMouseUp,
        KeyDown,
        KeyUp,
        MouseMoved,
        None,
        NodeInserted,
        MouseWheel
    }

    public class DiagramInteractionEventArguments
    {
        public Diagram Diagram;
        public Key Key;
        public Point MousePosition;
        public InteractionType Type;
        public Screen ViewModelUnderMouse;

        public DiagramInteractionEventArguments(InteractionType type)
        {
            Type = type;
        }

        public bool IsAltKeyPressed { get; internal set; }
        public bool IsCtrlKeyPressed { get; internal set; }
        public bool IsShiftKeyPressed { get; internal set; }
        public int MouseWheelDelta { get; internal set; }
    }
}