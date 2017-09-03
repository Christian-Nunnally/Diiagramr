using System;
using System.Collections.Generic;
using System.Linq;
using Diiagramr.Service;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Stylet;

namespace Diiagramr.ViewModel
{
    public class NodeSelectorViewModel : Screen
    {
        private readonly Dictionary<string, Type> _nodeNameToViewModelMap = new Dictionary<string, Type>();

        public NodeSelectorViewModel(Func<IEnumerable<AbstractNodeViewModel>> availibleNodes)
        {
            AvailibleNodeViewModels = new BindableCollection<AbstractNodeViewModel>();
            availibleNodes.Invoke().ForEach(AddNode);
        }

        public AbstractNodeViewModel SelectedNode { get; set; }

        public BindableCollection<AbstractNodeViewModel> AvailibleNodeViewModels { get; set; }

        public void AddNode(AbstractNodeViewModel abstractNodeViewModel)
        {
            _nodeNameToViewModelMap.Add(abstractNodeViewModel.Name, abstractNodeViewModel.GetType());
            AvailibleNodeViewModels.Add(abstractNodeViewModel);
        }

        public void SelectNode(object arg)
        {
            var selectedNode = AvailibleNodeViewModels.First(x => x == arg);
            if (selectedNode == null) return;
            SelectedNode = selectedNode;
        }

        public void SelectNode(Type type)
        {
            var selectedNode = AvailibleNodeViewModels.First(x => x.GetType() == type);
            if (selectedNode == null) return;
            SelectedNode = selectedNode;
        }

        public AbstractNodeViewModel CreateNewInstanceOfSelectedNode()
        {
            return SelectedNode == null ? null : ConstructAbstractNodeViewModel(SelectedNode.Name);
        }

        public AbstractNodeViewModel ConstructAbstractNodeViewModel(string name)
        {
            var viewModel = Activator.CreateInstance(_nodeNameToViewModelMap[name]) as AbstractNodeViewModel;
            viewModel.InitializeWithNode(new DiagramNode(name));
            viewModel.ConstructTerminals();
            return viewModel;
        }

        public AbstractNodeViewModel ConstructAbstractNodeViewModel(DiagramNode node)
        {
            var viewModel = Activator.CreateInstance(_nodeNameToViewModelMap[node.NodeType]) as AbstractNodeViewModel;
            viewModel.InitializeWithNode(node);
            viewModel.X = node.X;
            viewModel.Y = node.Y;
            return viewModel;
        }
    }
}