using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Service
{
    public class DiagramFactory
    {
        private readonly ColorTheme _colorTheme;
        private readonly IProvideNodes _nodeProvidor;
        private readonly Func<NodePalette> _nodeSelectorViewModelFactory;
        private readonly IEnumerable<DiagramInteractor> diagramInteractors;

        public DiagramFactory(
            Func<IProvideNodes> nodeProvidorFactory, 
            Func<NodePalette> nodeSelectorViewModelFactory, 
            Func<IEnumerable<IDiagramInteractorService>> _diagramInteractorsFactory,
            ColorTheme colorTheme)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            _nodeSelectorViewModelFactory = nodeSelectorViewModelFactory;
            diagramInteractors = _diagramInteractorsFactory.Invoke().OfType<DiagramInteractor>();
            _colorTheme = colorTheme;
        }

        public Diagram.Diagram CreateDiagramViewModel(DiagramModel diagram)
        {
            var nodeSelectorViewModel = _nodeSelectorViewModelFactory.Invoke();
            return new Diagram.Diagram(diagram, _nodeProvidor, _colorTheme, nodeSelectorViewModel, diagramInteractors);
        }
    }
}
