using Stylet;

namespace DiiagramrAPI.Diagram.Interacters
{
    public abstract class DiagramInteractor : Screen
    {
        public double X { get; set; }
        public double Y { get; set; }

        public abstract bool ShouldStartInteraction(DiagramInteractionEventArguments interaction);
        public abstract void StartInteraction(DiagramInteractionEventArguments interaction);
        public abstract bool ShouldStopInteraction(DiagramInteractionEventArguments interaction);
        public abstract void StopInteraction(DiagramInteractionEventArguments interaction);
        public abstract void ProcessInteraction(DiagramInteractionEventArguments interaction);
    }
}
