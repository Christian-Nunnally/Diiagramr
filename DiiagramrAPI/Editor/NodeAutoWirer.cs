using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Automactially figures out if there is a reasonable way to wire nodes together, and then wires them.
    /// </summary>
    public class NodeAutoWirer
    {
        /// <summary>
        /// Automatically figure out a reasonable way to wire the given nodes, and then wire them together.
        /// </summary>
        /// <param name="diagram">The diagram to do the wiring on.</param>
        /// <param name="nodesToWire">The nodes to try to automatically wire together.</param>
        public void AutoWireNodes(Diagram diagram, IList<Node> nodesToWire)
        {
            var topToBottomNodes = nodesToWire.OrderBy(n => n.Y).ToList();
            for (int i = 0; i < topToBottomNodes.Count - 1; i++)
            {
                AutoWireNodes(diagram, topToBottomNodes[i], topToBottomNodes[i + 1]);
            }
        }

        /// <summary>
        /// Automatically figure out a reasonable way to wire the given nodes, and then wire them together.
        /// </summary>
        /// <param name="diagram">The diagram to do the wiring on.</param>
        /// <param name="firstNode">The first node to wire.</param>
        /// <param name="secondNode">The second node to wire.</param>
        public void AutoWireNodes(Diagram diagram, Node firstNode, Node secondNode)
        {
            foreach (var terminal in firstNode.Terminals)
            {
                if (!terminal.IsConnected)
                {
                    if (TryAutoWireTerminals(diagram, terminal, secondNode))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// <summary>
        /// Automatically figure out a reasonable way to wire the given nodes, and then wire them together.
        /// </summary>
        /// <param name="diagram">The diagram to do the wiring on.</param>
        /// <param name="terminal">The terminal to wire to.</param>
        /// <param name="nodeToInsert">The node to wire to.</param>
        /// <returns>True if any connections were made.</returns>
        public bool TryAutoWireTerminals(Diagram diagram, Terminal terminal, Node nodeToInsert)
        {
            if (terminal != null)
            {
                var terminalsThatCouldBeWired = GetWireableTerminals(terminal, nodeToInsert);
                if (terminalsThatCouldBeWired.Count() == 1)
                {
                    TerminalWirer.TryWireTwoTerminalsOnDiagram(diagram, terminal, terminalsThatCouldBeWired.First(), ForgetfulTransactor.Instance, false);
                }
                return terminalsThatCouldBeWired.Any();
            }
            return false;
        }

        private IEnumerable<Terminal> GetWireableTerminals(Terminal startTerminal, Node node)
        {
            if (startTerminal.Model is InputTerminalModel inputTerminal)
            {
                return node.Terminals
                    .OfType<OutputTerminal>()
                    .Where(t => t.Model.CanWireToType(inputTerminal.Type));
            }
            else if (startTerminal.Model is OutputTerminalModel outputTerminal)
            {
                return node.Terminals
                    .OfType<InputTerminal>()
                    .Where(t => t.Model.CanWireFromData(outputTerminal.Data));
            }

            return Enumerable.Empty<Terminal>();
        }
    }
}