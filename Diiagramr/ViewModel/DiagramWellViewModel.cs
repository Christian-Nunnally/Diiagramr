using Stylet;
using System;
using System.ComponentModel;
using System.Linq;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;

namespace Diiagramr.ViewModel
{
    public class DiagramWellViewModel : Conductor<DiagramViewModel>.Collection.OneActive
    {

        public NodeSelectorViewModel NodeSelectorViewModel { get; set; }

        public bool NodeSelectorVisible { get; set; }

        public DiagramWellViewModel(Func<NodeSelectorViewModel> nodeSelectorViewModelFactory)
        {
            NodeSelectorViewModel = nodeSelectorViewModelFactory.Invoke();
            NodeSelectorViewModel.PropertyChanged += NodeSelectorPropertyChanged;
        }

        private void NodeSelectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedNode") return;
            ActiveItem.InsertingNodeViewModel = NodeSelectorViewModel.CreateNewInstanceOfSelectedNode();
            if (NodeSelectorViewModel.SelectedNode != null) NodeSelectorVisible = false;
        }

        public virtual void OpenDiagram(EDiagram diagram)
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

        public void CloseAllOpenDiagrams()
        {
            for (var i = Items.Count - 1; i >= 0; i--)
            {
                Items[i].RequestClose();
            }
        }
    }
}