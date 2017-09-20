using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Service.Interfaces;

namespace Diiagramr.ViewModel
{
    public class DiagramWellViewModel : Conductor<DiagramViewModel>.Collection.OneActive
    {
        private readonly IProjectManager _projectManager;
        private readonly IProvideNodes _nodeProvider;

        public NodeSelectorViewModel NodeSelectorViewModel { get; set; }

        public ObservableCollection<DiagramModel> CurrentDiagrams { get; private set; }

        public bool NodeSelectorVisible { get; set; }

        public DiagramWellViewModel(Func<IProjectManager> projectManagerFactory, Func<IProvideNodes> nodeProviderFactory, Func<NodeSelectorViewModel> nodeSelectorViewModelFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            NodeSelectorViewModel = nodeSelectorViewModelFactory.Invoke();
            NodeSelectorViewModel.PropertyChanged += NodeSelectorPropertyChanged;
            NodeSelectorViewModel.ShouldClose += CloseNodeSelector;

            _nodeProvider = nodeProviderFactory.Invoke();
        }

        private void CloseNodeSelector()
        {
            NodeSelectorVisible = false;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (_projectManager.CurrentProject != null && _projectManager.CurrentDiagrams != null)
            {
                SetCurrentDiagrams(_projectManager.CurrentDiagrams);
            }
        }

        private void SetCurrentDiagrams(ObservableCollection<DiagramModel> diagrams)
        {
            RemoveAllOldDiagrams();
            CurrentDiagrams = diagrams;
            AddAllNewDiagrams();
        }

        private void RemoveAllOldDiagrams()
        {
            if (CurrentDiagrams == null) return;

            foreach (var diagram in CurrentDiagrams)
            {
                diagram.IsOpen = false;
                diagram.PropertyChanged -= DiagramOnPropertyChanged;
            }

            CurrentDiagrams.CollectionChanged -= CurrentDiagramsOnCollectionChanged;
        }

        private void AddAllNewDiagrams()
        {
            foreach (var diagram in _projectManager.CurrentDiagrams)
            {
                diagram.PropertyChanged += DiagramOnPropertyChanged;
            }

            CurrentDiagrams.CollectionChanged += CurrentDiagramsOnCollectionChanged;
        }

        private void CurrentDiagramsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldDiagram in e.OldItems.OfType<DiagramModel>())
                {
                    oldDiagram.IsOpen = false;
                    oldDiagram.PropertyChanged -= DiagramOnPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newDiagram in e.NewItems.OfType<DiagramModel>())
                {
                    newDiagram.PropertyChanged += DiagramOnPropertyChanged;
                }
            }
        }

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var diagram = (DiagramModel)sender;
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

        private void CloseDiagram(DiagramModel diagram)
        {
            var diagramViewModel = Items.FirstOrDefault(viewModel => viewModel.Diagram == diagram);
            if (diagramViewModel != null)
            {
                CloseItem(diagramViewModel);
            }
        }

        private void NodeSelectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
                var selectedNode = NodeSelectorViewModel.SelectedNode;

                if (NodeSelectorViewModel.SelectedNode != null)
                {
                    ActiveItem.InsertingNodeViewModel = _nodeProvider.CreateNodeViewModelFromName(selectedNode.GetType().FullName);
                    NodeSelectorVisible = false;
                }
            }
        }

        private void OpenDiagram(DiagramModel diagram)
        {
            if (diagram == null) return;
            if (Items.Any(x => x.Name == diagram.Name))
            {
                ActiveItem = Items.First(x => x.Name == diagram.Name);
                return;
            }
            var diagramViewModel = _projectManager.DiagramViewModels.First(m => m.Diagram == diagram);
            diagramViewModel.PropertyChanged += DiagramViewModelOnPropertyChanged;
            Items.Insert(0, diagramViewModel);
            
            ActiveItem = diagramViewModel;
        }

        private void DiagramViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var diagramViewModelSender = (DiagramViewModel)sender;
            if (e.PropertyName.Equals(nameof(DiagramViewModel.Name)))
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
            else if (e.PropertyName.Equals(nameof(DiagramViewModel.InsertingNodeViewModel)))
            {
                if (ActiveItem.InsertingNodeViewModel == null)
                {
                    NodeSelectorViewModel.SelectedNode = null;
                }
            }
        }

        private void ReopenActiveDiagram()
        {
            var activeDiagram = ActiveItem;
            var indexOfActive = Items.IndexOf(ActiveItem);
            ActiveItem.RequestClose();
            Items.Insert(indexOfActive, activeDiagram);
            ActiveItem = activeDiagram;
        }

        public void CloseActiveDiagram()
        {
            if (ActiveItem != null)
            {
                ActiveItem.Diagram.IsOpen = false;
            }
        }

        public void RightMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            RightMouseDown(e.GetPosition((IInputElement)sender));
        }

        public void RightMouseDown(Point mousePoint)
        {
            if (ActiveItem == null)
            {
                NodeSelectorViewModel.SelectedNode = null;
                return;
            }
            if (NodeSelectorViewModel.SelectedNode == null)
            {
                NodeSelectorViewModel.RightPosition = mousePoint.X + 1;
                NodeSelectorViewModel.TopPosition = mousePoint.Y - 27;
                NodeSelectorVisible = true;
            }
            else NodeSelectorViewModel.SelectedNode = null;
        }

        public void LeftMouseDown()
        {
            NodeSelectorVisible = false;
        }
    }
}