﻿using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;

namespace DiiagramrAPI.Editor.Interactors
{
    public class WireDeleter : DiagramInteractor
    {
        private readonly ITransactor _transactor;

        public WireDeleter(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory.Invoke();
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.ViewModelUnderMouse is Wire wire)
            {
                var deleteWireCommand = new DeleteWireCommand(interaction.Diagram);
                _transactor.Transact(deleteWireCommand, wire.Model);
            }
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.IsCtrlKeyPressed
                && interaction.ViewModelUnderMouse is Wire wire;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return true;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}