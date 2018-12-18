using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<PluginNode> _availableNodeViewModels;
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap;
        private readonly IDictionary<string, NodeLibrary> _dependencyMap;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();

        public event PropertyChangedEventHandler PropertyChanged;

        public NodeProvider()
        {
            _availableNodeViewModels = new List<PluginNode>();
            _nodeNameToViewModelMap = new Dictionary<string, Type>();
            _dependencyMap = new Dictionary<string, NodeLibrary>();
        }

        public void RegisterNode(PluginNode node, NodeLibrary dependency)
        {
            _loadedAssemblies.Add(node.GetType().Assembly);
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

            if (fullName == null)
            {
                return null;
            }

            if (!(Activator.CreateInstance(GetViewModelTypeFromName(fullName)) is PluginNode viewModel))
            {
                throw NoViewModelException(fullName);
            }

            viewModel.InitializeWithNode(node);
            ResolveTerminalTypes(viewModel);
            return viewModel;
        }

        private void ResolveTerminalTypes(PluginNode viewModel)
        {
            viewModel.TerminalViewModels.Select(t => t.TerminalModel).ForEach(ResolveTerminalType);
        }

        private void ResolveTerminalType(TerminalModel terminal)
        {
            terminal.Type = Type.GetType(terminal.TypeName)
                ?? Type.GetType(terminal.TypeName, AssemblyResolver, TypeResolver);
        }

        private Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            return _loadedAssemblies.FirstOrDefault(a => a.FullName == assemblyName.FullName);
        }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null
                ? Type.GetType(name, true, ignore)
                : assembly.GetType(name, true, ignore);
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
