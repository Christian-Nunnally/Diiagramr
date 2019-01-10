using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;
using System.Linq;

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
        public void EmptyDiagram_SaveLoad_DiagramStateRestored()
        {
            const string TestDiagramName = nameof(TestDiagramName);
            _shell.ExecuteCommand("File:New");
            var projectExplorer = _shell.ProjectScreenViewModel.ProjectExplorerViewModel;
            var projectManager = projectExplorer.ProjectManager;
            projectManager.CreateDiagram();
            var diagram = projectManager.CurrentDiagrams.First();
            var oldDiagramId = diagram.Id;
            diagram.Name = TestDiagramName;
            Assert.AreEqual(0, diagram.Nodes.Count);

            SaveCloseLoadProject(_shell);

            diagram = projectManager.CurrentDiagrams.First();
            Assert.IsNotNull(projectExplorer.Project);
            Assert.AreEqual(0, diagram.Nodes.Count);
            Assert.AreEqual(TestDiagramName, diagram.Name);
            Assert.AreEqual(oldDiagramId, diagram.Id);
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
            _shell.ExecuteCommand("File:New");
            projectManager.CurrentProject.IsDirty = false;
            Assert.IsNotNull(projectManager.CurrentProject);

            projectManager.CreateDiagram();
            Assert.AreEqual(1, projectManager.CurrentDiagrams.Count);

            _shell.OpenDiagram();

            // add nodes
            var node1 = _shell.PlaceNode(testNode, 5, 6);
            var node2 = _shell.PlaceNode(testNode);

            // wire nodes
            var outputTerminal = node1.OutputTerminalViewModels.First();
            var inputTerminal = node2.InputTerminalViewModels.First();
            var wireViewModel = _shell.WireTerminals(outputTerminal, inputTerminal);

            // set output
            node1.InputTerminalViewModels.First().Data = 4;
            outputTerminal.Data = 5;
            Assert.AreEqual(outputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);

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

            projectManager.DiagramViewModels.ForEach(x => x.Diagram.Play());

            var diagramViewModel = _shell.OpenDiagram();
            Assert.AreEqual(2, diagramViewModel.NodeViewModels.Count);
            node1 = diagramViewModel.NodeViewModels[0];
            node2 = diagramViewModel.NodeViewModels[1];

            // check wire nodes
            outputTerminal = node1.OutputTerminalViewModels.First();
            inputTerminal = node2.InputTerminalViewModels.First();
            Assert.AreNotEqual(0, outputTerminal.TerminalModel.ConnectedWires.Count);
            Assert.AreNotEqual(0, inputTerminal.TerminalModel.ConnectedWires.Count);
            Assert.AreEqual(5, ((TestPassthroughNode)node2).InputTerminal.Data);
            Assert.AreEqual(5, ((TestPassthroughNode)node1).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestPassthroughNode)node2).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestPassthroughNode)node2).Value);
            Assert.AreEqual(((TestPassthroughNode)node1).OutputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);


            // change data
            outputTerminal.Data = 6;
            Assert.AreEqual(((TestPassthroughNode)node1).OutputTerminal.Data, ((TestPassthroughNode)node2).InputTerminal.Data);
            Assert.AreEqual(((TestPassthroughNode)node2).InputTerminal.Data + 1, ((TestPassthroughNode)node2).OutputTerminal.Data);

            // change location
            node1.X = 11;
            node1.Y = 12;
            inputTerminalNode2 = inputTerminal.TerminalModel;
            outputTerminalNode1 = outputTerminal.TerminalModel;
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

        private void SaveCloseLoadProject(ShellViewModel shell)
        {
            var projectManager = shell.ProjectScreenViewModel.ProjectExplorerViewModel.ProjectManager;
            projectManager.SaveProject();
            projectManager.CloseProject();
            projectManager.LoadProject();
        }
    }
}