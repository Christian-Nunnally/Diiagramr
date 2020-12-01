namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Interface that can be implemented by view models that want to react when the mouse enters or leaves thier view.
    /// </summary>
    internal interface IMouseEnterLeaveReaction
    {
        /// <summary>
        /// Occurs when the mouse enters the view.
        /// </summary>
        void MouseEntered();

        /// <summary>
        /// Occurs when the mouse leaves the view.
        /// </summary>
        void MouseLeft();
    }
}