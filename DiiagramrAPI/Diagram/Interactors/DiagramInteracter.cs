using Stylet;

namespace DiiagramrAPI.Diagram.Interactors
{
    /// <summary>
    /// A specific interaction sequence to be performed on the <see cref="Diagram"/> in response to user input.
    /// A <see cref="DiagramInteractionManager"/> manages the lifecycle of <see cref="DiagramInteractor"/>'s. 
    /// </summary>
    public abstract class DiagramInteractor : Screen, IDiagramInteractorService
    {
        public double Weight { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public abstract bool ShouldStartInteraction(DiagramInteractionEventArguments interaction);
        public abstract void StartInteraction(DiagramInteractionEventArguments interaction);
        public abstract bool ShouldStopInteraction(DiagramInteractionEventArguments interaction);
        public abstract void StopInteraction(DiagramInteractionEventArguments interaction);
        public abstract void ProcessInteraction(DiagramInteractionEventArguments interaction);
    }
}
