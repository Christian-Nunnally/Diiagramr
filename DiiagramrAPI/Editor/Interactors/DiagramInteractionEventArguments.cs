using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Windows;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Data class containing information about an interaction that occured on a <see cref="Diagram"/>.
    /// </summary>
    public class DiagramInteractionEventArguments
    {
        /// <summary>
        /// Creates a new instance of <see cref="DiagramInteractionEventArguments"/>.
        /// </summary>
        /// <param name="type">The type of this interaction.</param>
        public DiagramInteractionEventArguments(InteractionType type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets or gets the diagram the event occured on.
        /// </summary>
        public Diagram Diagram { get; internal set; }

        /// <summary>
        /// Gets whether any modifier key was pressed.
        /// </summary>
        public bool IsModifierKeyPressed => IsAltKeyPressed || IsCtrlKeyPressed || IsShiftKeyPressed;

        /// <summary>
        /// Gets whether the alt modifier key was pressed.
        /// </summary>
        public bool IsAltKeyPressed { get; internal set; }

        /// <summary>
        /// Gets whether the control modifier key was pressed.
        /// </summary>
        public bool IsCtrlKeyPressed { get; internal set; }

        /// <summary>
        /// Gets whether the shift modifier key was pressed.
        /// </summary>
        public bool IsShiftKeyPressed { get; internal set; }

        /// <summary>
        /// Gets the key that caused this interaction, if any.
        /// </summary>
        public Key Key { get; internal set; }

        /// <summary>
        /// Gets the position of the mouse at the time of this interaction.
        /// </summary>
        public Point MousePosition { get; internal set; }

        /// <summary>
        /// Gets the amount the mouse wheel changed during this interaction.
        /// </summary>
        public int MouseWheelDelta { get; internal set; }

        /// <summary>
        /// Gets the type of this interaction.
        /// </summary>
        public InteractionType Type { get; internal set; }

        /// <summary>
        /// Gets the view model was under the mouse at the time of this interaction.
        /// </summary>
        public Screen ViewModelUnderMouse { get; internal set; }
    }
}