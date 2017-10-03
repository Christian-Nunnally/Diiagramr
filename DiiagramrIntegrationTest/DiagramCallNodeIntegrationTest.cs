using System.Linq;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.Service.Interfaces;
using Diiagramr.View.CustomControls;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Diiagramr.ViewModel.Diagram.CoreNode;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;

namespace DiiagramrIntegrationTest
{
    [TestClass]
    public class DiagramCallNodeIntegrationTest
    {
        [TestMethod]
        public void TestSimpleWiredInputOutput()
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

            var shell = container.Get<ShellViewModel>();
            var nodeSelectorViewModel = shell.DiagramWellViewModel.NodeSelectorViewModel;
            var projectManager = shell.ProjectExplorerViewModel.ProjectManager;

            var inputNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(DiagramInputNodeViewModel));
            var outputNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(DiagramOutputNodeViewModel));
            var testNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(TestNode));

            shell.CreateProject();
            projectManager.CreateDiagram();
            projectManager.CreateDiagram();
            var diagramViewModel2 = shell.OpenDiagram(1);
            var diagramViewModel1 = shell.OpenDiagram(0);

            var diagramCallNode = diagramViewModel2.PlaceDiagramCallNodeFor(diagramViewModel1.Diagram);
            Assert.AreEqual(0, diagramCallNode.TerminalViewModels.Count);

            var inputNodeOnDiagram1 = shell.PlaceNode(inputNode, 0, 0);
            Assert.AreEqual(1, diagramCallNode.InputTerminalViewModels.Count());

            var outputNodeOnDiagram1 = shell.PlaceNode(outputNode, 0, 0);
            Assert.AreEqual(1, diagramCallNode.OutputTerminalViewModels.Count());

            shell.WireTerminals(inputNodeOnDiagram1.TerminalViewModels.First(), outputNodeOnDiagram1.TerminalViewModels.First());

            shell.OpenDiagram(1);

            var testNodeInput = shell.PlaceNode(testNode, 0, 0);
            var testNodeOutput = shell.PlaceNode(testNode, 0, 0);

            shell.WireTerminals(testNodeInput.OutputTerminalViewModels.First(), diagramCallNode.InputTerminalViewModels.First());
            shell.WireTerminals(diagramCallNode.OutputTerminalViewModels.First(), testNodeOutput.InputTerminalViewModels.First());

            testNodeInput.OutputTerminalViewModels.First().Data = 5;
            Assert.AreEqual(5, testNodeOutput.InputTerminalViewModels.First().Data);

            inputNodeOnDiagram1.DisconnectAllTerminals();

            testNodeInput.OutputTerminalViewModels.First().Data = 6;
            Assert.IsNull(testNodeOutput.InputTerminalViewModels.First().Data);
        }
    }
}