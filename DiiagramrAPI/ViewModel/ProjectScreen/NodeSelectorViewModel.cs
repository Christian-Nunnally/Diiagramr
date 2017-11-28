using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using PropertyChanged;
using Stylet;

namespace DiiagramrAPI.ViewModel
{
    public class NodeSelectorViewModel : Screen
    {
        private readonly IProvideNodes _nodeProvider;

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
            AddNodes();
        }

        public virtual PluginNode SelectedNode { get; set; }

        public IEnumerable<PluginNode> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
        public BindableCollection<PluginNode> VisibleNodesList { get; set; } = new BindableCollection<PluginNode>();
        public BindableCollection<Library> LibrariesList { get; set; } = new BindableCollection<Library>();

        public PluginNode MousedOverNode { get; set; }

        public bool Visible { get; set; }

        public bool NodePreviewVisible => MousedOverNode != null;

        public double TopPosition { get; set; }
        public double RightPosition { get; set; }

        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }
        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }

        public void AddNodes()
        {
            foreach (var nodeViewModel in _nodeProvider.GetRegisteredNodes())
            {
                if (nodeViewModel is DiagramCallNodeViewModel) continue;
                var fullTypeName = nodeViewModel.GetType().FullName;
                var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
                var library = GetOrCreateLibrary(libraryName);
                if (library.Nodes.Any(n => n.Equals(nodeViewModel))) continue;
                library.Nodes.Add(nodeViewModel);

                nodeViewModel.NodeModel = new NodeModel("");
                nodeViewModel.SetupNode(new NodeSetup(nodeViewModel));
            }
        }

        private Library GetOrCreateLibrary(string libraryName)
        {
            if (LibrariesList.All(l => l.Name != libraryName)) LibrariesList.Insert(0, new Library(libraryName));
            return LibrariesList.First(l => l.Name == libraryName);
        }

        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
            Visible = false;
        }

        public void SelectNode()
        {
            SelectedNode = MousedOverNode;
            Visible = false;
        }

        public void LibraryMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border) sender).DataContext is Library library)) return;
            ShowLibrary(library);
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border) sender).DataContext is PluginNode node)) return;
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

        private void PreviewNode(PluginNode node)
        {
            const int workingWidth = 100;
            const int workingHeight = 100;

            MousedOverNode = VisibleNodesList.First(m => m.Name == node.Name);
            var totalNodeWidth = MousedOverNode.Width + DiagramViewModel.NodeBorderWidth * 2;
            var totalNodeHeight = MousedOverNode.Height + DiagramViewModel.NodeBorderWidth * 2;
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

        private void NodesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddNodes();
        }
    }
    
    [AddINotifyPropertyChangedInterface]
    public class Library
    {
        public Library(string name)
        {
            Name = name;
            Nodes = new List<PluginNode>();
        }

        public virtual List<PluginNode> Nodes { get; }
        public string Name { get; }
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