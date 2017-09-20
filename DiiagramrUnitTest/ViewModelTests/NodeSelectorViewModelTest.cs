using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class NodeSelectorViewModelTest
    {
        private Mock<IProvideNodes> _nodeProvidorMoq;
        private Mock<PluginNode> _nodeMoq1;
        private Mock<PluginNode> _nodeMoq2;
        private NodeSelectorViewModel _nodeSelectorViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            MockedViewModelFactories.CreateSingletonMoqs();
            _nodeProvidorMoq = MockedViewModelFactories.CreateMoqNodeProvider();
            _nodeMoq1 = new Mock<PluginNode>();
            _nodeMoq2 = new Mock<PluginNode>();
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            _nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
        }

        [TestMethod]
        public void TestConstructor_CollectionsInitialized()
        {
            Assert.IsNotNull(_nodeSelectorViewModel.LibrariesList);
            Assert.IsNotNull(_nodeSelectorViewModel.AvailableNodeViewModels);
            Assert.IsNotNull(_nodeSelectorViewModel.VisibleNodesList);
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsNoNode_NoLibrariesInLibraryList()
        {
            var nodeList = new List<AbstractNodeViewModel>();
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(0, nodeSelectorViewModel.LibrariesList.Count);
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsANode_AddsNodeLibrary()
        {
            var nodeList = new List<AbstractNodeViewModel> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(1, nodeSelectorViewModel.LibrariesList.Count);
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsANode_NodeInAvailableNodesList()
        {
            var nodeList = new List<AbstractNodeViewModel> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(_nodeMoq1.Object, nodeSelectorViewModel.AvailableNodeViewModels.First());
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsANode_NodeInLibaryNodeList()
        {
            var nodeList = new List<AbstractNodeViewModel> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(_nodeMoq1.Object, nodeSelectorViewModel.LibrariesList.First().Nodes.First());
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsAPluginNode_SetupNodeInvokedOnNode()
        {
            var nodeList = new List<AbstractNodeViewModel> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            new NodeSelectorViewModel(NodeProvidorFactory);

            _nodeMoq1.Verify(n => n.SetupNode(It.IsAny<NodeSetup>()));
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsTwoNodes_AddsOnlyOne()
        {
            var nodeList = new List<AbstractNodeViewModel> { _nodeMoq1.Object, _nodeMoq2.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(1, nodeSelectorViewModel.LibrariesList.Count);
        }

        [TestMethod]
        public void TestBackgroundMouseDown_NodeInVisibleNodeList_VisibleNodeListCleared()
        {
            _nodeSelectorViewModel.VisibleNodesList.Add(_nodeMoq1.Object);
            Assert.AreEqual(1, _nodeSelectorViewModel.VisibleNodesList.Count);
            _nodeSelectorViewModel.BackgroundMouseDown();

            Assert.IsTrue(_nodeSelectorViewModel.VisibleNodesList.IsNullOrEmpty());
        }

        [TestMethod]
        public void TestBackgroundMouseDown_NodeMousedOver_MousedOverNodeSetToNull()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;
            _nodeSelectorViewModel.BackgroundMouseDown();

            Assert.IsNull(_nodeSelectorViewModel.MousedOverNode);
        }

        [TestMethod]
        public void TestBackgroundMouseDown_ShouldCloseInvoked()
        {
            var shouldClose = false;
            _nodeSelectorViewModel.ShouldClose += () => shouldClose = true;
            _nodeSelectorViewModel.BackgroundMouseDown();

            Assert.IsTrue(shouldClose);
        }

        [TestMethod]
        public void TestSelectNode_ShouldCloseInvoked()
        {
            var shouldClose = false;
            _nodeSelectorViewModel.ShouldClose += () => shouldClose = true;
            _nodeSelectorViewModel.SelectNode();

            Assert.IsTrue(shouldClose);
        }

        [TestMethod]
        public void TestSelectNode_MousedOverNodeSet_SelectedNodeSetToMousedOverNode()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;
            _nodeSelectorViewModel.SelectNode();

            Assert.AreEqual(_nodeMoq1.Object, _nodeSelectorViewModel.SelectedNode);
        }

        [TestMethod]
        public void TestShowLibrary_NodeAlreadyInVisibleNodes_VisibleNodesCleared()
        {
            _nodeSelectorViewModel.VisibleNodesList.Add(_nodeMoq1.Object);
            var libraryMoq = new Mock<Library>("");
            libraryMoq.SetupGet(l => l.Nodes).Returns(new List<AbstractNodeViewModel>());
            _nodeSelectorViewModel.ShowLibrary(libraryMoq.Object);

            Assert.IsTrue(_nodeSelectorViewModel.VisibleNodesList.IsNullOrEmpty());
        }

        [TestMethod]
        public void TestShowLibrary_NodeInLibrary_NodeAddedToVisibleNodes()
        {
            var libraryMoq = new Mock<Library>("");
            libraryMoq.SetupGet(l => l.Nodes).Returns(new List<AbstractNodeViewModel>{_nodeMoq1.Object});
            _nodeSelectorViewModel.ShowLibrary(libraryMoq.Object);

            Assert.AreEqual(_nodeMoq1.Object, _nodeSelectorViewModel.VisibleNodesList.First());
        }

        [TestMethod]
        public void TestShowLibrary_OtherLibraryExists_OtherLibraryUnselected()
        {
            var libraryMoq1 = new Mock<Library>("");
            var libraryMoq2 = new Mock<Library>("");
            libraryMoq2.SetupGet(l => l.Nodes).Returns(new List<AbstractNodeViewModel>());
            _nodeSelectorViewModel.LibrariesList.Add(libraryMoq1.Object);
            _nodeSelectorViewModel.ShowLibrary(libraryMoq2.Object);

            libraryMoq1.Verify(l => l.Unselect());
        }

        [TestMethod]
        public void TestShowLibrary_LibrarySelected()
        {
            var libraryMoq1 = new Mock<Library>("");
            libraryMoq1.SetupGet(l => l.Nodes).Returns(new List<AbstractNodeViewModel>());
            _nodeSelectorViewModel.ShowLibrary(libraryMoq1.Object);

            libraryMoq1.Verify(l => l.Select());
        }

        [TestMethod]
        public void TestShowLibrary_NodeMousedOver_MousedOverNodeSetToNull()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;
            var libraryMoq1 = new Mock<Library>("");
            libraryMoq1.SetupGet(l => l.Nodes).Returns(new List<AbstractNodeViewModel>());
            _nodeSelectorViewModel.ShowLibrary(libraryMoq1.Object);

            Assert.IsNull(_nodeSelectorViewModel.MousedOverNode);
        }

        [TestMethod]
        public void TestMouseLeftSelector_LibraryExists_LibrariesUnselected()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;
            var libraryMoq1 = new Mock<Library>("");
            _nodeSelectorViewModel.LibrariesList.Add(libraryMoq1.Object);

            _nodeSelectorViewModel.MouseLeftSelector();

            libraryMoq1.Verify(l => l.Unselect());
        }

        [TestMethod]
        public void TestMouseLeftSelector_NodeInVisibleNodeList_VisibleNodeListCleared()
        {
            _nodeSelectorViewModel.VisibleNodesList.Add(_nodeMoq1.Object);
            Assert.AreEqual(1, _nodeSelectorViewModel.VisibleNodesList.Count);

            _nodeSelectorViewModel.MouseLeftSelector();

            Assert.IsTrue(_nodeSelectorViewModel.VisibleNodesList.IsNullOrEmpty());
        }

        [TestMethod]
        public void TestMouseLeftSelector_NodeMousedOver_MousedOverNodeSetToNull()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;

            _nodeSelectorViewModel.MouseLeftSelector();

            Assert.IsNull(_nodeSelectorViewModel.MousedOverNode);
        }
    }
}
