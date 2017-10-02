using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.Service.Interfaces;
using Diiagramr.View;
using Diiagramr.ViewModel.Diagram;
using Diiagramr.ViewModel.Diagram.CoreNode;
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
                if (nodeViewModel is DiagramCallNodeViewModel) continue;
                var fullTypeName = nodeViewModel.GetType().FullName;
                var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
                var library = GetOrCreateLibrary(libraryName);
                library.Nodes.Add(nodeViewModel);

                nodeViewModel.NodeModel = new NodeModel("");
                nodeViewModel.SetupNode(new NodeSetup(nodeViewModel));
            }
        }

        public virtual PluginNode SelectedNode { get; set; }

        public IEnumerable<PluginNode> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
        public BindableCollection<PluginNode> VisibleNodesList { get; set; } = new BindableCollection<PluginNode>();
        public BindableCollection<Library> LibrariesList { get; set; } = new BindableCollection<Library>();

        public PluginNode MousedOverNode { get; set; }

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