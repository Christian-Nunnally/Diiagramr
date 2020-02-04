using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor.Interactors
{
    public class LassoDiagramNodeCreator : DiagramInteractor
    {
        private readonly IProvideNodes _nodeProvider;
        private double _endX;
        private double _endY;
        private double _startX;
        private double _startY;

        public LassoDiagramNodeCreator(Func<IProvideNodes> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
        }

        public double Height { get; set; }

        public double Width { get; set; }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.MouseMoved)
            {
                SetEnd(interaction.MousePosition.X, interaction.MousePosition.Y);
            }
        }

        public void SetEnd(double x, double y)
        {
            _endX = x;
            _endY = y;
            SetRectangleBounds();
        }

        public void SetStart(double x, double y)
        {
            _startX = x;
            _startY = y;
            SetRectangleBounds();
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown && interaction.IsShiftKeyPressed && interaction.IsCtrlKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseUp;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            SetStart(interaction.MousePosition.X, interaction.MousePosition.Y);
            SetEnd(interaction.MousePosition.X, interaction.MousePosition.Y);
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var left = diagram.GetDiagramPointFromViewPointX(X);
            var top = diagram.GetDiagramPointFromViewPointY(Y);
            var right = diagram.GetDiagramPointFromViewPointX(X + Width);
            var bottom = diagram.GetDiagramPointFromViewPointY(Y + Height);
            var nodesToMove = diagram.Nodes.Where(node =>
                 node.X > left && node.X + node.Width < right
                 && node.Y > top && node.Y + node.Height < bottom).ToList();
            var nodesToMoveModels = nodesToMove.Select(n => n.Model);

            var allWires = nodesToMove
                .SelectMany(n => n.Terminals)
                .SelectMany(t => t.TerminalModel.ConnectedWires)
                .Distinct();

            var internalWires = allWires.Where(w => nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) && nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));
            var outputWires = allWires.Where(w => nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) || !nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));
            var inputWires = allWires.Where(w => !nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) || nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));

            var storedInternalWires = StoreWireConnections(internalWires);
            DeleteNodes(diagram, nodesToMove);

            var diagramNode = _nodeProvider.CreateNodeFromName(typeof(DiagramNode).FullName) as DiagramNode;
            diagramNode.WhenResolved(d => RecreateNodesOnDiagram(d, nodesToMove, storedInternalWires));
            diagramNode.X = left;
            diagramNode.Y = top;
            diagram.AddNode(diagramNode);
        }

        private static void DeleteNodes(Diagram diagram, List<Node> nodesToMove)
        {
            var unWireAndDeleteCommand = new UnwireAndDeleteNodeCommand(diagram);
            foreach (var node in nodesToMove)
            {
                unWireAndDeleteCommand.Execute(node);
            }
        }

        private IEnumerable<(WireModel Wire, TerminalModel SinkTerminal, TerminalModel SourceTerminal)> StoreWireConnections(IEnumerable<WireModel> wires)
        {
            return wires.Select(w => (w, w.SinkTerminal, w.SourceTerminal));
        }

        private void RecreateNodesOnDiagram(
            Diagram diagram,
            IEnumerable<Node> nodes,
            IEnumerable<(WireModel Wire, TerminalModel SinkTerminal, TerminalModel SourceTerminal)> storedInternalWires)
        {
            AddNodesToDiagram(diagram, nodes);
            RewireNodes(diagram, storedInternalWires);
        }

        private void RewireNodes(Diagram diagram, IEnumerable<(WireModel Wire, TerminalModel SinkTerminal, TerminalModel SourceTerminal)> storedWiringInformation)
        {
            foreach (var (Wire, SinkTerminal, SourceTerminal) in storedWiringInformation)
            {
                var wireCommand = new WireToTerminalCommand(diagram, SinkTerminal);
                wireCommand.Execute(SourceTerminal);
            }
        }

        private void AddNodesToDiagram(Diagram diagram, IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                diagram.AddNode(node);
            }
        }

        private void SetRectangleBounds()
        {
            X = Math.Min(_startX, _endX);
            Y = Math.Min(_startY, _endY);
            Width = Math.Abs(_startX - _endX);
            Height = Math.Abs(_startY - _endY);
        }
    }
}