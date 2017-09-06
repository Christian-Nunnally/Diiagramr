using Stylet;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Diiagramr.Service;

namespace Diiagramr.ViewModel
{
    public class DiagramWellViewModel : Conductor<DiagramViewModel>.Collection.OneActive
    {
        private readonly IProjectManager _projectManager;

        public NodeSelectorViewModel NodeSelectorViewModel { get; set; }

        public bool NodeSelectorVisible { get; set; }

        public DiagramWellViewModel(Func<IProjectManager> projectManagerFactory, Func<NodeSelectorViewModel> nodeSelectorViewModelFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            NodeSelectorViewModel = nodeSelectorViewModelFactory.Invoke();
            NodeSelectorViewModel.PropertyChanged += NodeSelectorPropertyChanged;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (_projectManager.CurrentProject != null)
            {
                _projectManager.CurrentDiagrams.CollectionChanged += CurrentDiagramsOnCollectionChanged;
            }
        }

        private void CurrentDiagramsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var oldDiagram in e.OldItems.OfType<EDiagram>())
            {
                oldDiagram.IsOpen = false;
                oldDiagram.PropertyChanged -= DiagramOnPropertyChanged;
            }

            foreach (var newDiagram in e.NewItems.OfType<EDiagram>())
            {
                newDiagram.PropertyChanged += DiagramOnPropertyChanged;
            }
        }

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var diagram = (EDiagram)sender;
            if (e.PropertyName.Equals(nameof(diagram.IsOpen)))
            {
                if (diagram.IsOpen)
                {
                    OpenDiagram(diagram);
                }
                else
                {
                    CloseDiagram(diagram);
                }
            }
        }

        private void CloseDiagram(EDiagram diagram)
        {
            var diagramViewModel = Items.FirstOrDefault(viewModel => viewModel.Diagram == diagram);
            if (diagramViewModel != null)
            {
                CloseItem(diagramViewModel);
            }
        }

        private void NodeSelectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedNode") return;
            ActiveItem.InsertingNodeViewModel = NodeSelectorViewModel.CreateNewInstanceOfSelectedNode();
            if (NodeSelectorViewModel.SelectedNode != null) NodeSelectorVisible = false;
        }

        public void OpenDiagram(EDiagram diagram)
        {
            if (diagram == null) return;
            if (Items.Any(x => x.Name == diagram.Name))
            {
                ActiveItem = Items.First(x => x.Name == diagram.Name);
                return;
            }
            var diagramViewModel = new DiagramViewModel(diagram, NodeSelectorViewModel);
            diagramViewModel.PropertyChanged += DiagramViewModelOnPropertyChanged;
            Items.Add(diagramViewModel);
            ActiveItem = diagramViewModel;
        }

        private void DiagramViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var diagramViewModelSender = sender as DiagramViewModel;
            if (diagramViewModelSender == null) return;
            if (string.Equals(propertyChangedEventArgs.PropertyName, "Name"))
            {
                var oldActiveItem = ActiveItem;
                ActiveItem = diagramViewModelSender;
                if (diagramViewModelSender.Name.Equals(""))
                {
                    CloseActiveDiagram();
                    return;
                }
                ReopenActiveDiagram();
                if (oldActiveItem != diagramViewModelSender)
                    ActiveItem = oldActiveItem;
            }
            else if (propertyChangedEventArgs.PropertyName.Equals("InsertingNodeViewModel"))
            {
                if (ActiveItem.InsertingNodeViewModel == null)
                {
                    NodeSelectorViewModel.SelectedNode = null;
                }
            }
        }

        public void ReopenActiveDiagram()
        {
            var activeDiagram = ActiveItem;
            var indexOfActive = Items.IndexOf(ActiveItem);
            ActiveItem.RequestClose();
            Items.Insert(indexOfActive, activeDiagram);
            ActiveItem = activeDiagram;
        }

        public void CloseActiveDiagram()
        {
            ActiveItem.RequestClose();
        }

        public void RightMouseDown()
        {
            if (ActiveItem == null)
            {
                NodeSelectorViewModel.SelectedNode = null;
                return;
            }
            if (NodeSelectorViewModel.SelectedNode == null) NodeSelectorVisible = true;
            else NodeSelectorViewModel.SelectedNode = null;
        }

        public void LeftMouseDown()
        {
            NodeSelectorVisible = false;
        }

        // TODO: Call this
        /// <summary>
        /// Called right before the project is saved.
        /// </summary>
        public void SavingProject()
        {
            foreach (var diagramViewModel in Items)
            {
                diagramViewModel.SavingProject();
            }
        }
    }
}