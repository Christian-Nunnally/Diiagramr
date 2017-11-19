using System.Linq;
using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ShellScreen;
using DiiagramrAPI.ViewModel.ShellScreen.ProjectScreen;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;
using DiiagramrAPI.Model;

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
            builder.Bind<ProjectScreenViewModel>().ToSelf();
            builder.Bind<LibraryManagerScreenViewModel>().ToSelf();
            builder.Bind<StartScreenViewModel>().ToSelf();
            builder.Bind<IDirectoryService>().To<DirectoryService>();
            builder.Bind<IProjectLoadSave>().To<ProjectLoadSave>();
            builder.Bind<IProjectFileService>().To<ProjectFileService>().InSingletonScope();
            builder.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
            builder.Bind<IPluginLoader>().To<PluginLoader>().InSingletonScope();
            builder.Bind<IProvideNodes>().To<NodeProvider>().InSingletonScope();
            builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("open");
            builder.Bind<IFileDialog>().To<TestFileDialog>().WithKey("save");
            var container = builder.BuildContainer();

            var nodeProvider = container.Get<IProvideNodes>();
            nodeProvider.RegisterNode(new TestNode(), new DependencyModel("",""));
            nodeProvider.RegisterNode(new DiagramInputNodeViewModel(), new DependencyModel("", ""));
            nodeProvider.RegisterNode(new DiagramOutputNodeViewModel(), new DependencyModel("", ""));

            var shell = container.Get<ShellViewModel>();
            var projectScreen = shell.ProjectScreenViewModel;
            var nodeSelectorViewModel = projectScreen.DiagramWellViewModel.NodeSelectorViewModel;
            var projectManager = projectScreen.ProjectExplorerViewModel.ProjectManager;

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