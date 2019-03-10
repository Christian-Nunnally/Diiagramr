using DiiagramrAPI.Diagram.CoreNode;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
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

namespace DiiagramrAPI.Diagram.Interacters
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

    public class NodeSelectorViewModel : DiagramInteractor
    {
        private const double NodeSelectorBottomMargin = 250;
        private const double NodeSelectorRightMargin = 400;
        private IProvideNodes _nodeProvider;
        private DiagramViewModel _diagramViewModel;

        private bool _visible;

        private bool nodesAdded = false;

        public NodeSelectorViewModel(Func<IProvideNodes> nodeProvider)
        {
            _nodeProvider = nodeProvider.Invoke();
            _nodeProvider.PropertyChanged += NodesOnPropertyChanged;
            AddNodes();
        }

        public IEnumerable<PluginNode> AvailableNodeViewModels => LibrariesList.SelectMany(l => l.Nodes);
        public List<Library> LibrariesList { get; set; } = new List<Library>();
        public BindableCollection<Library> VisibleLibrariesList { get; set; } = new BindableCollection<Library>();
        public PluginNode MousedOverNode { get; set; }
        public bool NodePreviewVisible => MousedOverNode != null;
        public double PreviewNodePositionX { get; set; }
        public double PreviewNodePositionY { get; set; }
        public double PreviewNodeScaleX { get; set; }
        public double PreviewNodeScaleY { get; set; }

        public BindableCollection<PluginNode> VisibleNodesList { get; set; } = new BindableCollection<PluginNode>();

        private Func<PluginNode, bool> Filter = x => true;

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
            _diagramViewModel.StopInteractor(this);
            _diagramViewModel.BeginInsertingNode(MousedOverNode, true);
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

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown;
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            _diagramViewModel = interaction.Diagram;

            var availableWidth = _diagramViewModel.View != null ? _diagramViewModel.View.RenderSize.Width : 0;
            var availableHeight = _diagramViewModel.View != null ? _diagramViewModel.View.RenderSize.Height : 0;
            X = interaction.MousePosition.X < availableWidth - NodeSelectorRightMargin ? interaction.MousePosition.X : availableWidth - NodeSelectorRightMargin;
            Y = interaction.MousePosition.Y < availableHeight - NodeSelectorBottomMargin ? interaction.MousePosition.Y : availableHeight - NodeSelectorBottomMargin;

            var mousedOverViewModel = interaction.ViewModelMouseIsOver;
            ShowWithContextFilter(mousedOverViewModel);
        }

        private void ShowWithContextFilter(Screen mousedOverViewModel)
        {
            if (mousedOverViewModel is InputTerminalViewModel inputTerminalMouseIsOver)
            {
                Show(n => n.TerminalViewModels.Any(t => t is OutputTerminalViewModel && t.TerminalModel.Type.IsAssignableFrom(inputTerminalMouseIsOver.TerminalModel.Type)));
            }
            else if (mousedOverViewModel is OutputTerminalViewModel outputTerminalMouseIsOver)
            {
                Show(n => n.TerminalViewModels.Any(t => t is InputTerminalViewModel && t.TerminalModel.Type.IsAssignableFrom(outputTerminalMouseIsOver.TerminalModel.Type)));
            }
            else
            {
                Show(n => true);
            }
        }

        private void Show(Func<PluginNode, bool> filter)
        {
            Filter = filter;
            VisibleLibrariesList.Clear();
            VisibleLibrariesList.AddRange(LibrariesList.Where(l => l.Nodes.Where(filter).Any()));
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
        }
    }
}
