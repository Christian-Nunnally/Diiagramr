using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Stylet;
using System;
using System.Linq;

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

        public virtual AbstractNodeViewModel SelectedNode { get; set; }

        public BindableCollection<AbstractNodeViewModel> AvailibleNodeViewModels { get; set; }

        public void SelectNode(object arg)
        {
            var selectedNode = AvailibleNodeViewModels.First(x => x == arg);
            if (selectedNode == null) return;
            SelectedNode = selectedNode;
        }
    }
}