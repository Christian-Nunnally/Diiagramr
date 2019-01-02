using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrIntegrationTest.IntegrationHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyletIoC;
using System.Linq;

namespace DiiagramrIntegrationTest
{
    [TestClass]
    public class DiagramCallNodeIntegrationTest
    {
        private IContainer _container;
        private ShellViewModel _shell;

        [TestInitialize]
        public void TestInitialize()
        {
            _shell = IntegrationTestUtilities.SetupShellViewModel();
            _container = IntegrationTestUtilities.Container;
        }

        public void TestSimpleWiredInputOutput()
        {
            var projectScreen = _shell.ProjectScreenViewModel;
            var nodeSelectorViewModel = (NodeSelectorViewModel)_container.Get(typeof(NodeSelectorViewModel));
            var projectManager = projectScreen.ProjectExplorerViewModel.ProjectManager;

            nodeSelectorViewModel.Visible = true;
            var inputNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(DiagramInputNodeViewModel));
            var outputNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(DiagramOutputNodeViewModel));
            var testNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(TestPassthroughNode));

            _shell.CreateProject();
            projectManager.CreateDiagram();
            projectManager.CreateDiagram();
            var diagramViewModel2 = _shell.OpenDiagram(1);
            var diagramViewModel1 = _shell.OpenDiagram(0);

            var diagramCallNode = diagramViewModel2.PlaceDiagramCallNodeFor(diagramViewModel1.Diagram);
            Assert.AreEqual(0, diagramCallNode.TerminalViewModels.Count);

            var inputNodeOnDiagram1 = _shell.PlaceNode(inputNode, 0, 0);
            Assert.AreEqual(1, diagramCallNode.InputTerminalViewModels.Count());

            var outputNodeOnDiagram1 = _shell.PlaceNode(outputNode, 0, 0);
            Assert.AreEqual(1, diagramCallNode.OutputTerminalViewModels.Count());

            _shell.WireTerminals(inputNodeOnDiagram1.TerminalViewModels.First(), outputNodeOnDiagram1.TerminalViewModels.First());

            _shell.OpenDiagram(1);

            var testNodeInput = _shell.PlaceNode(testNode, 0, 0);
            var testNodeOutput = _shell.PlaceNode(testNode, 0, 0);

            _shell.WireTerminals(testNodeInput.OutputTerminalViewModels.First(), diagramCallNode.InputTerminalViewModels.First());
            _shell.WireTerminals(diagramCallNode.OutputTerminalViewModels.First(), testNodeOutput.InputTerminalViewModels.First());

            testNodeInput.OutputTerminalViewModels.First().Data = 5;
            Assert.AreEqual(5, testNodeOutput.InputTerminalViewModels.First().Data);

            inputNodeOnDiagram1.DisconnectAllTerminals();

            testNodeInput.OutputTerminalViewModels.First().Data = 6;
            Assert.IsNull(testNodeOutput.InputTerminalViewModels.First().Data);
        }

        /// <summary>
        ///     This test is failing.  It describes a scenario in which diagram call nodes do not work perfectly.
        ///     The scenario is as follows:
        ///     Diagram1:
        ///     ConstantNode -> OutputNode
        ///     Diagram2:
        ///     Diagram1CallNode -> ValueDisplayNode
        ///     The value diplay node should display the constant value set in the constant node because its value is a plugin node
        ///     setting,
        ///     but the value is not propagated when the diagram is serialized, meaning the output terminal on the DCN is always
        ///     set to null.
        /// </summary>
        [Ignore]
        public void TestPluginNodeSettingUpdatesDiagramCallNode()
        {
            var projectScreen = _shell.ProjectScreenViewModel;
            var nodeSelectorViewModel = (NodeSelectorViewModel)_container.Get(typeof(NodeSelectorViewModel));
            var projectManager = projectScreen.ProjectExplorerViewModel.ProjectManager;

            nodeSelectorViewModel.Visible = true;
            var outputNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(DiagramOutputNodeViewModel));
            var testPassthroughNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(TestPassthroughNode));
            var testIntNode = nodeSelectorViewModel.AvailableNodeViewModels.First(n => n.GetType() == typeof(TestIntNode));

            _shell.CreateProject();
            projectManager.CreateDiagram();
            projectManager.CreateDiagram();
            var diagramViewModel2 = _shell.OpenDiagram(1);
            var diagramViewModel1 = _shell.OpenDiagram(0);

            var diagramCallNode = diagramViewModel2.PlaceDiagramCallNodeFor(diagramViewModel1.Diagram);
            var outputNodeOnDiagram1 = _shell.PlaceNode(outputNode, 0, 0);
            var testIntNodeOnDiagram1 = (TestIntNode)_shell.PlaceNode(testIntNode, 0, 0);

            _shell.WireTerminals(testIntNodeOnDiagram1.TerminalViewModels.First(), outputNodeOnDiagram1.TerminalViewModels.First());

            _shell.OpenDiagram(1);

            var testNodeOutput = _shell.PlaceNode(testPassthroughNode, 0, 0);

            _shell.WireTerminals(diagramCallNode.OutputTerminalViewModels.First(), testNodeOutput.InputTerminalViewModels.First());

            testIntNodeOnDiagram1.SetValue(4);
            Assert.AreEqual(4, testNodeOutput.InputTerminalViewModels.First().Data);

            testIntNodeOnDiagram1.DisconnectAllTerminals();

            testIntNodeOnDiagram1.SetValue(5);
            Assert.IsNull(testNodeOutput.InputTerminalViewModels.First().Data);
        }
    }
}