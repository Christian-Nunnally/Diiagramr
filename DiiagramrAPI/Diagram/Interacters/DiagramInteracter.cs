using System;
using Stylet;

namespace DiiagramrAPI.Diagram.Interacters
{
    public abstract class DiagramInteracter : Screen
    {
        public double X { get; set; }
        public double Y { get; set; }
        public virtual bool Visible { get; set; }

        public abstract bool ShouldInteractionStart(InteractionEventArguments interaction);
        public abstract bool ShouldInteractionStop(InteractionEventArguments interaction);
    }
}
