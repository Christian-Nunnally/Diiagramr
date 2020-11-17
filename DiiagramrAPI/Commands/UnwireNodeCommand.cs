using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Linq;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that removes all of the wires connected to a node.
    /// </summary>
    public class UnwireNodeCommand : TransactingCommand
    {
        private readonly Diagram _diagram;
        private readonly IReversableCommand _removeWireCommand;

        /// <summary>
        /// Creates a new instance of <see cref="UnwireNodeCommand"/>.
        /// </summary>
        /// <param name="diagram">The diagram to unwire the node from.</param>
        public UnwireNodeCommand(Diagram diagram)
        {
            _diagram = diagram;
            _removeWireCommand = new DeleteWireCommand(_diagram);
        }

        /// <inheritdoc/>
        protected override void Execute(ITransactor transactor, object parameter)
        {
            if (parameter is Node node)
            {
                foreach (var wire in node.Terminals.SelectMany(t => t.Model.ConnectedWires).ToArray())
                {
                    transactor.Transact(_removeWireCommand, wire);
                }
            }
        }
    }
}