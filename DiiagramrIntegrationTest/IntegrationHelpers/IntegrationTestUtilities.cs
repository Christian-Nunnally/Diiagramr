using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.Service.Interfaces;
using Diiagramr.View;
using Diiagramr.View.CustomControls;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Diiagramr.ViewModel.Diagram.CoreNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StyletIoC;

namespace DiiagramrIntegrationTest.IntegrationHelpers
{
    public static class IntegrationTestUtilities
    {
        public static ShellViewModel SetupShellViewModel()
        {
            IStyletIoCBuilder builder = new StyletIoCBuilder();
            builder.Bind<ShellViewModel>().ToSelf();
            builder.Bind<ProjectExplorerViewModel>().ToSelf();
            builder.Bind<DiagramWellViewModel>().ToSelf();
            builder.Bind<DiagramViewModel>().ToSelf();
            builder.Bind<NodeSelectorViewModel>().ToSelf();
            builder.Bind<IDirectoryService>().To<DirectoryService>();
            builder.Bind<IProjectLoadSave>().To<ProjectLoadSave>();
            builder.Bind<IProjectFileService>().To<ProjectFileService>().InSingletonScope();
            builder.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
            builder.Bind<IProvideNodes>().To<NodeProvider>().InSingletonScope();
            builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("open");
            builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("save");
            builder.Bind<PluginNode>().To<DiagramInputNodeViewModel>();
            builder.Bind<PluginNode>().To<DiagramOutputNodeViewModel>();
            builder.Bind<PluginNode>().To<TestNode>();
            var container = builder.BuildContainer();
            return container.Get<ShellViewModel>();
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
        public static PluginNode PlaceNode(this ShellViewModel shell, PluginNode node, int ptX = 0, int ptY = 0)
        {
            var nodeSelectorViewModel = shell.DiagramWellViewModel.NodeSelectorViewModel;
            var pt = new Point(ptX, ptY);
            var diagramViewModel = shell.DiagramWellViewModel.ActiveItem;
            Assert.IsNotNull(diagramViewModel, "must have diagram open");
            nodeSelectorViewModel.MousedOverNode = node;
            nodeSelectorViewModel.SelectNode();
            Assert.AreEqual(node.GetType(), diagramViewModel.InsertingNodeViewModel.GetType());
            diagramViewModel.MouseMoved(pt);
            Assert.AreEqual(ptX - DiagramConstants.NodeBorderWidth - diagramViewModel.InsertingNodeViewModel.Width / 2, diagramViewModel.InsertingNodeViewModel.X);
            Assert.AreEqual(ptY - DiagramConstants.NodeBorderWidth - diagramViewModel.InsertingNodeViewModel.Height / 2, diagramViewModel.InsertingNodeViewModel.Y);
            diagramViewModel.PreviewLeftMouseButtonDown(pt);
            diagramViewModel.LeftMouseButtonDown(pt);
            var placedNode = diagramViewModel.NodeViewModels.Last();
            Assert.AreEqual(ptX - DiagramConstants.NodeBorderWidth - placedNode.Width / 2, placedNode.X);
            Assert.AreEqual(ptY - DiagramConstants.NodeBorderWidth - placedNode.Height / 2, placedNode.Y);
            return placedNode;
        }

        public static WireViewModel WireTerminals(this ShellViewModel shell, TerminalViewModel sourceTerminal,
            TerminalViewModel sinkTerminal)
        {
            sourceTerminal.DropObject(sinkTerminal.TerminalModel);
            Assert.IsNotNull(sourceTerminal.TerminalModel.ConnectedWire);
            Assert.IsNotNull(sinkTerminal.TerminalModel.ConnectedWire);
            var wireViewModel = shell.DiagramWellViewModel.ActiveItem.WireViewModels.Last();
            Assert.AreEqual(wireViewModel.WireModel, sourceTerminal.TerminalModel.ConnectedWire);
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