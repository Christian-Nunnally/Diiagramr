using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DiiagramrAPI.Project
{
    public class DiagramWell : Conductor<Diagram>.StackNavigation, IDiagramViewer
    {
        private readonly IProjectManager _projectManager;
        private List<Diagram> _attachedDiagrams = new List<Diagram>();

        public DiagramWell(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;
        }

        public void OpenDiagram(Diagram diagram)
        {
            if (diagram == null)
            {
                return;
            }
            diagram.ExecuteWhenViewLoaded(() => diagram.ResetPanAndZoom());
            ActiveItem = diagram;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (_projectManager.Project?.Diagrams is object)
            {
                var diagram = _projectManager.Diagrams.FirstOrDefault();
                if (diagram == null)
                {
                    _projectManager.CreateDiagram();
                }
                else
                {
                    OpenDiagram(diagram);
                }

                _projectManager.Diagrams.CollectionChanged += DiagramsCollectionChanged;
                DiagramsCollectionChanged(null, null);
            }
        }

        private void DiagramsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var attachedDiagram in _attachedDiagrams)
            {
                attachedDiagram.Viewer = null;
            }

            _attachedDiagrams.Clear();

            foreach (var diagram in _projectManager.Diagrams)
            {
                diagram.Viewer = this;
                _attachedDiagrams.Add(diagram);
            }
        }
    }
}