using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrModel;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor
{
    public class NodeAutoWirer
    {
        public void AutoWireNodes(Diagram diagram, IList<Node> nodesToWire)
        {
            var topToBottomNodes = nodesToWire.OrderBy(n => n.Y).ToList();
            for (int i = 0; i < topToBottomNodes.Count - 1; i++)
            {
                AutoWireNodes(diagram, topToBottomNodes[i], topToBottomNodes[i + 1]);
            }
        }

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

        public bool TryAutoWireTerminals(Diagram diagram, Terminal terminal, Node nodeToInsert)
        {
            if (terminal != null)
            {
                var terminalsThatCouldBeWired = GetWireableTerminals(terminal, nodeToInsert);
                if (terminalsThatCouldBeWired.Count() == 1)
                {
                    TerminalWirer.TryWireTwoTerminalsOnDiagram(diagram, terminal, terminalsThatCouldBeWired.First(), NullTransactor.Instance, false);
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