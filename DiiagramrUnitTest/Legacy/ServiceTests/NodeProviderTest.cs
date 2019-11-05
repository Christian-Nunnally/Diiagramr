using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace DiiagramrUnitTests.Legacy.ServiceTests
{
    [TestClass]
    public class NodeProviderTest
    {
        private NodeProvider _nodeProvider;
        private Mock<Node> _nodeViewModelMoq;
        private NodeModel _testNode;

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());

            var nodeViewModel = _nodeProvider.CreateNodeFromName(_nodeViewModelMoq.Object.GetType().FullName);
            nodeViewModel.X++;
            nodeViewModel.Y++;

            Assert.AreEqual(nodeViewModel.Model.X, nodeViewModel.X);
            Assert.AreEqual(nodeViewModel.Model.Y, nodeViewModel.Y);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_ReturnsNewViewModel()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());

            var nodeViewModel = _nodeProvider.CreateNodeFromName(_nodeViewModelMoq.Object.GetType().FullName);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());

            var nodeViewModel = _nodeProvider.CreateNodeFromName(_nodeViewModelMoq.Object.GetType().FullName);

            Assert.IsNotNull(nodeViewModel.Model);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _nodeProvider = new NodeProvider();
            _nodeViewModelMoq = new Mock<Node>
            {
                CallBase = false
            };
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _testNode = new NodeModel("")
            {
                Name = "TestNodeViewModel"
            };
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _testNode.Name = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);
            nodeViewModel.X++;
            nodeViewModel.Y++;

            Assert.AreEqual(_testNode.X, nodeViewModel.X);
            Assert.AreEqual(_testNode.Y, nodeViewModel.Y);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ReturnsNewViewModel()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _testNode.Name = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelPositionSet()
        {
            _testNode.X = 10;
            _testNode.Y = 11;
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _testNode.Name = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(_testNode.X, nodeViewModel.X);
            Assert.AreEqual(_testNode.Y, nodeViewModel.Y);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelSizeSet()
        {
            _testNode.Width = 10;
            _testNode.Height = 11;
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _testNode.Name = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(_testNode.Width, nodeViewModel.Width);
            Assert.AreEqual(_testNode.Height, nodeViewModel.Height);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _testNode.Name = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(nodeViewModel.Model, _testNode);
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNode_NodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            Assert.IsTrue(_nodeProvider.GetRegisteredNodes().Contains(_nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNodeTwice_OnlyOneNodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            Assert.AreEqual(1, _nodeProvider.GetRegisteredNodes().Count(m => m == _nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterTwoNodeWithSameFullyQualifiedName_OnlyOneNodeReturnedByGetRegisteredNodes()
        {
            var otherNodeViewModel = new Mock<Node>();
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new NodeLibrary());
            _nodeProvider.RegisterNode(otherNodeViewModel.Object, new NodeLibrary());
            Assert.AreEqual(1, _nodeProvider.GetRegisteredNodes().Count(m => m == _nodeViewModelMoq.Object));
        }
    }
}