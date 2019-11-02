using DiiagramrAPI.Editor;
using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Application.Commands.Transacting;
using System.Linq;

namespace DiiagramrAPI.Commands
{
    public class UnwireNodeCommand : TransactingCommand
    {
        private readonly Diagram _diagram;
        private readonly ICommand _removeWireCommand;

        public UnwireNodeCommand(Diagram diagram)
        {
            _diagram = diagram;
            _removeWireCommand = new DeleteWireCommand(_diagram);
        }

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