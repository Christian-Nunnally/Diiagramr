using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Stylet;
using System;
using System.Linq;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;

namespace Diiagramr.ViewModel
{
    public class NodeSelectorViewModel : Screen
    {
        private readonly IProvideNodes _nodeProvidor;

        public virtual AbstractNodeViewModel SelectedNode { get; set; }

        public BindableCollection<AbstractNodeViewModel> AvailableNodeViewModels { get; set; }

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            AvailableNodeViewModels = new BindableCollection<AbstractNodeViewModel>();
            _nodeProvidor = nodeProvider.Invoke();
            _nodeProvidor.GetRegisteredNodes().ForEach(AvailableNodeViewModels.Add);

            foreach (var nodeViewModel in AvailableNodeViewModels)
            {
                if (nodeViewModel is PluginNode pluginNode)
                {
                    pluginNode.NodeModel = new NodeModel("");
                    pluginNode.SetupNode(new NodeSetup(pluginNode));
                }
            }
        }

        public void SelectNode(object arg)
        {
            var selectedNode = AvailableNodeViewModels.First(x => x == arg);
            if (selectedNode == null) return;
            SelectedNode = selectedNode;
        }
    }
}