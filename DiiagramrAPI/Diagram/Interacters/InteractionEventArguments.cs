using System.Windows;
using System.Windows.Input;
using Stylet;

namespace DiiagramrAPI.Diagram.Interacters
{
    public class DiagramInteractionEventArguments
    {
        public DiagramViewModel Diagram;
        public InteractionType Type;
        public Key Key;
        public Screen ViewModelMouseIsOver;
        public Point MousePosition;
        public bool IsCtrlKeyPressed { get; internal set; }
        public bool IsAltKeyPressed { get; internal set; }
        public bool IsShiftKeyPressed { get; internal set; }
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
        NodeInserted
    }
}
