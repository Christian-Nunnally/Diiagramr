﻿using Diiagramr.Service;
using Diiagramr.Model;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Diiagramr.ViewModel
{
    public class ProjectExplorerViewModel : Screen
    {
        public IProjectManager ProjectManager { get; set; }

        public bool IsAddDiagramButtonVisible { get; set; }

        public Project Project { get; set; }

        public ObservableCollection<EDiagram> Diagrams { get; set; }

        public ProjectExplorerViewModel(Func<IProjectManager> projectManagerFactory)
        {
            ProjectManager = projectManagerFactory.Invoke();
            ProjectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            IsAddDiagramButtonVisible = ProjectManager?.CurrentProject != null;
            Project = ProjectManager?.CurrentProject;
            Diagrams = ProjectManager?.CurrentDiagrams;
        }

        public void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedDiagram != null)
                {
                    var dataObjectForDiagram = new DataObject(DataFormats.StringFormat, SelectedDiagram);
                    DragDrop.DoDragDrop((UIElement)sender, dataObjectForDiagram, DragDropEffects.Copy);
                }
            }
        }

        public EDiagram SelectedDiagram { get; set; }

        public void CreateDiagram()
        {
            ProjectManager.CreateDiagram();
        }

        public bool CanDeleteDiagram => SelectedDiagram != null;

        public void DeleteDiagram()
        {
            if (SelectedDiagram == null) return;
            var selectedDiagram = ProjectManager.CurrentDiagrams.First(x => x == SelectedDiagram);
            ProjectManager.DeleteDiagram(selectedDiagram);
        }
    }
}