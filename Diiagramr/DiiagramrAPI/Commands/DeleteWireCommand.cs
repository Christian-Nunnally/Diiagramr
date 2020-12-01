using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// Undoable command that deletes a single wire.
    /// </summary>
    public class DeleteWireCommand : IReversableCommand
    {
        private readonly Diagram _diagram;

        /// <summary>
        /// Creates a new instance of <see cref="DeleteWireCommand"/>.
        /// </summary>
        /// <param name="diagram">The diagram to delete the wire from.</param>
        public DeleteWireCommand(Diagram diagram)
        {
            _diagram = diagram;
        }

        /// <inheritdoc/>
        public Action Execute(object parameter)
        {
            if (parameter is WireModel wire && wire.SourceTerminal != null && wire.SinkTerminal != null)
            {
                if (wire.SourceTerminal != null && wire.SinkTerminal != null)
                {
                    var sourceTerminal = wire.SourceTerminal;
                    var sinkTerminal = wire.SinkTerminal;
                    sourceTerminal.DisconnectWire(wire, sinkTerminal);
                    _diagram.RemoveWire(wire);
                    return () =>
                    {
                        sourceTerminal.ConnectWire(wire, sinkTerminal);
                        _diagram.AddWire(wire);
                    };
                }
                _diagram.RemoveWire(wire);
            }

            return () => { };
        }
    }
}