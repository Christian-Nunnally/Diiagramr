using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Service
{
    public class DiagramFactory
    {
        private readonly IProvideNodes _nodeProvidor;
        private readonly IEnumerable<DiagramInteractor> diagramInteractors;

        public DiagramFactory(
            Func<IProvideNodes> nodeProvidorFactory,
            Func<IEnumerable<IDiagramInteractorService>> _diagramInteractorsFactory)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            diagramInteractors = _diagramInteractorsFactory.Invoke().OfType<DiagramInteractor>();
        }

        public Diagram.Diagram CreateDiagramViewModel(DiagramModel diagram)
        {
            return new Diagram.Diagram(diagram, _nodeProvidor, diagramInteractors);
        }
    }
}
