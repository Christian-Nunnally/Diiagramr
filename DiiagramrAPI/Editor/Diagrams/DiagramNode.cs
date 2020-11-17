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
    /// <summary>
    /// A special node that contains an entire diagram. Its inputs and output match the input and output <see cref="IoNode"/>'s on the inner diagram.
    /// </summary>
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

        /// <summary>
        /// Creates a new instance of <see cref="DiagramNode"/>.
        /// </summary>
        public DiagramNode()
        {
            Width = 60;
            Height = 60;
        }

        /// <summary>
        /// The name of the diagram contained by this node.
        /// </summary>
        [NodeSetting]
        public string DiagramName { get; set; }

        /// <summary>
        /// Optionally set this to render a diagram on the node.
        /// </summary>
        public Diagram VisibleDiagram { get; set; }

        /// <summary>
        /// The inner diagram this node represents.
        /// </summary>
        private Diagram ResolvedDiagram
        {
            get => _resolvedDiagram;
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

        /// <summary>
        /// Executes the given action when the inner diagram is first resolved.
        /// </summary>
        /// <param name="action">The action to run only when the diagram is resolved.</param>
        public void WhenResolved(Action<Diagram> action)
        {
            if (IsResolved)
            {
                action?.Invoke(ResolvedDiagram);
            }
            else
            {
                // TODO: this should be a list.
                _whenResolvedAction = action;
            }
        }

        /// <summary>
        /// Opens the inner diagram.
        /// </summary>
        public void OpenDiagram()
        {
            _openDiagramAction?.Invoke(ResolvedDiagram);
        }

        /// <inheritdoc/>
        protected override void ServicesUpdated(NodeServiceProvider nodeServiceProvider)
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
            newOutputNode.DataChanged += data => outputTerminal.OnDataSet(data);
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
            inputTerminal.OnDataSet = data => outputTerminal.OnDataSet(data);
            inputTerminal.OnDataGet = () => outputTerminal.OnDataGet();
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