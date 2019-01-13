using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using System;

namespace DiiagramrAPI.Service
{
    public class DiagramViewModelFactory
    {
        private readonly IProvideNodes _nodeProvidor;
        private readonly Func<NodeSelectorViewModel> _nodeSelectorViewModelFactory;
        private readonly ColorTheme _colorTheme;

        public DiagramViewModelFactory(Func<IProvideNodes> nodeProvidorFactory, Func<NodeSelectorViewModel> nodeSelectorViewModelFactory, ColorTheme colorTheme)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            _nodeSelectorViewModelFactory = nodeSelectorViewModelFactory;
            _colorTheme = colorTheme;
        }

        public DiagramViewModel CreateDiagramViewModel(DiagramModel diagram)
        {
            var nodeSelectorViewModel = _nodeSelectorViewModelFactory.Invoke();
            return new DiagramViewModel(diagram, _nodeProvidor, _colorTheme, nodeSelectorViewModel);
        }
    }
}
