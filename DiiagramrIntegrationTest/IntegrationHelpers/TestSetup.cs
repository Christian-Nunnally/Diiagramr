using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Diiagramr.Service;
using Diiagramr.View;
using Diiagramr.View.CustomControls;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public static class TestSetup
    {
        public static ShellViewModel SetupShellViewModel()
        {
            var dirService = new DirectoryService();
            var testFileDirectory = new Mock<IFileDialog>();
            testFileDirectory.Setup(m => m.ShowDialog()).Returns(DialogResult.OK);
            testFileDirectory.Setup(m => m.FileName).Returns("testProj");
            testFileDirectory.Setup(m => m.InitialDirectory).Returns("c://");
            var projFileSystem = new ProjectFileService(dirService, testFileDirectory.Object, testFileDirectory.Object, new TestLoadSave());
            ProjectFileService ProjectFileServiceFactory() => projFileSystem;
            var testNode = new TestNode();
            var testNodeList = new List<AbstractNodeViewModel> {testNode};
            IEnumerable<AbstractNodeViewModel> NodeFactory() => testNodeList;
            var nodeProvider = new NodeProvider(NodeFactory);
            NodeProvider NodeProviderFactory() => nodeProvider;
            var projManager = new ProjectManager(ProjectFileServiceFactory, NodeProviderFactory);
            ProjectManager ProjectManagerFactory() => projManager;
            var projExplorer = new ProjectExplorerViewModel(ProjectManagerFactory);
            ProjectExplorerViewModel ProjectExplorerViewModelFactory() => projExplorer;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProviderFactory);
            NodeSelectorViewModel NodeSelectorViewModelFactory() => nodeSelectorViewModel;
            var diagramWellViewModel = new DiagramWellViewModel(ProjectManagerFactory, NodeProviderFactory, NodeSelectorViewModelFactory);
            DiagramWellViewModel DiagramWellViewModelFactory() => diagramWellViewModel;
            var shell = new ShellViewModel(ProjectExplorerViewModelFactory, DiagramWellViewModelFactory, ProjectManagerFactory);
            return shell;
        }

        // Opens diagram at index, default to first
        public static DiagramViewModel OpenDiagram(this ShellViewModel shell, int index = 0)
        {
            var projExplorer = shell.ProjectExplorerViewModel;
            var projManager = projExplorer.ProjectManager;
            var diagram = projManager.CurrentDiagrams[index];
            projExplorer.SelectedDiagram = diagram;
            projExplorer.DiagramProjectItemMouseDown(2);
            var diagramViewModel = projManager.DiagramViewModels.First(d => d.Diagram == diagram);
            Assert.AreEqual(diagramViewModel, shell.DiagramWellViewModel.ActiveItem);
            return diagramViewModel;
        }


        // Places the given node at (ptX, ptY) on active diagram and returns new node
        public static AbstractNodeViewModel PlaceNode(this ShellViewModel shell, AbstractNodeViewModel node, int ptX = 0, int ptY = 0)
        {
            var nodeSelectorViewModel = shell.DiagramWellViewModel.NodeSelectorViewModel;
            var pt = new Point(ptX, ptY);
            var diagramViewModel = shell.DiagramWellViewModel.ActiveItem;
            // must have diagram open
            Assert.IsNotNull(diagramViewModel);
            nodeSelectorViewModel.MousedOverNode = node;
            nodeSelectorViewModel.SelectNode();
            Assert.AreEqual(node.GetType(), diagramViewModel.InsertingNodeViewModel.GetType());
            diagramViewModel.MouseMoved(pt);
            Assert.AreEqual(ptX - DiagramConstants.NodeBorderWidth, diagramViewModel.InsertingNodeViewModel.X);
            Assert.AreEqual(ptY - DiagramConstants.NodeBorderWidth, diagramViewModel.InsertingNodeViewModel.Y);
            diagramViewModel.PreviewLeftMouseButtonDown(pt);
            diagramViewModel.LeftMouseButtonDown(pt);
            var placedNode = diagramViewModel.NodeViewModels.Last();
            Assert.AreEqual(ptX - DiagramConstants.NodeBorderWidth, placedNode.X);
            Assert.AreEqual(ptY - DiagramConstants.NodeBorderWidth, placedNode.Y);
            return placedNode;
        }

        public static WireViewModel WireTerminals(this ShellViewModel shell, TerminalViewModel sourceTerminal,
            TerminalViewModel sinkTerminal)
        {
            sourceTerminal.DropObject(sinkTerminal.Terminal);
            Assert.IsNotNull(sourceTerminal.Terminal.ConnectedWire);
            Assert.IsNotNull(sinkTerminal.Terminal.ConnectedWire);
            var wireViewModel = shell.DiagramWellViewModel.ActiveItem.WireViewModels.Last();
            Assert.AreEqual(wireViewModel.WireModel, sourceTerminal.Terminal.ConnectedWire);
            return wireViewModel;
        }
    }
}