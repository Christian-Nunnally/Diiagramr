using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Interoperability;
using DiiagramrAPI.Diagram.Model;
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
        private readonly IList<Node> _availableNodeViewModels;
        private readonly IDictionary<string, NodeLibrary> _dependencyMap;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap;

        public NodeProvider()
        {
            _availableNodeViewModels = new List<Node>();
            _nodeNameToViewModelMap = new Dictionary<string, Type>();
            _dependencyMap = new Dictionary<string, NodeLibrary>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Node CreateNodeFromName(string typeFullName)
        {
            if (!_dependencyMap.ContainsKey(typeFullName))
            {
                throw NoViewModelException(typeFullName);
            }

            var node = new NodeModel(typeFullName, _dependencyMap[typeFullName]);
            return LoadNodeViewModelFromNode(node);
        }

        public IEnumerable<Node> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }

        public Node LoadNodeViewModelFromNode(NodeModel node)
        {
            var fullName = node.Name;

            if (fullName == null)
            {
                return null;
            }

            if (!(Activator.CreateInstance(GetViewModelTypeFromName(fullName)) is Node viewModel))
            {
                throw NoViewModelException(fullName);
            }

            viewModel.AttachToModel(node);
            ResolveTerminalTypes(viewModel);
            return viewModel;
        }

        public void RegisterNode(Node node, NodeLibrary dependency)
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

        private static Exception NoViewModelException(string typeFullName)
        {
            return new ProviderException($"Tried to load unregistered view model '{typeFullName}'");
        }

        private Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            return _loadedAssemblies.FirstOrDefault(a => a.FullName == assemblyName.FullName);
        }

        private Type GetViewModelTypeFromName(string fullNodeTypeName)
        {
            if (_nodeNameToViewModelMap.ContainsKey(fullNodeTypeName))
            {
                return _nodeNameToViewModelMap[fullNodeTypeName];
            }

            throw NoViewModelException(fullNodeTypeName);
        }

        private void ResolveTerminalType(TerminalModel terminal)
        {
            terminal.Type = Type.GetType(terminal.TypeName)
                ?? Type.GetType(terminal.TypeName, AssemblyResolver, TypeResolver);
        }

        private void ResolveTerminalTypes(Node viewModel)
        {
            viewModel.Terminals.Select(t => t.Model).ForEach(ResolveTerminalType);
        }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null
                ? Type.GetType(name, true, ignore)
                : assembly.GetType(name, true, ignore);
        }
    }

    public class ProviderException : Exception
    {
        public ProviderException(string message) : base($"Node Provider Exception: {message}") { }
    }
}
