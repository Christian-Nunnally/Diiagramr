using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A factory that creates instances of <see cref="Diagram"/>.
    /// </summary>
    public class DiagramFactory
    {
        private readonly INodeProvider _nodeProvidor;
        private readonly IEnumerable<DiagramInteractor> diagramInteractors;

        /// <summary>
        /// Creates a new instance of <see cref="DiagramFactory"/>.
        /// </summary>
        /// <param name="nodeProvidorFactory">A factory that creates a <see cref="NodeProvider"/> instance.</param>
        /// <param name="diagramInteractorsFactory">A factory that creates a list of <see cref="IDiagramInteractor"/> to add to newly created diagrams.</param>
        public DiagramFactory(
            Func<INodeProvider> nodeProvidorFactory,
            Func<IEnumerable<IDiagramInteractor>> diagramInteractorsFactory)
        {
            _nodeProvidor = nodeProvidorFactory.Invoke();
            diagramInteractors = diagramInteractorsFactory.Invoke().OfType<DiagramInteractor>();
        }

        /// <summary>
        /// Creates a new diagram view model instance.
        /// </summary>
        /// <param name="diagram">The diagram model to create the view model from.</param>
        /// <returns>The newly created <see cref="Diagram"/>.</returns>
        public Diagram CreateDiagramViewModel(DiagramModel diagram)
        {
            return new Diagram(diagram, _nodeProvidor, diagramInteractors);
        }
    }
}