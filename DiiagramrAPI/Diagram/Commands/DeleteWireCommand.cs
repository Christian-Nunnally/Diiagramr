﻿using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Shell.Commands;
using System;

namespace DiiagramrAPI.Diagram.Commands
{
    public class DeleteWireCommand : ICommand
    {
        private Diagram _diagram;

        public DeleteWireCommand(Diagram diagram)
        {
            _diagram = diagram;
        }

        public Action Execute(object parameter)
        {
            if (parameter is WireModel wire && wire.SourceTerminal != null && wire.SinkTerminal != null)
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
            return () => { };
        }
    }
}
