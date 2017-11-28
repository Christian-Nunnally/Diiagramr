using System;
using System.Collections.Generic;
using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using StyletIoC.Internal;
using System.ComponentModel;
using DiiagramrAPI.ViewModel.ShellScreen;
using System.Threading;

namespace DiiagramrAPI.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<PluginNode> _availableNodeViewModels = new List<PluginNode>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap = new Dictionary<string, Type>();
        private readonly IDictionary<string, DependencyModel> _dependencyMap = new Dictionary<string, DependencyModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        // TODO: Remove this.
        public IProjectManager ProjectManager { get; set; }

        public void RegisterNode(PluginNode node, DependencyModel dependency)
        {
            if (_availableNodeViewModels.Contains(node)) return;
            if (_nodeNameToViewModelMap.ContainsKey(node.GetType().FullName)) return;
            _nodeNameToViewModelMap.Add(node.GetType().FullName, node.GetType());
            _availableNodeViewModels.Add(node);
            _dependencyMap.Add(node.GetType().FullName, dependency);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddNodes"));
        }

        // TODO: Refactor this to  handle diagram call nodes better
        public PluginNode LoadNodeViewModelFromNode(NodeModel node)
        {
            if (!_nodeNameToViewModelMap.ContainsKey(node.NodeFullName)) throw new NodeProviderException($"Tried to load node of type '{node.NodeFullName}' but no view model under that name was registered");
            if (!(Activator.CreateInstance(_nodeNameToViewModelMap[node.NodeFullName]) is PluginNode viewModel)) throw new NodeProviderException($"Error creating a view model for node of type '{node.NodeFullName}'");

            viewModel.InitializeWithNode(node);
            if (viewModel is DiagramCallNodeViewModel diagramCallNode)
            {
                if (ProjectManager == null) throw new InvalidOperationException("Diagram call nodes can not be created without being able to resolve the diagram");
                diagramCallNode.NodeProvider = this;
                diagramCallNode.SetReferencingDiagramModelIfNotBroken(ProjectManager.CurrentDiagrams.First(m => m.Name == diagramCallNode.DiagramName));
            }

            viewModel.X = node.X;
            viewModel.Y = node.Y;
            viewModel.Width = node.Width != 0 ? node.Width : viewModel.Width;
            viewModel.Height = node.Height != 0 ? node.Height : viewModel.Height;
            return viewModel;
        }

        public PluginNode CreateNodeViewModelFromName(string typeFullName)
        {
            if (!_dependencyMap.ContainsKey(typeFullName)) throw new NodeProviderException($"Tried to load node of type '{typeFullName}' but no view model under that name was registered");
            var node = new NodeModel(typeFullName, _dependencyMap[typeFullName]);
            return LoadNodeViewModelFromNode(node);
        }

        public IEnumerable<PluginNode> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }
    }

    public class NodeProviderException : Exception
    {
        public NodeProviderException(string message) : base(message) { }
    }
}
