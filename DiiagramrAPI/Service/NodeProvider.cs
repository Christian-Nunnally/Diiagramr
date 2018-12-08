using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DiiagramrAPI.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<PluginNode> _availableNodeViewModels;
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap;
        private readonly IDictionary<string, NodeLibrary> _dependencyMap;

        public event PropertyChangedEventHandler PropertyChanged;

        public NodeProvider()
        {
            // Todo: Code smell
            DiagramCallNodeViewModel.NodeProvider = this;

            _availableNodeViewModels = new List<PluginNode>();
            _nodeNameToViewModelMap = new Dictionary<string, Type>();
            _dependencyMap = new Dictionary<string, NodeLibrary>();
        }

        public void RegisterNode(PluginNode node, NodeLibrary dependency)
        {
            var fullName = node.GetType().FullName ?? "";
            if (_availableNodeViewModels.Contains(node))
            {
                return;
            }

            if (_nodeNameToViewModelMap.ContainsKey(fullName))
            {
                return;
            }

            if (_dependencyMap.ContainsKey(fullName))
            {
                throw new ProviderException("Node registered twice");
            }

            _nodeNameToViewModelMap.Add(fullName, node.GetType());
            _availableNodeViewModels.Add(node);
            _dependencyMap.Add(fullName, dependency);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddNodes"));
        }

        public PluginNode LoadNodeViewModelFromNode(NodeModel node)
        {
            var fullName = node.Name;
            if (!(Activator.CreateInstance(GetViewModelTypeFromName(fullName)) is PluginNode viewModel))
            {
                throw NoViewModelException(fullName);
            }

            viewModel.InitializeWithNode(node);
            return viewModel;
        }

        private Type GetViewModelTypeFromName(string fullNodeTypeName)
        {
            if (_nodeNameToViewModelMap.ContainsKey(fullNodeTypeName))
            {
                return _nodeNameToViewModelMap[fullNodeTypeName];
            }

            throw NoViewModelException(fullNodeTypeName);
        }

        public PluginNode CreateNodeViewModelFromName(string typeFullName)
        {
            if (!_dependencyMap.ContainsKey(typeFullName))
            {
                throw NoViewModelException(typeFullName);
            }

            var node = new NodeModel(typeFullName, _dependencyMap[typeFullName]);
            return LoadNodeViewModelFromNode(node);
        }

        private static Exception NoViewModelException(string typeFullName)
        {
            return new ProviderException($"Tried to load unregistered view model '{typeFullName}'");
        }

        public IEnumerable<PluginNode> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }
    }

    public class ProviderException : Exception
    {
        public ProviderException(string message) : base($"Node Provider Exception: {message}") { }
    }
}
