using System;
using System.Collections.Generic;
using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using StyletIoC.Internal;
using System.ComponentModel;

namespace DiiagramrAPI.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<PluginNode> _availableNodeViewModels = new List<PluginNode>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap = new Dictionary<string, Type>();
        private IPluginWatcher _pluginWatcher;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public NodeProvider(Func<IPluginWatcher> pluginWatcher)
        {
            _pluginWatcher = pluginWatcher.Invoke();
            _pluginWatcher.PropertyChanged += AssembliesOnPropertyChanged;
            RegisterNodesFromAssemblies();
        }

        public IProjectManager ProjectManager { get; set; }

        public void RegisterNode(PluginNode node)
        {
            if (_availableNodeViewModels.Contains(node)) return;
            if (_nodeNameToViewModelMap.ContainsKey(node.GetType().FullName)) return;
            _nodeNameToViewModelMap.Add(node.GetType().FullName, node.GetType());
            _availableNodeViewModels.Add(node);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddNodes"));
        }

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
            var node = new NodeModel(typeFullName);
            return LoadNodeViewModelFromNode(node);
        }

        public IEnumerable<PluginNode> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }

        private void RegisterNodesFromAssemblies()
        {
            foreach (var pluginAssembly in _pluginWatcher.Assemblies)
                foreach (var exportedType in pluginAssembly.ExportedTypes)
                    if (exportedType.Implements(typeof(PluginNode)) && !exportedType.IsAbstract)
                    {
                        RegisterNode((PluginNode)Activator.CreateInstance(exportedType));
                    }
        }

        private void AssembliesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RegisterNodesFromAssemblies();
        }
    }

    public class NodeProviderException : Exception
    {
        public NodeProviderException(string message) : base(message) { }
    }
}
