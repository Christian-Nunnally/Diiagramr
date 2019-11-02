using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// A <see cref="DiagramInteractionManager"/> manages a list of <see cref="DiagramInteractor"/> objects and handles
    /// user interactions forwarded by a <see cref="Diagram"/>. The manager controls the lifecycle of interactor in the list
    /// and activates them when the appropriate user interactions are received.
    ///
    /// When input is received through a call to <see cref="HandleDiagramInput"/> the manager gives each interactor a chance
    /// to start interacting in order of thier weights. If an interactor chooses to begin its interaction, the manager stops
    /// progressing through the list of lower weighted interactors. When an interactor has begun interacting, it exclusively
    /// receives all user input until it has stopped interacting.
    /// </summary>
    public class DiagramInteractionManager : Screen
    {
        public DiagramInteractionManager(Func<IEnumerable<DiagramInteractor>> interactorFactory)
        {
            var interactors = interactorFactory.Invoke();
            var weighedInteractors = interactors.OrderBy(x => -x.Weight);
            WeightedDiagramInteractors = new BindableCollection<DiagramInteractor>(weighedInteractors);
        }

        public BindableCollection<string> ActiveDiagramInteractorNames { get; } = new BindableCollection<string>();
        public BindableCollection<DiagramInteractor> ActiveDiagramInteractors { get; } = new BindableCollection<DiagramInteractor>();
        public bool ShowDebugInfo { get; set; } = false;
        public IList<DiagramInteractor> WeightedDiagramInteractors { get; } = new List<DiagramInteractor>();

        public void HandleDiagramInput(InteractionType type, Diagram diagram)
        {
            var interaction = new DiagramInteractionEventArguments(type);
            HandleDiagramInput(interaction, diagram);
        }

        public void HandleDiagramInput(DiagramInteractionEventArguments interaction, Diagram diagram)
        {
            interaction.Diagram = diagram;
            PutViewModelMouseIsOverInInteraction(interaction);
            PutKeyStatesInInteraction(interaction);
            SendInteractionToInteractors(interaction);
        }

        private static void PutKeyStatesInInteraction(DiagramInteractionEventArguments interaction)
        {
            interaction.IsShiftKeyPressed = Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift);
            interaction.IsCtrlKeyPressed = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);
            interaction.IsAltKeyPressed = Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt);
        }

        private static void PutViewModelMouseIsOverInInteraction(DiagramInteractionEventArguments interaction)
        {
            var elementMouseIsOver = Mouse.DirectlyOver as FrameworkElement;
            if (!(elementMouseIsOver?.DataContext is Screen viewModelMouseIsOver))
            {
                var contentPresenter = elementMouseIsOver?.DataContext as ContentPresenter;
                viewModelMouseIsOver = contentPresenter?.DataContext as Screen;
            }
            interaction.ViewModelUnderMouse = viewModelMouseIsOver;
        }

        private void SendInteractionToActiveInteractions(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                activeInteractor.ProcessInteraction(interaction);
            }
        }

        private void SendInteractionToInteractors(DiagramInteractionEventArguments interaction)
        {
            if (!ActiveDiagramInteractors.Any())
            {
                StartAndProcessInteractionsThatShouldStart(interaction);
            }
            else
            {
                SendInteractionToActiveInteractions(interaction);
                StopActiveInteractionsThatShouldStop(interaction);
            }
        }

        private void StartAndProcessInteractionsThatShouldStart(DiagramInteractionEventArguments interaction)
        {
            foreach (var interactor in WeightedDiagramInteractors)
            {
                if (interactor.ShouldStartInteraction(interaction))
                {
                    interactor.StartInteraction(interaction);
                    ActiveDiagramInteractors.Add(interactor);
                    ActiveDiagramInteractorNames.Add(interactor.GetType().Name);
                    interactor.ProcessInteraction(interaction);
                    if (!TryStoppingInteraction(interaction, interactor))
                    {
                        break;
                    }
                }
            }
        }

        private void StopActiveInteractionsThatShouldStop(DiagramInteractionEventArguments interaction)
        {
            var activeInteractors = ActiveDiagramInteractors.ToArray();
            foreach (var activeInteractor in activeInteractors)
            {
                TryStoppingInteraction(interaction, activeInteractor);
            }
        }

        private bool TryStoppingInteraction(DiagramInteractionEventArguments interaction, DiagramInteractor activeInteractor)
        {
            var didInteractionStop = activeInteractor.ShouldStopInteraction(interaction);
            if (didInteractionStop)
            {
                activeInteractor.StopInteraction(interaction);
                ActiveDiagramInteractors.Remove(activeInteractor);
                ActiveDiagramInteractorNames.Remove(activeInteractor.GetType().Name);
            }
            return didInteractionStop;
        }
    }
}