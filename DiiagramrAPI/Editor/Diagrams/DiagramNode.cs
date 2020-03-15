using DiiagramrAPI.Editor.Interactors;
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
    [HideFromNodeSelector]
    [Help("Contains another diagram. Passes data from its input terminal(s) to input nodes on the contained diagram, and from output nodes on the contained diagram to its output terminal(s).")]
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

        public Diagram VisibleDiagram { get; set; }

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

                    // Uncomment for some fun with DiagramNodes.
                    // VisibleDiagram = ResolvedDiagram;
                    // VisibleDiagram.Zoom = 0.1;
                }
                else
                {
                    throw new InvalidOperationException("'Unresolving' the diagram is unsupported");
                }
            }
        }

        private bool IsResolved => ResolvedDiagram != null;

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

        public void OpenDiagram()
        {
            _openDiagramAction?.Invoke(ResolvedDiagram);
        }

        protected override void UpdateServices(NodeServiceProvider nodeServiceProvider)
        {
            var projectManager = nodeServiceProvider.GetService<IProjectManager>();
            if (projectManager != null)
            {
                if (DiagramName == null)
                {
                    var diagramModel = new DiagramModel() { Name = "DiagramNodeDiagram" };
                    projectManager.InsertDiagram(diagramModel);
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
            var outputTerminalCount = Model.Terminals.OfType<OutputTerminalModel>().Count();
            var inputTerminalCount = Model.Terminals.OfType<InputTerminalModel>().Count();
            var mostTerminals = Math.Max(outputTerminalCount, inputTerminalCount);
            Width = (mostTerminals + 1) * (Terminal.TerminalWidth + MarginBetweenTerminals);
        }

        private void AddInputTerminalForInputNode(DiagramInputNode newInputNode)
        {
            var inputTerminal = new InputTerminalModel("Diagram Input", typeof(object), Direction.North);
            var outputTerminal = (OutputTerminalModel)newInputNode.Terminals.First().Model;
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