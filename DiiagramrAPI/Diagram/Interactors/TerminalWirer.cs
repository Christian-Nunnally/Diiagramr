﻿using System.Linq;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class TerminalWirer : DiagramInteractor
    {
        public TerminalWirer()
        {
            Weight = 0.75;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            if (interaction.Type == InteractionType.LeftMouseDown)
            {
                if (interaction.ViewModelMouseIsOver is Terminal terminal)
                {
                    var selectedTerminals = diagram.NodeViewModels.SelectMany(n => n.TerminalViewModels.Where(t => t.IsSelected));
                    if (selectedTerminals.Count() == 1)
                    {
                        var selectedTerminal = selectedTerminals.First();
                        if (selectedTerminal == terminal)
                        {
                            terminal.IsSelected = false;
                            return;
                        }
                        selectedTerminal.WireToTerminal(terminal.Model);
                        diagram.UnHighlightAllTerminals();
                        diagram.UnselectTerminals();
                        return;
                    }
                    diagram.UnselectTerminals();
                    diagram.UnselectNodes();
                    terminal.IsSelected = true;
                    diagram.HighlightTerminalsOfSameType(terminal.Model);
                }
                else
                {
                    diagram.UnHighlightAllTerminals();
                    diagram.UnselectNodes();
                }
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelMouseIsOver is Terminal;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
