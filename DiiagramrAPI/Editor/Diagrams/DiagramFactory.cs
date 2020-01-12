using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class DiagramFactory
    {
        private readonly IProvideNodes _nodeProvidor;
        private readonly IEnumerable<DiagramInteractor> diagramInteractors;

        public DiagramFactory(
            Func<IProvideNodes> nodeProvidorFactory,
            Func<IEnumerable<IDiagramInteractorService>> diagramInteractorsFactory)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            diagramInteractors = diagramInteractorsFactory.Invoke().OfType<DiagramInteractor>();
        }

        public Diagram CreateDiagramViewModel(DiagramModel diagram)
        {
            return new Diagram(diagram, _nodeProvidor, diagramInteractors);
        }
    }
}