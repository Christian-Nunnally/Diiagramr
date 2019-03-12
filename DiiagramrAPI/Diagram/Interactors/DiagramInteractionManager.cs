using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class DiagramInteractionManager : Screen
    {
        public DiagramInteractionManager(Func<IEnumerable<DiagramInteractor>> interactorFactory)
        {
            DiagramInteractors = new BindableCollection<DiagramInteractor>(interactorFactory.Invoke());
        }

        public IList<DiagramInteractor> DiagramInteractors { get; set; } = new List<DiagramInteractor>();
        public BindableCollection<DiagramInteractor> ActiveDiagramInteractors { get; set; } = new BindableCollection<DiagramInteractor>();

        public void DiagramInputHandler(DiagramInteractionEventArguments interaction, DiagramViewModel diagram)
        {
            interaction.Diagram = diagram;
            PutViewModelMouseIsOverInInteraction(interaction);
            PutKeyStatesInInteraction(interaction);
            SendInteractionToInteractors(interaction);
        }

        private static void PutViewModelMouseIsOverInInteraction(DiagramInteractionEventArguments interaction)
        {
            var elementMouseIsOver = Mouse.DirectlyOver as FrameworkElement;
            if (!(elementMouseIsOver?.DataContext is Screen viewModelMouseIsOver))
            {
                var contentPresenter = elementMouseIsOver?.DataContext as ContentPresenter;
                viewModelMouseIsOver = contentPresenter?.DataContext as Screen;
            }
            interaction.ViewModelMouseIsOver = viewModelMouseIsOver;
        }

        private static void PutKeyStatesInInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.IsShiftKeyPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift);
            interaction.IsCtrlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            interaction.IsAltKeyPressed = Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt);
        }

        private void SendInteractionToInteractors(DiagramInteractionEventArguments interaction)
        {
            if (!ActiveDiagramInteractors.Any())
            {
                StartInteractionsThatShouldStart(interaction);
            }
            SendInteractionToActiveInteractions(interaction);
            StopActiveInteractionsThatShouldStop(interaction);
        }

        private void StopActiveInteractionsThatShouldStop(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                if (activeInteractor.ShouldStopInteraction(interaction))
                {
                    activeInteractor.StopInteraction(interaction);
                    ActiveDiagramInteractors.Remove(activeInteractor);
                }
            }
        }

        private void SendInteractionToActiveInteractions(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                activeInteractor.ProcessInteraction(interaction);
            }
        }

        private void StartInteractionsThatShouldStart(DiagramInteractionEventArguments interaction)
        {
            foreach (var interactor in DiagramInteractors)
            {
                if (interactor.ShouldStartInteraction(interaction))
                {
                    interactor.StartInteraction(interaction);
                    ActiveDiagramInteractors.Add(interactor);
                }
            }
        }
    }
}
