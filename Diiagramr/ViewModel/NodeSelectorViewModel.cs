﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.View;
using Diiagramr.ViewModel.Diagram;
using PropertyChanged;
using Stylet;

namespace Diiagramr.ViewModel
{
    public class NodeSelectorViewModel : Screen
    {

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            var nodeProvidor = nodeProvider.Invoke();

            foreach (var nodeViewModel in nodeProvidor.GetRegisteredNodes())
            {
                var fullTypeName = nodeViewModel.GetType().FullName;
                var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
                var library = GetOrCreateLibrary(libraryName);
                library.Nodes.Add(nodeViewModel);

                if (nodeViewModel is PluginNode pluginNode)
                {
                    pluginNode.NodeModel = new NodeModel("");
                    pluginNode.SetupNode(new NodeSetup(pluginNode));
                }
            }
        }

        public virtual AbstractNodeViewModel SelectedNode { get; set; }

        public IEnumerable<AbstractNodeViewModel> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
        public BindableCollection<AbstractNodeViewModel> VisibleNodesList { get; set; } = new BindableCollection<AbstractNodeViewModel>();
        public BindableCollection<Library> LibrariesList { get; set; } = new BindableCollection<Library>();

        public AbstractNodeViewModel MousedOverNode { get; set; }

        public bool NodePreviewVisible => MousedOverNode != null;

        public double TopPosition { get; set; }
        public double RightPosition { get; set; }

        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }
        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }
        public event Action ShouldClose;

        private Library GetOrCreateLibrary(string libraryName)
        {
            if (LibrariesList.All(l => l.Name != libraryName)) LibrariesList.Add(new Library(libraryName));
            return LibrariesList.First(l => l.Name == libraryName);
        }

        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
            ShouldClose?.Invoke();
        }

        public void SelectNode()
        {
            SelectedNode = MousedOverNode;
            ShouldClose?.Invoke();
        }

        public void LibraryMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border) sender).DataContext is Library library)) return;
            ShowLibrary(library);
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border) sender).DataContext is AbstractNodeViewModel node)) return;
            PreviewNode(node);
        }

        public void ShowLibrary(Library library)
        {
            VisibleNodesList.Clear();
            VisibleNodesList.AddRange(library.Nodes);
            LibrariesList.ForEach(l => l.Unselect());
            library.Select();
            MousedOverNode = null;
        }

        private void PreviewNode(AbstractNodeViewModel node)
        {
            const int workingWidth = 100;
            const int workingHeight = 100;

            MousedOverNode = VisibleNodesList.First(m => m.Name == node.Name);
            var totalNodeWidth = MousedOverNode.Width + DiagramConstants.NodeBorderWidth * 2;
            var totalNodeHeight = MousedOverNode.Height + DiagramConstants.NodeBorderWidth * 2;
            PreviewNodeScaleX = workingWidth / totalNodeWidth;
            PreviewNodeScaleY = workingHeight / totalNodeHeight;

            PreviewNodeScaleX = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);
            PreviewNodeScaleY = Math.Min(PreviewNodeScaleX, PreviewNodeScaleY);

            var newWidth = totalNodeWidth * PreviewNodeScaleX;
            var newHeight = totalNodeHeight * PreviewNodeScaleY;

            PreviewNodePositionX = (workingWidth - newWidth) / 2.0;
            PreviewNodePositionY = (workingHeight - newHeight) / 2.0;
        }

        public void MouseLeftSelector()
        {
            LibrariesList.ForEach(l => l.Unselect());
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Library
    {
        public Library(string name)
        {
            Name = name;
            Nodes = new List<AbstractNodeViewModel>();
        }

        public virtual List<AbstractNodeViewModel> Nodes { get; private set; }
        public string Name { get; private set; }
        public Brush BackgroundBrush { get; private set; }

        public virtual void Select()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        }

        public virtual void Unselect()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
    }
}