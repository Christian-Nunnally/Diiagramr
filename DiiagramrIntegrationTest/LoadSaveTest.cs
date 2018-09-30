using System.Linq;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;

namespace DiiagramrIntegrationTest
{
    [TestClass]
    public class LoadSaveTest
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
        public void LoadSaveSimpleDiagramDragNodeTest()
        {
            var projectScreen = _shell.ProjectScreenViewModel;
            var projectExplorer = projectScreen.ProjectExplorerViewModel;
            var projectManager = projectExplorer.ProjectManager;
            var diagramWell = projectScreen.DiagramWellViewModel;
            var nodeSelector = (NodeSelectorViewModel)_container.Get(typeof(NodeSelectorViewModel));
            nodeSelector.Visible = true;
            var testNode = nodeSelector.AvailableNodeViewModels.OfType<TestPassthroughNode>().First();
            _shell.CreateProject();
            projectManager.CurrentProject.IsDirty = false;
            Assert.IsNotNull(projectManager.CurrentProject);

            projectManager.CreateDiagram();
            Assert.AreEqual(1, projectManager.CurrentDiagrams.Count);

            _shell.OpenDiagram();

            // add nodes
            var node1 = _shell.PlaceNode(testNode, 5, 6);
            var node2 = _shell.PlaceNode(testNode);

            // wire nodes
            var outputTerm = node1.OutputTerminalViewModels.First();
            var inputTerm = node2.InputTerminalViewModels.First();
            var wireViewModel = _shell.WireTerminals(outputTerm, inputTerm);

            // set output
            node1.InputTerminalViewModels.First().Data = 4;
            outputTerm.Data = 5;
            Assert.AreEqual(((TestPassthroughNode)node1).OutputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);

            // move node2
            node2.X = 16;
            node2.Y = 17;

            var inputTerminalNode2 = node2.InputTerminalViewModels.First().TerminalModel;
            var outputTerminalNode1 = node1.OutputTerminalViewModels.First().TerminalModel;
            Assert.AreEqual(inputTerminalNode2.X, node2.X);
            Assert.AreEqual(inputTerminalNode2.Y, node2.Y);
            Assert.AreEqual(inputTerminalNode2.NodeX + inputTerminalNode2.OffsetX, inputTerminalNode2.X);
            Assert.AreEqual(inputTerminalNode2.NodeY + inputTerminalNode2.OffsetY, inputTerminalNode2.Y);
            Assert.AreEqual(inputTerminalNode2.X + DiagramViewModel.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(inputTerminalNode2.Y + DiagramViewModel.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(outputTerminalNode1.X + DiagramViewModel.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(outputTerminalNode1.Y + DiagramViewModel.NodeBorderWidth, wireViewModel.Y2);

            // save
            projectManager.SaveProject();
            projectManager.CloseProject();
            projectManager.LoadProject();

            var diagramViewModel = _shell.OpenDiagram();
            Assert.AreEqual(2, diagramViewModel.NodeViewModels.Count);
            node1 = diagramViewModel.NodeViewModels[0];
            node2 = diagramViewModel.NodeViewModels[1];

            // check wire nodes
            outputTerm = node1.OutputTerminalViewModels.First();
            inputTerm = node2.InputTerminalViewModels.First();
            Assert.AreNotEqual(0, outputTerm.TerminalModel.ConnectedWires.Count);
            Assert.AreNotEqual(0, inputTerm.TerminalModel.ConnectedWires.Count);
            Assert.AreEqual(5, ((TestPassthroughNode)node2).InputTerminal.Data);
            Assert.AreEqual(5, ((TestPassthroughNode)node1).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestPassthroughNode)node2).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestPassthroughNode)node2).Value);
            Assert.AreEqual(((TestPassthroughNode)node1).OutputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);


            // change data
            outputTerm.Data = 6;
            Assert.AreEqual(((TestPassthroughNode)node1).OutputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);
            Assert.AreEqual(((TestPassthroughNode)node2).InputTerminal.Data + 1, ((TestPassthroughNode)node2).OutputTerminal.Data);

            // change location
            node1.X = 11;
            node1.Y = 12;
            inputTerminalNode2 = inputTerm.TerminalModel;
            outputTerminalNode1 = outputTerm.TerminalModel;
            wireViewModel = diagramWell.ActiveItem.WireViewModels.First();
            Assert.AreEqual(inputTerminalNode2.X, node2.X);
            Assert.AreEqual(inputTerminalNode2.Y, node2.Y);
            Assert.AreEqual(inputTerminalNode2.NodeX + inputTerminalNode2.OffsetX, inputTerminalNode2.X);
            Assert.AreEqual(inputTerminalNode2.NodeY + inputTerminalNode2.OffsetY, inputTerminalNode2.Y);
            Assert.AreEqual(inputTerminalNode2.X + DiagramViewModel.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(inputTerminalNode2.Y + DiagramViewModel.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(outputTerminalNode1.X + DiagramViewModel.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(outputTerminalNode1.Y + DiagramViewModel.NodeBorderWidth, wireViewModel.Y2);
        }
    }
}