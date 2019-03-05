using DiiagramrAPI.Diagram.Interacters;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using PropertyChanged;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Library
    {
        public Library(string name)
        {
            Name = name;
            Nodes = new List<PluginNode>();
        }

        public bool IsLibraryMenuExpanded { get; set; }
        public Brush BackgroundBrush { get; private set; } = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public string Name { get; }
        public virtual List<PluginNode> Nodes { get; }
        public bool NodesLoaded { get; set; }

        public virtual void Select()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            IsLibraryMenuExpanded = true;
        }

        public virtual void Unselect()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            IsLibraryMenuExpanded = false;
        }
    }

    public class NodeSelectorViewModel : DiagramInteracter
    {
        private IProvideNodes _nodeProvider;

        private bool _visible;

        private bool nodesAdded = false;

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
        }

        public event Action<PluginNode> NodeSelected;

        public IEnumerable<PluginNode> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
        public List<Library> LibrariesList { get; set; } = new List<Library>();
        public BindableCollection<Library> VisibleLibrariesList { get; set; } = new BindableCollection<Library>();
        public PluginNode MousedOverNode { get; set; }
        public bool NodePreviewVisible => MousedOverNode != null;
        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }
        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }

        public override bool Visible
        {
            get => _visible;

            set
            {
                _visible = value;
                if (_visible)
                {
                    AddNodes();
                }
                else
                {
                    Filter = x => true;
                }
            }
        }

        public BindableCollection<PluginNode> VisibleNodesList { get; set; } = new BindableCollection<PluginNode>();

        private Func<PluginNode, bool> Filter = x => true;

        public void Show(Func<PluginNode, bool> filter)
        {
            Visible = true;
            Filter = filter;
            VisibleLibrariesList.Clear();
            VisibleLibrariesList.AddRange(LibrariesList.Where(l => l.Nodes.Where(filter).Any()));
        }

        public void AddNodes()
        {
            if (nodesAdded)
            {
                return;
            }

            nodesAdded = true;
            foreach (var nodeViewModel in _nodeProvider.GetRegisteredNodes())
            {
                if (nodeViewModel is DiagramCallNodeViewModel)
                {
                    continue;
                }

                if (IsHiddenFromSelector(nodeViewModel))
                {
                    continue;
                }

                var fullTypeName = nodeViewModel.GetType().FullName;
                var libraryName = fullTypeName?.Split('.').FirstOrDefault() ?? fullTypeName;
                var library = GetOrCreateLibrary(libraryName);
                if (library.Nodes.Any(n => n.Equals(nodeViewModel)))
                {
                    continue;
                }

                var nodeModel = new NodeModel("");
                try
                {
                    nodeViewModel.InitializeWithNode(nodeModel);
                    library.Nodes.Add(nodeViewModel);
                }
                catch (FileNotFoundException e)
                {
                    Console.Error.WriteLine($"Error in '{fullTypeName}.InitializeWithNode(NodeModel node)' --- Exception message: {e.Message}");
                }
            }
        }

        public void BackgroundMouseDown()
        {
            VisibleNodesList.Clear();
            MousedOverNode = null;
            Visible = false;
        }

        public void LibraryMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is Library library))
            {
                return;
            }

            if (!library.NodesLoaded)
            {
                library.NodesLoaded = true;
            }
            ShowLibrary(library);
        }

        public void MouseLeftSelector()
        {
            LibrariesList.ForEach(l => l.Unselect());
            VisibleNodesList.Clear();
            MousedOverNode = null;
        }

        public void NodeMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (!(((Border)sender).DataContext is PluginNode node))
            {
                return;
            }

            PreviewNode(node);
        }

        public void SelectNode()
        {
            NodeSelected?.Invoke(MousedOverNode);
            Visible = false;
        }

        public void ShowLibrary(Library library)
        {
            VisibleNodesList.Clear();
            VisibleNodesList.AddRange(library.Nodes.Where(Filter).OrderBy(n => n.Weight));
            VisibleLibrariesList.ForEach(l => l.Unselect());
            library.Select();
            MousedOverNode = null;
        }

        private Library GetOrCreateLibrary(string libraryName)
        {
            if (LibrariesList.All(l => l.Name != libraryName))
            {
                LibrariesList.Insert(0, new Library(libraryName));
            }

            return LibrariesList.First(l => l.Name == libraryName);
        }

        private bool IsHiddenFromSelector(PluginNode nodeViewModel)
        {
            return nodeViewModel.GetType().IsDefined(typeof(HideFromNodeSelector), false);
        }

        private void NodesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddNodes();
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

        public override bool ShouldInteractionStart(InteractionEventArguments interaction)
        {
            return false;
        }

        public override bool ShouldInteractionStop(InteractionEventArguments interaction)
        {
            return false;
        }
    }
}
