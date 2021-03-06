﻿using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Helper class to position terminals along the border of a node.
    /// </summary>
    public class TerminalPlacer
    {
        private readonly double _nodeHeight;
        private readonly double _nodeWidth;

        /// <summary>
        /// Creates a new instance of <see cref="TerminalPlacer"/>.
        /// </summary>
        /// <param name="nodeHeight">The height of the node to place terminals on.</param>
        /// <param name="nodeWidth">The width of the node to place terminals on.</param>
        public TerminalPlacer(double nodeHeight, double nodeWidth)
        {
            _nodeHeight = nodeHeight;
            _nodeWidth = nodeWidth;
        }

        /// <summary>
        /// Arrange all given terminals around the node.
        /// </summary>
        /// <param name="terminals">The terminals to move.</param>
        public void ArrangeTerminals(IEnumerable<Terminal> terminals)
        {
            ArrangeAllTerminalsOnEdge(Direction.North, terminals);
            ArrangeAllTerminalsOnEdge(Direction.East, terminals);
            ArrangeAllTerminalsOnEdge(Direction.South, terminals);
            ArrangeAllTerminalsOnEdge(Direction.West, terminals);
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
            var terminalsOnEdge = terminals.Where(t => t.Model.DefaultSide == edge).ToArray();
            var increment = 1 / (terminalsOnEdge.Length + 1.0f);
            for (var i = 0; i < terminalsOnEdge.Length; i++)
            {
                PlaceTerminalOnEdge(terminalsOnEdge[i], edge, increment * (i + 1.0f));
            }
        }
    }
}