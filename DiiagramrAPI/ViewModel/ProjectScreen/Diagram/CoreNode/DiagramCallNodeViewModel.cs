using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DiiagramrAPI.ViewModel.Diagram.CoreNode
{
    public class DiagramCallNodeViewModel : PluginNode
    {
        private static readonly List<string> DiagramsCopiedDuringCallNodeCreation = new List<string>();
        public static IProjectManager ProjectManager
        {
            private get => s_projectManager;
            set
            {
                if (value == null)
                {
                    return;
                }

                s_projectManager = value;
                ProjectManagerOnLoadActions.ForEach(x => x.Invoke());
                ProjectManagerOnLoadActions.Clear();
            }
        }
        public static IProvideNodes NodeProvider { private get; set; }
        private static readonly List<Action> ProjectManagerOnLoadActions = new List<Action>();

        private DiagramCopier _diagramCopier;

        private readonly Dictionary<DiagramInputNodeViewModel, Terminal<object>> _inputNodeToTerminal = new Dictionary<DiagramInputNodeViewModel, Terminal<object>>();

        private readonly Dictionary<int, TerminalViewModel> _ioNodeIdToTerminalViewModel = new Dictionary<int, TerminalViewModel>();
        private readonly Dictionary<DiagramOutputNodeViewModel, Terminal<object>> _outputNodeToTerminal = new Dictionary<DiagramOutputNodeViewModel, Terminal<object>>();
        private bool _diagramValidated;

        private NodeSetup _nodeSetup;
        private static IProjectManager s_projectManager;

        public DiagramModel ReferencingDiagramModel { get; private set; }

        private DiagramModel InternalDiagramModel { get; set; }
        private DiagramViewModel InternalDiagramViewModel { get; set; }

        [PluginNodeSetting]
        public bool BrokenDueToRecursion { get; set; }

        [PluginNodeSetting]
        public string DiagramName { get; set; }

        public static DiagramCallNodeViewModel CreateDiagramCallNode(DiagramModel diagram)
        {
            var diagramNode = new DiagramCallNodeViewModel();
            if (ProjectManager != null)
            {
                diagramNode.InitializeDiagramCopier(ProjectManager);
            }
            else
            {
                ProjectManagerOnLoadActions.Add(() => diagramNode.InitializeDiagramCopier(ProjectManager));
            }
            var nodeModel = new NodeModel(typeof(DiagramCallNodeViewModel).FullName);
            diagramNode.InitializeWithNode(nodeModel);
            diagramNode.SetReferencingDiagramModelIfNotBroken(diagram);
            return diagramNode;
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
                SetReferencingDiagramModelIfNotBroken(ProjectManager.CurrentDiagrams.First(d => d.DiagramName.Equals(DiagramName)));
            }
        }

        public void SetReferencingDiagramModelIfNotBroken(DiagramModel referencingDiagramModel)
        {
            if (BrokenDueToRecursion)
            {
                return;
            }

            ReferencingDiagramModel = referencingDiagramModel;
            referencingDiagramModel.PropertyChanged += ReferencingDiagramModelPropertyChanged;
            DiagramName = ReferencingDiagramModel.DiagramName;
        }

        private void ReferencingDiagramModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(DiagramName)))
            {
                DiagramName = ReferencingDiagramModel.DiagramName;
                Name = DiagramName + " Call";
            }
        }

        private void CopyReferencingDiagramAvoidingRecursion()
        {
            if (ReferencingDiagramModel == null)
            {
                throw new DiagramCallRecursionException();
            }

            if (DiagramsCopiedDuringCallNodeCreation.Contains(ReferencingDiagramModel.DiagramName))
            {
                throw new DiagramCallRecursionException();
            }

            DiagramsCopiedDuringCallNodeCreation.Add(ReferencingDiagramModel.DiagramName);
            InternalDiagramModel = _diagramCopier.Copy(ReferencingDiagramModel);
            InternalDiagramViewModel = new DiagramViewModel(InternalDiagramModel, NodeProvider, null, null);
            DiagramsCopiedDuringCallNodeCreation.Remove(ReferencingDiagramModel.DiagramName);
        }

        private void InitializeDiagramCopier(IProjectManager projectManager)
        {
            _diagramCopier = new DiagramCopier(projectManager);
        }

        private void SyncTerminals()
        {
            foreach (var ioNode in InternalDiagramViewModel.NodeViewModels.OfType<IoNode>())
            {
                var terminal = GetTerminalForNodeIfItAlreadyExists(ioNode);

                if (ioNode is DiagramInputNodeViewModel inputNode)
                {
                    SyncTerminalForInput(terminal, inputNode);
                }
                else if (ioNode is DiagramOutputNodeViewModel outputNode)
                {
                    SyncTerminalForOutput(terminal, outputNode);
                }
            }
        }

        private Terminal<object> GetTerminalForNodeIfItAlreadyExists(IoNode ioNode)
        {
            if (!_ioNodeIdToTerminalViewModel.ContainsKey(ioNode.Id))
            {
                return null;
            }

            return _nodeSetup.CreateClientTerminal<object>(_ioNodeIdToTerminalViewModel[ioNode.Id]);
        }

        private void SyncTerminalForOutput(Terminal<object> terminal, DiagramOutputNodeViewModel outputNode)
        {
            if (terminal == null)
            {
                terminal = _nodeSetup.OutputTerminal<object>(outputNode.Name, Direction.South);
                _ioNodeIdToTerminalViewModel.Add(outputNode.Id, terminal.UnderlyingTerminal);
            }

            _outputNodeToTerminal.Add(outputNode, terminal);
            outputNode.DataChanged += ValidateDiagramReference;
            outputNode.DataChanged += terminal.ChangeTerminalData;
            terminal.Data = outputNode.InputTerminal.Data;
        }

        private void SyncTerminalForInput(Terminal<object> terminal, DiagramInputNodeViewModel inputNode)
        {
            if (terminal == null)
            {
                terminal = _nodeSetup.InputTerminal<object>(inputNode.Name, Direction.North);
                _ioNodeIdToTerminalViewModel.Add(inputNode.Id, terminal.UnderlyingTerminal);
            }

            _inputNodeToTerminal.Add(inputNode, terminal);
            terminal.DataChanged += ValidateDiagramReference;
            terminal.DataChanged += inputNode.TerminalDataChanged;
        }

        private void DiagramModelOnSemanticsChanged()
        {
            _diagramValidated = false;
            if (ReferencingDiagramModel.Nodes.Select(n => n.NodeViewModel).OfType<IoNode>().Count() == _ioNodeIdToTerminalViewModel.Count)
            {
                return;
            }

            ValidateDiagramReference();
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
        }

        private void AutoSizeNodeToFitTerminals()
        {
            var northTerminalCount = TerminalViewModels.Count(m => m.TerminalModel.Direction == Direction.North);
            var southTerminalCount = TerminalViewModels.Count(m => m.TerminalModel.Direction == Direction.South);
            var northSouthMaxTerminals = Math.Max(northTerminalCount, southTerminalCount);
            var idealWidth = northSouthMaxTerminals * (TerminalViewModel.TerminalDiameter + 5) + DiagramViewModel.NodeBorderWidth * 2;
            Width = Math.Max(Width, idealWidth);
        }

        private void RemoveTerminalsForNoLongerExistingIoNodes()
        {
            var existingNodeIds = InternalDiagramViewModel.NodeViewModels.OfType<IoNode>().Select(n => n.Id);
            foreach (var noLongerExistingIoNodeId in _ioNodeIdToTerminalViewModel.Keys.Except(existingNodeIds))
            {
                RemoveTerminalViewModel(_ioNodeIdToTerminalViewModel[noLongerExistingIoNodeId]);
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

            if (InternalDiagramViewModel.NodeViewModels.OfType<DiagramCallNodeViewModel>().Any(diagramCallNodeViewModel => diagramCallNodeViewModel.BrokenDueToRecursion))
            {
                BrokenDueToRecursion = true;
                Uninitialize();
                return;
            }

            ReferencingDiagramModel.SemanticsChanged += DiagramModelOnSemanticsChanged;
            DiagramName = ReferencingDiagramModel.DiagramName;
            _nodeSetup.NodeName(DiagramName + " Call");
            SyncTerminals();
        }

        public override void Uninitialize()
        {
            if (ReferencingDiagramModel == null)
            {
                return;
            }

            ReferencingDiagramModel.SemanticsChanged -= DiagramModelOnSemanticsChanged;
        }

        public void HandlePreviewMouseDoubleClick()
        {
            ReferencingDiagramModel.IsOpen = true;
        }
    }

    internal class DiagramCallRecursionException : Exception
    {
    }
}