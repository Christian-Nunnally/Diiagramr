using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram;
using Stylet;

namespace DiiagramrAPI.ViewModel
{
    public class DiagramWellViewModel : Conductor<DiagramViewModel>.Collection.OneActive
    {
        private readonly IProjectManager _projectManager;

        public DiagramWellViewModel(Func<IProjectManager> projectManagerFactory)
        {
            _projectManager = projectManagerFactory.Invoke();
            _projectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;
        }

        public ObservableCollection<DiagramModel> CurrentDiagrams { get; private set; }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (_projectManager.CurrentProject != null && _projectManager.CurrentDiagrams != null)
                SetCurrentDiagrams(_projectManager.CurrentDiagrams);
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
                diagram.PropertyChanged += DiagramOnPropertyChanged;

            CurrentDiagrams.CollectionChanged += CurrentDiagramsOnCollectionChanged;
        }

        private void CurrentDiagramsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var oldDiagram in e.OldItems.OfType<DiagramModel>())
                {
                    oldDiagram.IsOpen = false;
                    oldDiagram.PropertyChanged -= DiagramOnPropertyChanged;
                }

            if (e.NewItems != null)
                foreach (var newDiagram in e.NewItems.OfType<DiagramModel>())
                    newDiagram.PropertyChanged += DiagramOnPropertyChanged;
        }

        private void DiagramOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var diagram = (DiagramModel) sender;
            if (e.PropertyName.Equals(nameof(diagram.IsOpen)))
                if (diagram.IsOpen)
                    OpenDiagram(diagram);
                else
                    CloseDiagram(diagram);
        }

        private void CloseDiagram(DiagramModel diagram)
        {
            var diagramViewModel = Items.FirstOrDefault(viewModel => viewModel.Diagram == diagram);
            if (diagramViewModel != null)
                CloseItem(diagramViewModel);
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
            var diagramViewModelSender = (DiagramViewModel) sender;
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
                ActiveItem.Diagram.IsOpen = false;
        }
    }
}