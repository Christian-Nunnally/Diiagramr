using System.Linq;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ServiceTests
{
    [TestClass]
    public class NodeProviderTest
    {
        private NodeProvider _nodeProvider;
        private Mock<PluginNode> _nodeViewModelMoq;
        private NodeModel _testNode;

        [TestInitialize]
        public void TestInitialize()
        {
            _nodeProvider = new NodeProvider();
            _nodeViewModelMoq = new Mock<PluginNode>();
            _nodeViewModelMoq.CallBase = false;
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _nodeViewModelMoq.SetupGet(m => m.Name).Returns("TestNodeViewModel");
            _testNode = new NodeModel("");
            _testNode.NodeFullName = "TestNodeViewModel";
        }

        [Ignore]
        [TestMethod]
        public void TestConstructor_InjectNode_InjectedNodeRegistered()
        {
            var nodeProvider = new NodeProvider();
            Assert.IsTrue(nodeProvider.GetRegisteredNodes().Contains(_nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNode_NodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            Assert.IsTrue(_nodeProvider.GetRegisteredNodes().Contains(_nodeViewModelMoq.Object));
        }

        [TestMethod]
        public void TestRegisterNode_RegisterNodeTwice_OnlyOneNodeReturnedByGetRegisteredNodes()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
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
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_NodesViewModelSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(nodeViewModel, _testNode.NodeViewModel);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(nodeViewModel.NodeModel, _testNode);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelPositionSet()
        {
            _testNode.X = 10;
            _testNode.Y = 11;
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(_testNode.X, nodeViewModel.X);
            Assert.AreEqual(_testNode.Y, nodeViewModel.Y);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelRegistered_ViewModelSizeSet()
        {
            _testNode.Width = 10;
            _testNode.Height = 11;
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

            var nodeViewModel = _nodeProvider.LoadNodeViewModelFromNode(_testNode);

            Assert.AreEqual(_testNode.Width, nodeViewModel.Width);
            Assert.AreEqual(_testNode.Height, nodeViewModel.Height);
        }

        [TestMethod]
        public void TestLoadNodeViewModelFromNode_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));
            _testNode.NodeFullName = _nodeViewModelMoq.Object.GetType().FullName;

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
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.GetType().FullName);

            Assert.AreNotEqual(_nodeViewModelMoq.Object, nodeViewModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_NodesViewModelSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.GetType().FullName);

            Assert.AreEqual(nodeViewModel, nodeViewModel.NodeModel.NodeViewModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelRegistered_ViewModelsNodeSet()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.GetType().FullName);

            Assert.IsNotNull(nodeViewModel.NodeModel);
        }

        [TestMethod]
        public void TestCreateNodeViewModelFromName_ViewModelPositionChanges_UpdatesModelPosition()
        {
            _nodeProvider.RegisterNode(_nodeViewModelMoq.Object, new DependencyModel("", ""));

            var nodeViewModel = _nodeProvider.CreateNodeViewModelFromName(_nodeViewModelMoq.Object.GetType().FullName);
            nodeViewModel.X++;
            nodeViewModel.Y++;

            Assert.AreEqual(nodeViewModel.NodeModel.X, nodeViewModel.X);
            Assert.AreEqual(nodeViewModel.NodeModel.Y, nodeViewModel.Y);
        }
    }
}