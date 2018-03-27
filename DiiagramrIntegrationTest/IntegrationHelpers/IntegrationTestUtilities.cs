using System.Linq;
using System.Windows;
using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ProjectScreen;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public static class IntegrationTestUtilities
    {
        private static IContainer _container;

        public static IContainer Container
        {
            get
            {
                if (_container != null) return _container;
                IStyletIoCBuilder builder = new StyletIoCBuilder();
                builder.Bind<ShellViewModel>().ToSelf();
                builder.Bind<ProjectExplorerViewModel>().ToSelf();
                builder.Bind<DiagramWellViewModel>().ToSelf();
                builder.Bind<DiagramViewModel>().ToSelf();
                builder.Bind<NodeSelectorViewModel>().ToSelf();
                builder.Bind<ProjectScreenViewModel>().ToSelf();
                builder.Bind<LibraryManagerWindowViewModel>().ToSelf();
                builder.Bind<StartScreenViewModel>().ToSelf();
                builder.Bind<IDirectoryService>().To<DirectoryService>();
                builder.Bind<IProjectLoadSave>().To<ProjectLoadSave>();
                builder.Bind<ILibraryManager>().To<LibraryManager>();
                builder.Bind<IFetchWebResource>().To<WebResourceFetcher>();
                builder.Bind<LibrarySourceManagerWindowViewModel>().ToSelf();
                builder.Bind<IProjectFileService>().To<ProjectFileService>().InSingletonScope();
                builder.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
                builder.Bind<IProvideNodes>().To<NodeProvider>().InSingletonScope();
                builder.Bind<IPluginLoader>().To<PluginLoader>().InSingletonScope();
                builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("open");
                builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("save");
                builder.Bind<DiagramViewModelFactory>().ToSelf();
                builder.Bind<ColorTheme>().ToSelf();
                _container = builder.BuildContainer();
                return _container;
            }
        }

        public static ShellViewModel SetupShellViewModel()
        {
            var nodeProvider = Container.Get<IProvideNodes>();
            nodeProvider.RegisterNode(new TestPassthroughNode(), new NodeLibrary());
            nodeProvider.RegisterNode(new TestIntNode(), new NodeLibrary());
            nodeProvider.RegisterNode(new DiagramInputNodeViewModel(), new NodeLibrary());
            nodeProvider.RegisterNode(new DiagramOutputNodeViewModel(), new NodeLibrary());

            return Container.Get<ShellViewModel>();
        }

        // Opens diagram at index, default to first
        public static DiagramViewModel OpenDiagram(this ShellViewModel shell, int index = 0)
        {
            var projectScreen = shell.ProjectScreenViewModel;
            var projectExplorer = projectScreen.ProjectExplorerViewModel;
            var projectManager = projectExplorer.ProjectManager;
            var diagram = projectManager.CurrentDiagrams[index];
            projectExplorer.SelectedDiagram = diagram;
            projectExplorer.DiagramProjectItemMouseUp();
            var diagramViewModel = projectManager.DiagramViewModels.First(d => d.Diagram == diagram);
            Assert.AreEqual(diagramViewModel, projectScreen.DiagramWellViewModel.ActiveItem);
            return diagramViewModel;
        }


        // Places the given node at (ptX, ptY) on active diagram and returns new node
        public static PluginNode PlaceNode(this ShellViewModel shell, PluginNode node, int ptX = 0, int ptY = 0)
        {
            var projectScreen = shell.ProjectScreenViewModel;
            var diagramWell = projectScreen.DiagramWellViewModel;
            var diagramViewModel = diagramWell.ActiveItem;
            var nodeSelector = diagramViewModel.NodeSelectorViewModel;
            var pt = new Point(ptX, ptY);
            Assert.IsNotNull(diagramViewModel, "must have diagram open");
            nodeSelector.MousedOverNode = node;
            nodeSelector.SelectNode();
            Assert.AreEqual(node.GetType(), diagramViewModel.InsertingNodeViewModel.GetType());
            diagramViewModel.MouseMoved(pt);
            Assert.AreEqual(ptX - DiagramViewModel.NodeBorderWidth - diagramViewModel.InsertingNodeViewModel.Width / 2, diagramViewModel.InsertingNodeViewModel.X);
            Assert.AreEqual(ptY - DiagramViewModel.NodeBorderWidth - diagramViewModel.InsertingNodeViewModel.Height / 2, diagramViewModel.InsertingNodeViewModel.Y);
            diagramViewModel.PreviewLeftMouseButtonDown(pt);
            diagramViewModel.LeftMouseButtonDown(pt);
            var placedNode = diagramViewModel.NodeViewModels.Last();
            Assert.AreEqual(ptX - DiagramViewModel.NodeBorderWidth - placedNode.Width / 2, placedNode.X);
            Assert.AreEqual(ptY - DiagramViewModel.NodeBorderWidth - placedNode.Height / 2, placedNode.Y);
            return placedNode;
        }

        public static WireViewModel WireTerminals(this ShellViewModel shell, TerminalViewModel sourceTerminal,
            TerminalViewModel sinkTerminal)
        {
            var projectScreen = shell.ProjectScreenViewModel;
            sourceTerminal.DropObject(sinkTerminal.TerminalModel);
            Assert.AreNotEqual(0, sourceTerminal.TerminalModel.ConnectedWires.Count);
            Assert.AreNotEqual(0, sinkTerminal.TerminalModel.ConnectedWires.Count);
            var wireViewModel = projectScreen.DiagramWellViewModel.ActiveItem.WireViewModels.Last();
            Assert.IsTrue(sourceTerminal.TerminalModel.ConnectedWires.Contains(wireViewModel.WireModel));
            return wireViewModel;
        }

        public static DiagramCallNodeViewModel PlaceDiagramCallNodeFor(this DiagramViewModel diagramViewModel, DiagramModel diagram)
        {
            diagramViewModel.DiagramDragEnter(diagram);
            diagramViewModel.DroppedDiagramCallNode(null, null);
            diagramViewModel.LeftMouseButtonDown(new Point(0, 0));
            return diagramViewModel.NodeViewModels.Last() as DiagramCallNodeViewModel;
        }
    }
}