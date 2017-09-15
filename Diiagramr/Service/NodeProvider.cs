using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diiagramr.Service
{
    public class NodeProvider : IProvideNodes
    {
        private readonly IList<AbstractNodeViewModel> _availableNodeViewModels = new List<AbstractNodeViewModel>();
        private readonly IDictionary<string, Type> _nodeNameToViewModelMap = new Dictionary<string, Type>();

        public NodeProvider(Func<IEnumerable<AbstractNodeViewModel>> availibleNodes)
        {
            availibleNodes.Invoke().ForEach(RegisterNode);
        }

        public void RegisterNode(AbstractNodeViewModel node)
        {
            if (_availableNodeViewModels.Contains(node)) return;
            _nodeNameToViewModelMap.Add(node.GetType().FullName, node.GetType());
            _availableNodeViewModels.Add(node);
        }

        public AbstractNodeViewModel LoadNodeViewModelFromNode(DiagramNode node)
        {
            if (!_nodeNameToViewModelMap.ContainsKey(node.NodeFullName)) throw new NodeProviderException($"Tried to load node of type '{node.NodeFullName}' but no view model under that name was registered");
            var viewModel = Activator.CreateInstance(_nodeNameToViewModelMap[node.NodeFullName]) as AbstractNodeViewModel;
            if (viewModel == null) throw new NodeProviderException($"Error creating a view model for node of type '{node.NodeFullName}'");

            viewModel.InitializeWithNode(node);
            viewModel.X = node.X;
            viewModel.Y = node.Y;
            viewModel.Width = node.Width != 0 ? node.Width : viewModel.Width;
            viewModel.Height = node.Height != 0 ? node.Height : viewModel.Height;
            return viewModel;
        }

        public AbstractNodeViewModel CreateNodeViewModelFromName(string typeFullName)
        {
            var node = new DiagramNode(typeFullName);
            return LoadNodeViewModelFromNode(node);
        }

        public IEnumerable<AbstractNodeViewModel> GetRegisteredNodes()
        {
            return _availableNodeViewModels.ToArray();
        }
    }

    public class NodeProviderException : Exception
    {
        public NodeProviderException(string message) : base(message) { }
    }
}
