using System;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;

namespace DiiagramrAPI.Service
{
    public class DiagramViewModelFactory
    {
        private readonly IProvideNodes _nodeProvidor;
        private readonly Func<NodeSelectorViewModel> _nodeSelectorViewModelFactory;

        public DiagramViewModelFactory(Func<IProvideNodes> nodeProvidorFactory, Func<NodeSelectorViewModel> nodeSelectorViewModelFactory)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            _nodeSelectorViewModelFactory = nodeSelectorViewModelFactory;
        }

        public DiagramViewModel CreateDiagramViewModel(DiagramModel diagram)
        {
            var nodeSelectorViewModel = _nodeSelectorViewModelFactory.Invoke();
            return new DiagramViewModel(diagram, _nodeProvidor, nodeSelectorViewModel);
        }
    }
}