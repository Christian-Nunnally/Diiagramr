using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DiiagramrAPI.Diagram.Nodes
{
    public class DiagramCallNode : Node
    {
        // TODO: Come up with cleaner recursion detection method so we can get rid of this.
        private static readonly List<string> DiagramsCopiedDuringCallNodeCreation = new List<string>();

        private readonly DiagramCopier _diagramCopier = new DiagramCopier(ProjectManager);
        private readonly Dictionary<DiagramInputNode, TypedTerminal<object>> _inputNodeToTerminal = new Dictionary<DiagramInputNode, TypedTerminal<object>>();
        private readonly Dictionary<int, Terminal> _ioNodeIdToTerminalViewModel = new Dictionary<int, Terminal>();
        private readonly IProvideNodes _nodeProvider;
        private readonly Dictionary<DiagramOutputNode, TypedTerminal<object>> _outputNodeToTerminal = new Dictionary<DiagramOutputNode, TypedTerminal<object>>();
        private bool _diagramValidated;
        private NodeSetup _nodeSetup;

        public DiagramCallNode(IProvideNodes nodeProvider)
        {
            _nodeProvider = nodeProvider;
        }

        public static IProjectManager ProjectManager { get; set; }

        [NodeSetting]
        public bool BrokenDueToRecursion { get; set; }

        [NodeSetting]
        public string DiagramName { get; set; }

        public DiagramModel ReferencingDiagramModel { get; private set; }
        private DiagramModel InternalDiagramModel { get; set; }
        private Diagram InternalDiagramViewModel { get; set; }

        public static DiagramCallNode CreateDiagramCallNode(DiagramModel diagram, IProvideNodes nodeProvider)
        {
            var diagramNode = new DiagramCallNode(nodeProvider);
            var nodeModel = new NodeModel(typeof(DiagramCallNode).FullName);
            diagramNode.InitializeWithNode(nodeModel);
            diagramNode.SetReferencingDiagramModelIfNotBroken(diagram);
            return diagramNode;
        }

        public void HandlePreviewMouseDoubleClick()
        {
            ReferencingDiagramModel.Open();
        }

        public override void Initialize()
        {
            try
            {
                CopyReferencingDiagramAvoidingRecursion();
            }
            catch (DiagramCallRecursionException)
            {
                BrokenDueToRecursion = true;
                ReferencingDiagramModel = null;
                ReferencingDiagramModel.PropertyChanged -= ReferencingDiagramModelPropertyChanged;
                return;
            }

            if (InternalDiagramViewModel.NodeViewModels.OfType<DiagramCallNode>().Any(diagramCallNodeViewModel => diagramCallNodeViewModel.BrokenDueToRecursion))
            {
                BrokenDueToRecursion = true;
                Uninitialize();
                return;
            }

            ReferencingDiagramModel.SemanticsChanged += DiagramModelOnSemanticsChanged;
            DiagramName = ReferencingDiagramModel.Name;
            _nodeSetup.NodeName(DiagramName + " Call");
            SyncTerminals();
        }

        public void SetReferencingDiagramModelIfNotBroken(DiagramModel referencingDiagramModel)
        {
            if (BrokenDueToRecursion)
            {
                return;
            }

            ReferencingDiagramModel = referencingDiagramModel;
            referencingDiagramModel.PropertyChanged += ReferencingDiagramModelPropertyChanged;
            DiagramName = ReferencingDiagramModel.Name;
        }

        public override void Uninitialize()
        {
            if (ReferencingDiagramModel == null)
            {
                return;
            }

            ReferencingDiagramModel.SemanticsChanged -= DiagramModelOnSemanticsChanged;
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
            setup.EnableResize();
            _nodeSetup = setup;

            if (ProjectManager == null)
            {
                throw new NullReferenceException("Diagram call nodes need access to the project manager in order to resolve diagrams.");
            }

            if (!string.IsNullOrEmpty(DiagramName))
            {
                SetReferencingDiagramModelIfNotBroken(ProjectManager.CurrentDiagrams.First(d => d.Name.Equals(DiagramName)));
            }
        }

        private void AutoSizeNodeToFitTerminals()
        {
            var northTerminalCount = TerminalViewModels.Count(m => m.Model.Direction == Direction.North);
            var southTerminalCount = TerminalViewModels.Count(m => m.Model.Direction == Direction.South);
            var northSouthMaxTerminals = Math.Max(northTerminalCount, southTerminalCount);
            var idealWidth = northSouthMaxTerminals * (Terminal.TerminalDiameter + 5) + Diagram.NodeBorderWidth * 2;
            Width = Math.Max(Width, idealWidth);
        }

        private bool CanDelayCopy()
        {
            var ioNodeCount = ReferencingDiagramModel.Nodes
                .Select(n => n.NodeViewModel)
                .OfType<IoNode>()
                .Count();
            return ioNodeCount == _ioNodeIdToTerminalViewModel.Count;
        }

        private void CopyReferencingDiagramAvoidingRecursion()
        {
            if (ReferencingDiagramModel == null)
            {
                throw new DiagramCallRecursionException();
            }

            if (DiagramsCopiedDuringCallNodeCreation.Contains(ReferencingDiagramModel.Name))
            {
                throw new DiagramCallRecursionException();
            }

            DiagramsCopiedDuringCallNodeCreation.Add(ReferencingDiagramModel.Name);
            InternalDiagramModel = _diagramCopier.Copy(ReferencingDiagramModel);
            InternalDiagramViewModel = new Diagram(InternalDiagramModel, _nodeProvider, null, null);
            DiagramsCopiedDuringCallNodeCreation.Remove(ReferencingDiagramModel.Name);
        }

        private void CopyTerminalDataAcrossNowThatEventListenersAreInPlace()
        {
            foreach (var node in ReferencingDiagramModel.Nodes)
            {
                foreach (var terminal in node.Terminals.Where(t => t.Kind == TerminalKind.Output))
                {
                    var machingTerminal = InternalDiagramModel.Nodes
                        .SelectMany(n => n.Terminals)
                        .First(t => t.Id == terminal.Id);
                    machingTerminal.Data = terminal.Data;
                }
            }
        }

        private void DiagramModelOnSemanticsChanged()
        {
            _diagramValidated = false;

            if (CanDelayCopy())
            {
                return;
            }

            ValidateDiagramReference();
        }

        private TypedTerminal<object> GetTerminalForNodeIfItAlreadyExists(IoNode ioNode)
        {
            if (!_ioNodeIdToTerminalViewModel.ContainsKey(ioNode.Model.Id))
            {
                return null;
            }

            return _nodeSetup.CreateClientTerminal<object>(_ioNodeIdToTerminalViewModel[ioNode.Model.Id]);
        }

        private void ReferencingDiagramModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(DiagramName)))
            {
                DiagramName = ReferencingDiagramModel.Name;
                Name = DiagramName + " Call";
            }
        }

        private void RemoveTerminalsForNoLongerExistingIoNodes()
        {
            var existingNodeIds = InternalDiagramViewModel.NodeViewModels.OfType<IoNode>().Select(n => n.Model.Id);
            foreach (var noLongerExistingIoNodeId in _ioNodeIdToTerminalViewModel.Keys.Except(existingNodeIds))
            {
                RemoveTerminalViewModel(_ioNodeIdToTerminalViewModel[noLongerExistingIoNodeId]);
            }
        }

        private void SyncTerminalForInput(TypedTerminal<object> terminal, DiagramInputNode inputNode)
        {
            if (terminal == null)
            {
                terminal = _nodeSetup.InputTerminal<object>(inputNode.Name, Direction.North);
                _ioNodeIdToTerminalViewModel.Add(inputNode.Model.Id, terminal.UnderlyingTerminal);
            }

            _inputNodeToTerminal.Add(inputNode, terminal);
            terminal.DataChanged += ValidateDiagramReference;
            terminal.DataChanged += inputNode.TerminalDataChanged;
        }

        private void SyncTerminalForOutput(TypedTerminal<object> terminal, DiagramOutputNode outputNode)
        {
            if (terminal == null)
            {
                terminal = _nodeSetup.OutputTerminal<object>(outputNode.Name, Direction.South);
                _ioNodeIdToTerminalViewModel.Add(outputNode.Model.Id, terminal.UnderlyingTerminal);
            }

            _outputNodeToTerminal.Add(outputNode, terminal);
            outputNode.DataChanged += ValidateDiagramReference;
            outputNode.DataChanged += terminal.ChangeTerminalData;
            terminal.Data = outputNode.InputTerminal.Data;
        }

        private void SyncTerminals()
        {
            foreach (var ioNode in InternalDiagramViewModel.NodeViewModels.OfType<IoNode>())
            {
                var terminal = GetTerminalForNodeIfItAlreadyExists(ioNode);

                if (ioNode is DiagramInputNode inputNode)
                {
                    SyncTerminalForInput(terminal, inputNode);
                }
                else if (ioNode is DiagramOutputNode outputNode)
                {
                    SyncTerminalForOutput(terminal, outputNode);
                }
            }
        }

        private void UnsyncOldTerminals()
        {
            foreach (var inputNodeAndTerminal in _inputNodeToTerminal)
            {
                inputNodeAndTerminal.Value.DataChanged -= ValidateDiagramReference;
                inputNodeAndTerminal.Value.DataChanged -= inputNodeAndTerminal.Key.TerminalDataChanged;
            }

            foreach (var outputNodeAndTerminal in _outputNodeToTerminal)
            {
                outputNodeAndTerminal.Key.DataChanged -= ValidateDiagramReference;
                outputNodeAndTerminal.Key.DataChanged -= outputNodeAndTerminal.Value.ChangeTerminalData;
            }

            _inputNodeToTerminal.Clear();
            _outputNodeToTerminal.Clear();
        }

        private void ValidateDiagramReference(object data = null)
        {
            if (_diagramValidated)
            {
                return;
            }

            _diagramValidated = true;
            UnsyncOldTerminals();
            CopyReferencingDiagramAvoidingRecursion();
            RemoveTerminalsForNoLongerExistingIoNodes();
            SyncTerminals();

            AutoSizeNodeToFitTerminals();
            CopyTerminalDataAcrossNowThatEventListenersAreInPlace();

            InternalDiagramModel.Play();
        }
    }

    internal class DiagramCallRecursionException : Exception
    {
    }
}
