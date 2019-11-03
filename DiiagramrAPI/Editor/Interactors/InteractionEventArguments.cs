using DiiagramrAPI.Editor.Diagrams;
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
        public DiagramInteractionEventArguments(InteractionType type)
        {
            Type = type;
        }

        public Diagram Diagram { get; internal set; }
        public bool IsAltKeyPressed { get; internal set; }
        public bool IsCtrlKeyPressed { get; internal set; }
        public bool IsShiftKeyPressed { get; internal set; }
        public Key Key { get; internal set; }
        public Point MousePosition { get; internal set; }
        public int MouseWheelDelta { get; internal set; }
        public InteractionType Type { get; internal set; }
        public Screen ViewModelUnderMouse { get; internal set; }
    }
}