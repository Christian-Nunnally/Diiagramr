using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrAPI.Editor.Interactors
{
    public class LassoDiagramNodeCreator : DiagramInteractor
    {
        private readonly INodeProvider _nodeProvider;
        private double _endX;
        private double _endY;
        private double _startX;
        private double _startY;

        public LassoDiagramNodeCreator(Func<INodeProvider> nodeProviderFactory)
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
            var interactionRectangle = GetDiagramPontsInteractionRectangle(diagram);
            DiagramState diagramState = SaveDiagramState(diagram, interactionRectangle);
            DeleteNodes(diagram, diagramState.Nodes);

            var diagramNode = _nodeProvider.CreateNodeFromName(typeof(DiagramNode).FullName) as DiagramNode;
            diagram.AddNode(diagramNode);
            diagramNode.WhenResolved(d => RecreateNodesOnDiagram(d, diagram, diagramState, diagramNode));
            Point nodePosition = CalculateNodePositionToCenterInLasso(interactionRectangle, diagramNode);
            diagramNode.X = nodePosition.X;
            diagramNode.Y = nodePosition.Y;
        }

        private static Point CalculateNodePositionToCenterInLasso(Rect interactionRectangle, DiagramNode diagramNode)
        {
            var centerOfLassoInDiagramPointX = interactionRectangle.Right - (interactionRectangle.Width / 2);
            var centerOfLassoInDiagramPointY = interactionRectangle.Bottom - (interactionRectangle.Height / 2);
            var halfDiagramNodeWidth = diagramNode.Width / 2;
            var halfDiagramNodeHeight = diagramNode.Height / 2;
            var nodePointX = centerOfLassoInDiagramPointX - halfDiagramNodeWidth - Diagram.NodeBorderWidth;
            var nodePointY = centerOfLassoInDiagramPointY - halfDiagramNodeHeight - Diagram.NodeBorderWidth;
            var nodePoint = new Point(nodePointX, nodePointY);
            return nodePoint;
        }

        private static void DeleteNodes(Diagram diagram, IEnumerable<Node> nodesToMove)
        {
            var unWireAndDeleteCommand = new UnwireAndDeleteNodeCommand(diagram);
            foreach (var node in nodesToMove)
            {
                unWireAndDeleteCommand.Execute(node);
            }
        }

        private DiagramState SaveDiagramState(Diagram diagram, Rect interactionRectangle)
        {
            var nodesToMove = diagram.Nodes.Where(node =>
                             node.X > interactionRectangle.Left && node.X + node.Width < interactionRectangle.Right
                             && node.Y > interactionRectangle.Top && node.Y + node.Height < interactionRectangle.Bottom).ToList();
            var nodesToMoveModels = nodesToMove.Select(n => n.Model);
            var allWires = nodesToMove.SelectMany(n => n.Terminals).SelectMany(t => t.TerminalModel.ConnectedWires).Distinct();
            var internalWires = allWires.Where(w => nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) && nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));
            var outputWires = allWires.Where(w => nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) && !nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));
            var inputWires = allWires.Where(w => !nodesToMoveModels.Contains(w.SourceTerminal.ParentNode) && nodesToMoveModels.Contains(w.SinkTerminal.ParentNode));
            var internalWireStates = StoreWireConnections(internalWires);
            var inputWireStates = StoreWireConnections(inputWires);
            var outputWireStates = StoreWireConnections(outputWires);
            return new DiagramState
            {
                Nodes = nodesToMove,
                InternalWireStates = internalWireStates,
                InputWireStates = inputWireStates,
                OutputWireStates = outputWireStates
            };
        }

        private Rect GetDiagramPontsInteractionRectangle(Diagram diagram)
        {
            var left = diagram.GetDiagramPointFromViewPointX(X);
            var top = diagram.GetDiagramPointFromViewPointY(Y);
            var right = diagram.GetDiagramPointFromViewPointX(X + Width);
            var bottom = diagram.GetDiagramPointFromViewPointY(Y + Height);
            return new Rect(left, top, Math.Abs(right - left), Math.Abs(bottom - top));
        }

        private IEnumerable<WireState> StoreWireConnections(IEnumerable<WireModel> wires)
        {
            return wires.Select(w => new WireState { Wire = w, SinkTerminal = w.SinkTerminal, SourceTerminal = w.SourceTerminal }).ToArray();
        }

        private void RecreateNodesOnDiagram(Diagram diagram, Diagram outerDiagram, DiagramState diagramState, DiagramNode diagramNode)
        {
            AddNodesToDiagram(diagram, diagramState.Nodes);
            RewireNodes(diagram, diagramState.InternalWireStates);
            CreateInputs(diagram, outerDiagram, diagramState.InputWireStates, diagramNode);
            CreateOutputs(diagram, outerDiagram, diagramState.OutputWireStates, diagramNode);
        }

        private void CreateOutputs(Diagram diagram, Diagram outerDiagram, IEnumerable<WireState> outputWireStates, DiagramNode diagramNode)
        {
            foreach (var outputWireState in outputWireStates)
            {
                var outputNode = _nodeProvider.CreateNodeFromName(typeof(DiagramOutputNode).FullName) as DiagramOutputNode;
                outputNode.X = outputWireState.SinkTerminal.ParentNode.X;
                outputNode.Y = outputWireState.SinkTerminal.ParentNode.Y;
                diagram.AddNode(outputNode);

                var wireCommand = new WireToTerminalCommand(diagram, outputNode.Terminals.First().TerminalModel);
                wireCommand.Execute(outputWireState.SourceTerminal);

                var newDiagramNodeTerminal = diagramNode.Terminals.Last().TerminalModel;
                wireCommand = new WireToTerminalCommand(outerDiagram, newDiagramNodeTerminal);
                wireCommand.Execute(outputWireState.SinkTerminal);
            }
        }

        private void CreateInputs(Diagram diagram, Diagram outerDiagram, IEnumerable<WireState> inputWireStates, DiagramNode diagramNode)
        {
            foreach (var inputWireState in inputWireStates)
            {
                var inputNode = _nodeProvider.CreateNodeFromName(typeof(DiagramInputNode).FullName) as DiagramInputNode;
                inputNode.X = inputWireState.SourceTerminal.ParentNode.X;
                inputNode.Y = inputWireState.SourceTerminal.ParentNode.Y;
                diagram.AddNode(inputNode);

                var wireCommand = new WireToTerminalCommand(diagram, inputNode.Terminals.First().TerminalModel);
                wireCommand.Execute(inputWireState.SinkTerminal);

                var newDiagramNodeTerminal = diagramNode.Terminals.Last().TerminalModel;
                wireCommand = new WireToTerminalCommand(outerDiagram, newDiagramNodeTerminal);
                wireCommand.Execute(inputWireState.SourceTerminal);
            }
        }

        private void RewireNodes(Diagram diagram, IEnumerable<WireState> storedWiringInformation)
        {
            foreach (var wireState in storedWiringInformation)
            {
                var wireCommand = new WireToTerminalCommand(diagram, wireState.SinkTerminal);
                wireCommand.Execute(wireState.SourceTerminal);
            }
        }

        private void AddNodesToDiagram(Diagram diagram, IEnumerable<Node> nodes)
        {
            nodes.ForEach(diagram.AddNode);
        }

        private void SetRectangleBounds()
        {
            X = Math.Min(_startX, _endX);
            Y = Math.Min(_startY, _endY);
            Width = Math.Abs(_startX - _endX);
            Height = Math.Abs(_startY - _endY);
        }

        private struct WireState
        {
            public WireModel Wire;
            public TerminalModel SinkTerminal;
            public TerminalModel SourceTerminal;
        }

        private struct DiagramState
        {
            public IEnumerable<Node> Nodes;
            public IEnumerable<WireState> InternalWireStates;
            public IEnumerable<WireState> InputWireStates;
            public IEnumerable<WireState> OutputWireStates;
        }
    }
}