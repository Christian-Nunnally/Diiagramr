using System.Linq;
using DiiagramrAPI.ViewModel;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;

namespace DiiagramrIntegrationTest
{
    [TestClass]
    public class ExecutionTest
    {
        private ShellViewModel _shell;
        private IContainer _container;

        [TestInitialize]
        public void TestInitialize()
        {
            _shell = IntegrationTestUtilities.SetupShellViewModel();
            _container = IntegrationTestUtilities.Container;
        }

        [TestMethod]
        public void RunPauseStop()
        {
            var projectScreen = _shell.ProjectScreenViewModel;
            var projectExplorer = projectScreen.ProjectExplorerViewModel;
            var projectManager = projectExplorer.ProjectManager;
            var diagramWell = projectScreen.DiagramWellViewModel;
            var nodeSelector = (NodeSelectorViewModel)_container.Get(typeof(NodeSelectorViewModel));
            nodeSelector.Visible = true;
            var testNode = nodeSelector.AvailableNodeViewModels.OfType<TestPassthroughNode>().First();

            _shell.CreateProject();
            projectManager.CreateDiagram();
            var diagramViewModel = _shell.OpenDiagram();
            var node1 = _shell.PlaceNode(testNode);
            var node2 = _shell.PlaceNode(testNode);

            var outTerm = node1.OutputTerminalViewModels.First();
            var inTerm = node2.InputTerminalViewModels.First();
            _shell.WireTerminals(outTerm, inTerm);

            var controlViewModel = diagramWell.ActiveItem.DiagramControlViewModel;

            // play 
            outTerm.Data = 5;
            Assert.AreEqual(outTerm.Data, inTerm.Data);
            Assert.AreEqual((int)inTerm.Data + 1, node2.OutputTerminalViewModels.First().Data);

            // pause
            controlViewModel.Pause();
            outTerm.Data = 6;
            Assert.AreEqual(5, inTerm.Data);
            Assert.AreEqual(6, node2.OutputTerminalViewModels.First().Data);

            // play after pause
            controlViewModel.Play();
            Assert.AreEqual(6, inTerm.Data);
            Assert.AreEqual(7, node2.OutputTerminalViewModels.First().Data);

            // stop
            controlViewModel.Stop();
            Assert.AreEqual(6, outTerm.Data);
            Assert.IsNull(inTerm.Data);

            // play after stop
            controlViewModel.Play();
            Assert.AreEqual(6, inTerm.Data);
            Assert.AreEqual(7, node2.OutputTerminalViewModels.First().Data);
        }
    }
}