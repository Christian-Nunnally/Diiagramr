using DiiagramrAPI.Editor.Diagrams;
using DiiagramrCore;
using DiiagramrModel;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor
{
    public class TerminalPlacer
    {
        private readonly double _nodeHeight;
        private readonly double _nodeWidth;

        public TerminalPlacer(double nodeHeight, double nodeWidth)
        {
            _nodeHeight = nodeHeight;
            _nodeWidth = nodeWidth;
        }

        public void ArrangeTerminals(IEnumerable<Terminal> terminals)
        {
            ArrangeAllTerminalsOnEdge(Direction.North, terminals);
            ArrangeAllTerminalsOnEdge(Direction.East, terminals);
            ArrangeAllTerminalsOnEdge(Direction.South, terminals);
            ArrangeAllTerminalsOnEdge(Direction.West, terminals);
            terminals.ForEach(t => t.CalculateUTurnLimitsForTerminal(_nodeWidth, _nodeHeight));
        }

        private void PlaceTerminalOnEdge(Terminal terminal, Direction edge, double precentAlongEdge)
        {
            const int extraSpace = 7;
            var widerWidth = _nodeWidth + (extraSpace * 2);
            var tallerHeight = _nodeHeight + (extraSpace * 2);
            switch (edge)
            {
                case Direction.North:
                    terminal.XRelativeToNode = (widerWidth * precentAlongEdge) - extraSpace + Diagram.NodeBorderWidth;
                    terminal.YRelativeToNode = Diagram.NodeBorderWidth;
                    break;

                case Direction.East:
                    terminal.XRelativeToNode = _nodeWidth + Diagram.NodeBorderWidth;
                    terminal.YRelativeToNode = (tallerHeight * precentAlongEdge) - extraSpace + Diagram.NodeBorderWidth;
                    break;

                case Direction.South:
                    terminal.XRelativeToNode = (widerWidth * precentAlongEdge) - extraSpace + Diagram.NodeBorderWidth;
                    terminal.YRelativeToNode = _nodeHeight + Diagram.NodeBorderWidth;
                    break;

                case Direction.West:
                    terminal.XRelativeToNode = Diagram.NodeBorderWidth;
                    terminal.YRelativeToNode = (tallerHeight * precentAlongEdge) - extraSpace + Diagram.NodeBorderWidth;
                    break;
            }
        }

        private void ArrangeAllTerminalsOnEdge(Direction edge, IEnumerable<Terminal> terminals)
        {
            var terminalsOnEdge = terminals.Where(t => t.TerminalModel.DefaultSide == edge).ToArray();
            var increment = 1 / (terminalsOnEdge.Length + 1.0f);
            for (var i = 0; i < terminalsOnEdge.Length; i++)
            {
                PlaceTerminalOnEdge(terminalsOnEdge[i], edge, increment * (i + 1.0f));
                terminalsOnEdge[i].EdgeIndex = i;
            }
        }
    }
}