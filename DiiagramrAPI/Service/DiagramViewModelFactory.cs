using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Service
{
    public class DiagramViewModelFactory
    {
        private readonly ColorTheme _colorTheme;
        private readonly IProvideNodes _nodeProvidor;
        private readonly Func<NodeSelectorViewModel> _nodeSelectorViewModelFactory;
        private readonly IEnumerable<DiagramInteractor> diagramInteractors;

        public DiagramViewModelFactory(
            Func<IProvideNodes> nodeProvidorFactory, 
            Func<NodeSelectorViewModel> nodeSelectorViewModelFactory, 
            Func<IEnumerable<IDiagramInteractorService>> _diagramInteractorsFactory,
            ColorTheme colorTheme)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            _nodeSelectorViewModelFactory = nodeSelectorViewModelFactory;
            diagramInteractors = _diagramInteractorsFactory.Invoke().OfType<DiagramInteractor>();
            _colorTheme = colorTheme;
        }

        public DiagramViewModel CreateDiagramViewModel(DiagramModel diagram)
        {
            var nodeSelectorViewModel = _nodeSelectorViewModelFactory.Invoke();
            return new DiagramViewModel(diagram, _nodeProvidor, _colorTheme, nodeSelectorViewModel, diagramInteractors);
        }
    }
}
