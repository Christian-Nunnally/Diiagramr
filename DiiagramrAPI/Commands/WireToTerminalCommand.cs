using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Commands
{
    public class WireToTerminalCommand : ICommand
    {
        private readonly Diagram _diagram;
        private readonly TerminalModel _fromTerminal;
        private readonly ICommand _removeWireCommand;
        private bool _animateWireWhenLoaded;

        public WireToTerminalCommand(Diagram diagram, TerminalModel fromTerminal, bool animateWireWhenLoaded = false)
        {
            _diagram = diagram;
            _fromTerminal = fromTerminal;
            _animateWireWhenLoaded = animateWireWhenLoaded;
            _removeWireCommand = new DeleteWireCommand(_diagram);
        }

        public Action Execute(object parameter)
        {
            if (parameter is TerminalModel toTerminal)
            {
                var wire = new WireModel();
                _fromTerminal.ConnectWire(wire, toTerminal);
                var wireViewModel = new Wire(wire)
                {
                    DoAnimationWhenViewIsLoaded = _animateWireWhenLoaded
                };
                _diagram.AddWire(wireViewModel);
                return () =>
                {
                    _fromTerminal.DisconnectWire(wire, toTerminal);
                    _diagram.RemoveWire(wireViewModel);
                };
            }
            return () => { };
        }
    }
}