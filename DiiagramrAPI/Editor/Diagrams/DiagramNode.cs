using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Editor;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class DiagramNode : Node
    {
        private Action<Diagram> _whenResolvedAction;
        private Diagram _resolvedDiagram;

        public DiagramNode()
        {
            Width = 30;
            Height = 30;
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
        }

        protected override void MouseEnteredNode()
        {
            if (IsResolved)
            {
                ResolvedDiagram.OpenIfViewerAvailable();
            }
        }

        private void InitializeDiagramNodeWithResolvedDiagram()
        {
            ResolvedDiagram.Nodes.CollectionChanged += ResolvedDiagramNodesCollectionChanged;
            ResolvedDiagram.Nodes.OfType<DiagramInputNode>().ForEach(AddInputTerminalForInputNode);
            ResolvedDiagram.Nodes.OfType<DiagramOutputNode>().ForEach(AddOutputTerminalForOutputNode);
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
                // throw new NotImplementedException("Need to support removing terminals from diagram nodes. Probably using an input node to terminal map.");
            }
        }

        private void AddOutputTerminalForOutputNode(DiagramOutputNode newOutputNode)
        {
            var outputTerminal = new InputTerminalModel("Diagram Output", typeof(object), Direction.South);
            newOutputNode.DataChanged += data => outputTerminal.Data = data;
            AddTerminal(outputTerminal);
        }

        private void AddInputTerminalForInputNode(DiagramInputNode newInputNode)
        {
            var inputTerminal = new InputTerminalModel("Diagram Input", typeof(object), Direction.North);
            inputTerminal.DataChanged += data => newInputNode.Terminals.First().Data = data;
            AddTerminal(inputTerminal);
        }
    }
}