using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that creates a wire between two terminals.
    /// </summary>
    public class WireToTerminalCommand : IReversableCommand
    {
        private readonly Diagram _diagram;
        private readonly TerminalModel _fromTerminal;
        private readonly bool _animateWireWhenLoaded;

        /// <summary>
        /// Creates a new instance of <see cref="WireToTerminalCommand"/>
        /// </summary>
        /// <param name="diagram">The diagram to perform the wiring on.</param>
        /// <param name="fromTerminal">The terminal to wire from.</param>
        /// <param name="animateWireWhenLoaded">Whether to animate the newly added wire.</param>
        public WireToTerminalCommand(Diagram diagram, TerminalModel fromTerminal, bool animateWireWhenLoaded = false)
        {
            _diagram = diagram;
            _fromTerminal = fromTerminal;
            _animateWireWhenLoaded = animateWireWhenLoaded;
        }

        /// <inheritdoc/>
        public Action Execute(object parameter)
        {
            if (parameter is TerminalModel toTerminal)
            {
                var wire = new WireModel();
                _fromTerminal.ConnectWire(wire, toTerminal);
                var wireViewModel = new Wire(wire, new WirePathingAlgorithum())
                {
                    DoAnimationWhenViewIsLoaded = _animateWireWhenLoaded,
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