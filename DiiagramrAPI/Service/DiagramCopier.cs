using DiiagramrAPI.Model;
using System.Collections.Generic;

namespace DiiagramrAPI.Service
{
    public class DiagramCopier
    {
        public DiagramModel Copy(DiagramModel diagram)
        {
            var copiedDiagram = new DiagramModel
            {
                Id = diagram.Id,
                Name = diagram.Name + "_Copy"
            };

            var terminalMap = new Dictionary<TerminalModel, TerminalModel>();
            var wires = new HashSet<WireModel>();
            foreach (var node in diagram.Nodes)
            {
                var copiedNode = new NodeModel(node.Name, node.Dependency)
                {
                    Id = node.Id,
                    Width = node.Width,
                    Height = node.Height,
                    X = node.X,
                    Y = node.Y
                };

                foreach (var terminal in node.Terminals)
                {
                    var copiedTerminal = new TerminalModel(terminal.Name, terminal.Type, terminal.Direction, terminal.Kind, terminal.TerminalIndex)
                    {
                        Data = terminal.Data,
                        Id = terminal.Id
                    };

                    terminalMap.Add(terminal, copiedTerminal);
                    copiedNode.AddTerminal(copiedTerminal);
                    terminal.ConnectedWires.ForEach(w => wires.Add(w));
                }

                copiedDiagram.AddNode(copiedNode);
            }

            foreach (var wire in wires)
            {
                var terminal1 = terminalMap[wire.SinkTerminal];
                var terminal2 = terminalMap[wire.SourceTerminal];
                new WireModel(terminal1, terminal2) { Id = wire.Id };
            }

            return copiedDiagram;
        }
    }
}
