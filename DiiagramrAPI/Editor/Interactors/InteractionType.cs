namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// The different types of interactions that can happen on a diagram.
    /// </summary>
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
        MouseWheel,
    }
}