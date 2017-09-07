using System;
using System.Linq;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Stylet;

namespace Diiagramr.ViewModel
{
    public class NodeSelectorViewModel : Screen
    {
        private readonly IProvideNodes _nodeProvidor;

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            AvailibleNodeViewModels = new BindableCollection<AbstractNodeViewModel>();
            _nodeProvidor = nodeProvider.Invoke();
            _nodeProvidor.GetRegisteredNodes().ForEach(AvailibleNodeViewModels.Add);
        }

        public AbstractNodeViewModel SelectedNode { get; set; }

        public BindableCollection<AbstractNodeViewModel> AvailibleNodeViewModels { get; set; }

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
    }
}