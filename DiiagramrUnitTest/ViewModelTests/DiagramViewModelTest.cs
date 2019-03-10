using DiiagramrAPI.Diagram;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class DiagramViewModelTest
    {
        private Mock<DiagramModel> _diagramMoq;
        private DiagramViewModel _diagramViewModel;
        private Mock<NodeModel> _nodeMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorViewModelMoq;
        private Mock<PluginNode> _pluginNodeMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _diagramMoq = new Mock<DiagramModel>();
            _diagramMoq.Setup(d => d.AddNode(It.IsAny<NodeModel>())).Verifiable();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _nodeSelectorViewModelMoq = new Mock<NodeSelectorViewModel>((Func<IProvideNodes>)(() => _nodeProviderMoq.Object));
            _diagramViewModel = new DiagramViewModel(_diagramMoq.Object, _nodeProviderMoq.Object, null, _nodeSelectorViewModelMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_CorrectArguments_ChildViewModelCollectionsNotNull()
        {
            Assert.IsNotNull(_diagramViewModel.NodeViewModels);
            Assert.IsNotNull(_diagramViewModel.WireViewModels);
        }

        [TestMethod]
        public void TestConstructor_CorrectArguments_DiagramIsNotNull()
        {
            Assert.IsNotNull(_diagramViewModel.Diagram);
        }

        [TestMethod]
        public void TestConstructor_SetsDiagramControlViewModel()
        {
            Assert.IsNotNull(_diagramViewModel.DiagramControlViewModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Diagram view model requires a node provider")]
        public void TestConstructor_NullNodeProvider_ThrowsArgumentNullException()
        {
            new DiagramViewModel(_diagramMoq.Object, null, null, _nodeSelectorViewModelMoq.Object);
        }

        private void ConstructDiagramViewModelWithDiagramThatAlreadyHasANode()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(n => n.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> { _nodeMoq.Object });
            _nodeProviderMoq.Setup(n => n.LoadNodeViewModelFromNode(It.IsAny<NodeModel>())).Returns(_pluginNodeMoq.Object);
            _diagramViewModel = new DiagramViewModel(_diagramMoq.Object, _nodeProviderMoq.Object, null, _nodeSelectorViewModelMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_LoadsNodeViewModel()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _nodeProviderMoq.Verify(p => p.LoadNodeViewModelFromNode(It.Is<NodeModel>(n => n == _nodeMoq.Object)));
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_SetsUpTerminalPropertyChangedNotificationsOnNode()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _nodeMoq.Verify(p => p.SetTerminalsPropertyChanged());
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_AddsNodeViewModelToViewModelList()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count);
        }
    }
}
