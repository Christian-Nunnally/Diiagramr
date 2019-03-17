using Stylet;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramInteractionEventArguments
    {
        public Diagram Diagram;
        public InteractionType Type;
        public Key Key;
        public Screen ViewModelMouseIsOver;
        public Point MousePosition;

        public DiagramInteractionEventArguments(InteractionType type)
        {
            Type = type;
        }

        public bool IsCtrlKeyPressed { get; internal set; }
        public bool IsAltKeyPressed { get; internal set; }
        public bool IsShiftKeyPressed { get; internal set; }
        public int MouseWheelDelta { get; internal set; }
    }

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
}
