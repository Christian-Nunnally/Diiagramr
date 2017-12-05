using System;
using System.Collections.Generic;
using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using System.ComponentModel;

namespace DiiagramrAPI.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<PluginNode> _availableNodeViewModels = new List<PluginNode>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap = new Dictionary<string, Type>();
        private readonly IDictionary<string, NodeLibrary> _dependencyMap = new Dictionary<string, NodeLibrary>();

        public event PropertyChangedEventHandler PropertyChanged;

        public NodeProvider()
        {
            DiagramCallNodeViewModel.NodeProvider = this;
        }

        public void RegisterNode(PluginNode node, NodeLibrary dependency)
        {
            if (_availableNodeViewModels.Contains(node)) return;
            if (_nodeNameToViewModelMap.ContainsKey(node.GetType().FullName)) return;
            _nodeNameToViewModelMap.Add(node.GetType().FullName, node.GetType());
            _availableNodeViewModels.Add(node);

            if (_dependencyMap.ContainsKey(node.GetType().FullName)) throw new InvalidOperationException("Node added twice");
            _dependencyMap.Add(node.GetType().FullName, dependency);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddNodes"));
        }

        public PluginNode LoadNodeViewModelFromNode(NodeModel node)
        {
            if (!_nodeNameToViewModelMap.ContainsKey(node.NodeFullName)) throw new NodeProviderException($"Tried to load node of type '{node.NodeFullName}' but no view model under that name was registered");
            if (!(Activator.CreateInstance(_nodeNameToViewModelMap[node.NodeFullName]) is PluginNode viewModel)) throw new NodeProviderException($"Error creating a view model for node of type '{node.NodeFullName}'");

            viewModel.InitializeWithNode(node);

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
