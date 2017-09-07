using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class NodeProviderTests
    {
        private NodeProvider _nodeProvider;
        private Mock<AbstractNodeViewModel> _nodeViewModelMoq;
        private DiagramNode _testNode;

        [TestInitialize]
        public void TestInitialize()
        {
            _nodeProvider = new NodeProvider(() => new AbstractNodeViewModel[0]);
            _nodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _testNode = new DiagramNode("");
            _testNode.NodeType = "TestNodeViewModel";
        }

        [TestMethod]
        public void TestConstructor_InjectNode_InjectedNodeRegistered()
        {
            var nodeProvider = new NodeProvider(() => new List<AbstractNodeViewModel> { _nodeViewModelMoq.Object });
            Assert.IsTrue(nodeProvider.GetRegisteredNodes().Contains(_nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNode_NodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);
            Assert.IsTrue(_nodeProvider.GetRegisteredNodes().Contains(_nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNodeTwice_OnlyOneNodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);
            Assert.AreEqual(1, _nodeProvider.GetRegisteredNodes().Count(m => m == _nodeViewModelMoq.Object));
        }

        [TestMethod]
        [ExpectedException(typeof(NodeProviderException), "No registered view model for given node.")]
        public void TestLoadNodeViewModelFromNode_ViewModelNotRegistered_ThrowsException()
        {
            _nodeProvider.LoadNodeViewModelFromNode(_testNode);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ReturnsNewViewModel()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_NodesViewModelSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(nodeViewModel, _testNode.NodeViewModel);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(nodeViewModel.DiagramNode, _testNode);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelPositionSet()
        {
            _testNode.X = 10;
            _testNode.Y = 10;
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(_testNode.X, nodeViewModel.X);
            Assert.AreEqual(_testNode.Y, nodeViewModel.Y);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);
            nodeViewModel.X++;
            nodeViewModel.Y++;

            Assert.AreEqual(_testNode.X, nodeViewModel.X);
            Assert.AreEqual(_testNode.Y, nodeViewModel.Y);
        }

        [TestMethod]
        [ExpectedException(typeof(NodeProviderException), "No registered view model for given node.")]
        public void TestCreateNodeViewModelFromName_ViewModelNotRegistered_ThrowsException()
        {
            _nodeProvider.CreateNodeViewModelFromName("");
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_ReturnsNewViewModel()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.Name);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_NodesViewModelSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.Name);

            Assert.AreEqual(nodeViewModel, nodeViewModel.DiagramNode.NodeViewModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.Name);

            Assert.IsNotNull(nodeViewModel.DiagramNode);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object);

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.Name);
            nodeViewModel.X++;
            nodeViewModel.Y++;

            Assert.AreEqual(nodeViewModel.DiagramNode.X, nodeViewModel.X);
            Assert.AreEqual(nodeViewModel.DiagramNode.Y, nodeViewModel.Y);
        }
    }
}