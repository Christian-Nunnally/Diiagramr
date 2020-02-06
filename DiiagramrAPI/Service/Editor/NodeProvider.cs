﻿using DiiagramrAPI.Editor.Diagrams;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DiiagramrAPI.Service.Editor
{
    public class NodeProvider : INodeProvider
    {
        private readonly IList<Node> _availableNodeViewModels;
        private readonly IDictionary<string, NodeLibrary> _dependencyMap;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap;
        private readonly NodeServiceProvider _nodeServiceProvider;

        public NodeProvider(Func<NodeServiceProvider> nodeServiceProviderFactory)
        {
            _nodeServiceProvider = nodeServiceProviderFactory();
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
            return CreateNodeFromModel(node);
        }

        public IEnumerable<Node> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }

        public Node CreateNodeFromModel(NodeModel node)
        {
            var fullName = node.Name;
            if (fullName == null)
            {
                return null;
            }

            Node newNode = InstantiateNode(fullName);
            newNode.AttachToModel(node);
            newNode.SetServiceProvider(_nodeServiceProvider);
            ResolveTerminalTypes(newNode);
            return newNode;
        }

        public void RegisterNode(Node node, NodeLibrary dependency)
        {
            _loadedAssemblies.Add(node.GetType().Assembly);
            var fullName = node.GetType().FullName ?? string.Empty;
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
                throw new InvalidOperationException($"Node registered twice with {GetType().AssemblyQualifiedName}");
            }

            _nodeNameToViewModelMap.Add(fullName, node.GetType());
            _availableNodeViewModels.Add(node);
            _dependencyMap.Add(fullName, dependency);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddNodes"));
        }

        private Node InstantiateNode(string fullName)
        {
            if (!(Activator.CreateInstance(GetViewModelTypeFromName(fullName)) is Node newNode))
            {
                throw NoViewModelException(fullName);
            }
            return newNode;
        }

        private Exception NoViewModelException(string typeFullName)
        {
            return new InvalidOperationException($"{GetType().AssemblyQualifiedName} tried to load unregistered view model '{typeFullName}'");
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
            viewModel.Terminals.Select(t => t.TerminalModel).ForEach(ResolveTerminalType);
        }

        private Type TypeResolver(Assembly assembly, string name, bool ignore)
        {
            return assembly == null
                ? Type.GetType(name, true, ignore)
                : assembly.GetType(name, true, ignore);
        }
    }
}