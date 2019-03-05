using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interacters
{
    public struct InteractionEventArguments
    {
        public InteractionType Interaction;
        public Key Key;
        public bool LeftCtrlKeyDown;
        public bool RightCtrlKeyDown;
        public bool CtrlKeyDown => LeftCtrlKeyDown || RightCtrlKeyDown;
    }

    public enum InteractionType
    {
        LeftMouseDown,
        LeftMouseUp,
        RightMouseDown,
        RightMouseUp,
        KeyDown,
        KeyUp
    }
}
