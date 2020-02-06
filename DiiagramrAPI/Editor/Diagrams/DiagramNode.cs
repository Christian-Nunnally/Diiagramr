using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class DiagramNode : Node
    {
        private const int MarginBetweenTerminals = 10;
        private readonly Dictionary<DiagramInputNode, TerminalModel> _inputNodeToTerminalMap = new Dictionary<DiagramInputNode, TerminalModel>();
        private readonly Dictionary<DiagramOutputNode, TerminalModel> _outputNodeToTerminalMap = new Dictionary<DiagramOutputNode, TerminalModel>();
        private Action<Diagram> _whenResolvedAction;
        private Diagram _resolvedDiagram;
        private Action<Diagram> _openDiagramAction;

        public DiagramNode()
        {
            Width = 60;
            Height = 60;
        }

        [NodeSetting]
        public string DiagramName { get; set; }

        private bool IsResolved => ResolvedDiagram != null;

        private Diagram ResolvedDiagram
        {
            get
            {
                return _resolvedDiagram;
            }
            set
            {
                _resolvedDiagram = value;
                if (IsResolved)
                {
                    _whenResolvedAction?.Invoke(ResolvedDiagram);
                    _whenResolvedAction = null;
                    InitializeDiagramNodeWithResolvedDiagram();
                }
                else
                {
                    throw new InvalidOperationException("'Unresolving' the diagram is unsupported");
                }
            }
        }

        public DiagramNode WhenResolved(Action<Diagram> action)
        {
            if (IsResolved)
            {
                action?.Invoke(ResolvedDiagram);
            }
            else
            {
                _whenResolvedAction = action;
            }
            return this;
        }

        protected override void UpdateServices(NodeServiceProvider nodeServiceProvider)
        {
            var projectManager = nodeServiceProvider.GetService<IProjectManager>();
            if (projectManager != null)
            {
                if (DiagramName == null)
                {
                    var diagramModel = new DiagramModel() { Name = "DiagramNodeDiagram" };
                    projectManager.CreateDiagram(diagramModel);
                    DiagramName = diagramModel.Name;
                }
                if (!IsResolved)
                {
                    ResolvedDiagram = projectManager.Diagrams.FirstOrDefault(d => d.Name == DiagramName);
                }
            }

            var projectScreen = nodeServiceProvider.GetService<ProjectScreen>();
            if (projectScreen != null)
            {
                _openDiagramAction = d => projectScreen.OpenDiagram(d);
            }
        }

        protected override void MouseEnteredNode()
        {
            if (IsResolved)
            {
                _openDiagramAction?.Invoke(ResolvedDiagram);
            }
        }

        private void InitializeDiagramNodeWithResolvedDiagram()
        {
            ResolvedDiagram.Nodes.CollectionChanged += ResolvedDiagramNodesCollectionChanged;
            ResolvedDiagram.Nodes.OfType<DiagramInputNode>().ForEach(AddInputTerminalForInputNode);
            ResolvedDiagram.Nodes.OfType<DiagramOutputNode>().ForEach(AddOutputTerminalForOutputNode);
            Name = ResolvedDiagram.Name + "Node";
        }

        private void ResolvedDiagramNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newInputNode in e.NewItems.OfType<DiagramInputNode>())
                {
                    AddInputTerminalForInputNode(newInputNode);
                }

                foreach (var newOutputNode in e.NewItems.OfType<DiagramOutputNode>())
                {
                    AddOutputTerminalForOutputNode(newOutputNode);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var newInputNode in e.OldItems.OfType<DiagramInputNode>())
                {
                    RemoveInputTerminalForInputNode(newInputNode);
                }

                foreach (var newOutputNode in e.OldItems.OfType<DiagramOutputNode>())
                {
                    RemoveOutputTerminalForOutputNode(newOutputNode);
                }
            }
        }

        private void AddOutputTerminalForOutputNode(DiagramOutputNode newOutputNode)
        {
            var outputTerminal = new OutputTerminalModel("Diagram Output", typeof(object), Direction.South);
            newOutputNode.DataChanged += data => outputTerminal.UpdateData(data);
            _outputNodeToTerminalMap.Add(newOutputNode, outputTerminal);
            AddTerminal(outputTerminal);
            UpdateDiagramNodeWidth();
        }

        private void UpdateDiagramNodeWidth()
        {
            var outputTerminalCount = NodeModel.Terminals.OfType<OutputTerminalModel>().Count();
            var inputTerminalCount = NodeModel.Terminals.OfType<InputTerminalModel>().Count();
            var mostTerminals = Math.Max(outputTerminalCount, inputTerminalCount);
            Width = (mostTerminals + 1) * (Terminal.TerminalWidth + MarginBetweenTerminals);
        }

        private void AddInputTerminalForInputNode(DiagramInputNode newInputNode)
        {
            var inputTerminal = new InputTerminalModel("Diagram Input", typeof(object), Direction.North);
            var outputTerminal = (OutputTerminalModel)newInputNode.Terminals.First().TerminalModel;
            inputTerminal.DataChanged += data => outputTerminal.UpdateData(data);
            _inputNodeToTerminalMap.Add(newInputNode, inputTerminal);
            AddTerminal(inputTerminal);
            UpdateDiagramNodeWidth();
        }

        private void RemoveOutputTerminalForOutputNode(DiagramOutputNode newOutputNode)
        {
            var outputTerminal = _outputNodeToTerminalMap[newOutputNode];
            _outputNodeToTerminalMap.Remove(newOutputNode);
            outputTerminal.ConnectedWires.ForEach(w => outputTerminal.DisconnectWire(w, w.SinkTerminal));
            RemoveTerminal(outputTerminal);
            UpdateDiagramNodeWidth();
        }

        private void RemoveInputTerminalForInputNode(DiagramInputNode newInputNode)
        {
            var inputTerminal = _inputNodeToTerminalMap[newInputNode];
            _inputNodeToTerminalMap.Remove(newInputNode);
            inputTerminal.ConnectedWires.ForEach(w => inputTerminal.DisconnectWire(w, w.SourceTerminal));
            RemoveTerminal(inputTerminal);
            UpdateDiagramNodeWidth();
        }
    }
}