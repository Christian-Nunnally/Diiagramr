using Stylet;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// A specific interaction sequence to be performed on the <see cref="Diagram"/> in response to user input.
    /// A <see cref="DiagramInteractionManager"/> manages the lifecycle of <see cref="DiagramInteractor"/>'s.
    /// </summary>
    public abstract class DiagramInteractor : Screen, IDiagramInteractor
    {
        /// <summary>
        /// The weight that determines the order in which interactors get to handle events.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// The X position of this interactor if it has a visible element.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y position of this interactor if it has a visible element.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Processes the given interaction. Only called while this interaction is active.
        /// </summary>
        /// <param name="interaction">The interaction data to process</param>
        public abstract void ProcessInteraction(DiagramInteractionEventArguments interaction);

        /// <summary>
        /// Checks to see if this interaction should stop.
        /// </summary>
        /// <param name="interaction">The interaction data to process</param>
        /// <returns>True if the interaction should start.</returns>
        public abstract bool ShouldStartInteraction(DiagramInteractionEventArguments interaction);

        /// <summary>
        /// Checks to see if this interaction should start.
        /// </summary>
        /// <param name="interaction">The interaction data to process</param>
        /// <returns>True if the interaction should stop.</returns>
        public abstract bool ShouldStopInteraction(DiagramInteractionEventArguments interaction);

        /// <summary>
        /// Starts the interaction.
        /// </summary>
        /// <param name="interaction">The interaction data to process</param>
        public abstract void StartInteraction(DiagramInteractionEventArguments interaction);

        /// <summary>
        /// Ends the interation.
        /// </summary>
        /// <param name="interaction">The interaction data to process</param>
        public abstract void StopInteraction(DiagramInteractionEventArguments interaction);
    }
}