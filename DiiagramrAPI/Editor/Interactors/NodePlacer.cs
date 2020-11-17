using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Allows the user to place a node by moving the mouse to pick a position and then clicking.
    /// </summary>
    public class NodePlacer : DiagramInteractor
    {
        private readonly ITransactor _transactor;

        /// <summary>
        /// Creates a new instance of <see cref="NodePlacer"/>.
        /// </summary>
        /// <param name="transactorFactory">A factory that returns an instance of <see cref="ITransactor"/> to transact with.</param>
        public NodePlacer(Func<ITransactor> transactorFactory)
        {
            Weight = 1.0f;
            _transactor = transactorFactory.Invoke();
        }

        /// <summary>
        /// The node that is being inserted.
        /// </summary>
        public Node InsertingNodeViewModel { get; set; }

        /// <inheritdoc/>
        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                MoveInsertingNode(interaction.Diagram, interaction.MousePosition);
                interaction.Diagram.UpdateDiagramBoundingBox();
                InsertingNodeViewModel.Visible = true;
            }
        }

        /// <inheritdoc/>
        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.NodeInserted;
        }

        /// <inheritdoc/>
        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown || interaction.Type == InteractionType.RightMouseDown;
        }

        /// <inheritdoc/>
        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            InsertingNodeViewModel = interaction.Diagram.Nodes.LastOrDefault();
            interaction.Diagram.ShowSnapGrid = true;
        }

        /// <inheritdoc/>
        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.RightMouseDown)
            {
                var removeNodeCommand = new UnwireAndDeleteNodeCommand(interaction.Diagram);
                removeNodeCommand.Execute(InsertingNodeViewModel);
            }
            else
            {
                InsertingNodeViewModel.X = interaction.Diagram.SnapToGrid(InsertingNodeViewModel.X);
                InsertingNodeViewModel.Y = interaction.Diagram.SnapToGrid(InsertingNodeViewModel.Y);
            }

            _transactor.Transact(new UndoCommand(new UnwireAndDeleteNodeCommand(interaction.Diagram)), InsertingNodeViewModel);
            InsertingNodeViewModel = null;
            interaction.Diagram.ShowSnapGrid = false;
        }

        private void MoveInsertingNode(Diagram diagram, Point mouseLocation)
        {
            if (InsertingNodeViewModel != null)
            {
                InsertingNodeViewModel.X = diagram.GetDiagramPointFromViewPointX(mouseLocation.X) - InsertingNodeViewModel.Width / 2.0 - Diagram.NodeBorderWidth;
                InsertingNodeViewModel.Y = diagram.GetDiagramPointFromViewPointY(mouseLocation.Y) - InsertingNodeViewModel.Height / 2.0 - Diagram.NodeBorderWidth;
            }
        }
    }
}