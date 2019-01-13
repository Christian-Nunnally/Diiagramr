using Castle.Core.Internal;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class NodeSelectorViewModelTest
    {
        private Mock<PluginNode> _nodeMoq1;
        private Mock<PluginNode> _nodeMoq2;
        private Mock<IProvideNodes> _nodeProvidorMoq;
        private NodeSelectorViewModel _nodeSelectorViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _nodeProvidorMoq = new Mock<IProvideNodes>();
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
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(new List<PluginNode>());
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);

            Assert.AreEqual(0, nodeSelectorViewModel.LibrariesList.Count);
        }

        [TestMethod]
        public void TestVisible_ProviderReturnsANode_LazilyAddsNodeLibrary()
        {
            var nodeList = new List<PluginNode> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
            Assert.AreEqual(0, nodeSelectorViewModel.LibrariesList.Count);

            nodeSelectorViewModel.Visible = true;

            Assert.AreEqual(1, nodeSelectorViewModel.LibrariesList.Count);
        }

        [TestMethod]
        public void TestVisible_ProviderReturnsANode_NodeInAvailableNodesListLazily()
        {
            var nodeList = new List<PluginNode> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
            Assert.IsNull(nodeSelectorViewModel.AvailableNodeViewModels.FirstOrDefault());

            nodeSelectorViewModel.Visible = true;
            Assert.AreEqual(_nodeMoq1.Object, nodeSelectorViewModel.AvailableNodeViewModels.First());
        }

        [TestMethod]
        public void TestConstructor_ProviderReturnsANode_NodeInLibaryNodeListLazily()
        {
            var nodeList = new List<PluginNode> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
            Assert.IsNull(nodeSelectorViewModel.LibrariesList.FirstOrDefault()?.Nodes?.FirstOrDefault());

            nodeSelectorViewModel.Visible = true;

            Assert.AreEqual(_nodeMoq1.Object, nodeSelectorViewModel.LibrariesList.First().Nodes.First());
        }

        [TestMethod]
        public void TestVisible_ProviderReturnsAPluginNode_InitializeWithNodeInvokedOnNodeLazily()
        {
            var nodeList = new List<PluginNode> { _nodeMoq1.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;

            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
            _nodeMoq1.Verify(n => n.InitializeWithNode(It.IsAny<NodeModel>()), Times.Never);
            nodeSelectorViewModel.Visible = true;

            _nodeMoq1.Verify(n => n.InitializeWithNode(It.IsAny<NodeModel>()));
        }

        [TestMethod]
        public void TestVisible_ProviderReturnsTwoNodes_AddsOnlyOneLazily()
        {
            var nodeList = new List<PluginNode> { _nodeMoq1.Object, _nodeMoq2.Object };
            _nodeProvidorMoq.Setup(p => p.GetRegisteredNodes()).Returns(nodeList);
            IProvideNodes NodeProvidorFactory() => _nodeProvidorMoq.Object;
            var nodeSelectorViewModel = new NodeSelectorViewModel(NodeProvidorFactory);
            Assert.AreEqual(0, nodeSelectorViewModel.LibrariesList.Count);

            nodeSelectorViewModel.Visible = true;

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
        public void TestBackgroundMouseDown_VisibleSetToFalse()
        {
            _nodeSelectorViewModel.BackgroundMouseDown();
            Assert.IsFalse(_nodeSelectorViewModel.Visible);
        }

        [TestMethod]
        public void TestSelectNode_VisibleSetToFalse()
        {
            _nodeSelectorViewModel.SelectNode();
            Assert.IsFalse(_nodeSelectorViewModel.Visible);
        }

        [TestMethod]
        public void NodeMousedOver_SelectNode_NodeSelectedEventFiredForMousedOverNode()
        {
            var nodeSelected = false;
            _nodeSelectorViewModel.NodeSelected += n => nodeSelected = _nodeMoq1.Object == n;
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;

            _nodeSelectorViewModel.SelectNode();

            Assert.IsTrue(nodeSelected);
        }

        [TestMethod]
        public void TestShowLibrary_NodeAlreadyInVisibleNodes_VisibleNodesCleared()
        {
            _nodeSelectorViewModel.VisibleNodesList.Add(_nodeMoq1.Object);
            var libraryMoq = new Mock<Library>("");
            libraryMoq.SetupGet(l => l.Nodes).Returns(new List<PluginNode>());
            _nodeSelectorViewModel.ShowLibrary(libraryMoq.Object);

            Assert.IsTrue(_nodeSelectorViewModel.VisibleNodesList.IsNullOrEmpty());
        }

        [TestMethod]
        public void TestShowLibrary_NodeInLibrary_NodeAddedToVisibleNodes()
        {
            var libraryMoq = new Mock<Library>("");
            libraryMoq.SetupGet(l => l.Nodes).Returns(new List<PluginNode> { _nodeMoq1.Object });
            _nodeSelectorViewModel.ShowLibrary(libraryMoq.Object);

            Assert.AreEqual(_nodeMoq1.Object, _nodeSelectorViewModel.VisibleNodesList.First());
        }

        [TestMethod]
        public void TestShowLibrary_OtherLibraryExists_OtherLibraryUnselected()
        {
            var libraryMoq1 = new Mock<Library>("");
            var libraryMoq2 = new Mock<Library>("");
            libraryMoq2.SetupGet(l => l.Nodes).Returns(new List<PluginNode>());
            _nodeSelectorViewModel.LibrariesList.Add(libraryMoq1.Object);
            _nodeSelectorViewModel.ShowLibrary(libraryMoq2.Object);

            libraryMoq1.Verify(l => l.Unselect());
        }

        [TestMethod]
        public void TestShowLibrary_LibrarySelected()
        {
            var libraryMoq1 = new Mock<Library>("");
            libraryMoq1.SetupGet(l => l.Nodes).Returns(new List<PluginNode>());
            _nodeSelectorViewModel.ShowLibrary(libraryMoq1.Object);

            libraryMoq1.Verify(l => l.Select());
        }

        [TestMethod]
        public void TestShowLibrary_NodeMousedOver_MousedOverNodeSetToNull()
        {
            _nodeSelectorViewModel.MousedOverNode = _nodeMoq1.Object;
            var libraryMoq1 = new Mock<Library>("");
            libraryMoq1.SetupGet(l => l.Nodes).Returns(new List<PluginNode>());
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

        [TestMethod]
        public void TestLibrarySelect_SetsBackgroundColorToNonWhite()
        {
            var library = new Library("");
            library.Select();
            var brush = (SolidColorBrush)library.BackgroundBrush;
            Assert.IsFalse(brush.Color.R == 255 && brush.Color.G == 255 && brush.Color.B == 255);
        }

        [TestMethod]
        public void TestLibraryUnselect_SetsBackgroundColorToWhite()
        {
            var library = new Library("");
            library.Unselect();
            var brush = (SolidColorBrush)library.BackgroundBrush;
            Assert.IsTrue(brush.Color.R == 255 && brush.Color.G == 255 && brush.Color.B == 255);
        }
    }
}
