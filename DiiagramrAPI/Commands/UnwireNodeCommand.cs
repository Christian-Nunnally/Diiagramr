using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using System.Linq;

namespace DiiagramrAPI.Commands
{
    public class UnwireNodeCommand : TransactingCommand
    {
        private readonly Diagram _diagram;
        private readonly IReversableCommand _removeWireCommand;

        public UnwireNodeCommand(Diagram diagram)
        {
            _diagram = diagram;
            _removeWireCommand = new DeleteWireCommand(_diagram);
        }

        protected override void Execute(ITransactor transactor, object parameter)
        {
            if (parameter is Node node)
            {
                foreach (var wire in node.Terminals.SelectMany(t => t.TerminalModel.ConnectedWires).ToArray())
                {
                    transactor.Transact(_removeWireCommand, wire);
                }
            }
        }
    }
}