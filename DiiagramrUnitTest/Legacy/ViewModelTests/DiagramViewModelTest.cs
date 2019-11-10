using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Editor;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace DiiagramrUnitTests.Legacy.ViewModelTests
{
    [TestClass]
    public class DiagramViewModelTest
    {
        private Mock<DiagramModel> _diagramMoq;
        private Diagram _diagramViewModel;
        private Mock<NodeModel> _nodeMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<Node> _pluginNodeMoq;

        [TestMethod]
        public void TestConstructor_CorrectArguments_ChildViewModelCollectionsNotNull()
        {
            Assert.IsNotNull(_diagramViewModel.Nodes);
            Assert.IsNotNull(_diagramViewModel.Wires);
        }

        [TestMethod]
        public void TestConstructor_CorrectArguments_DiagramIsNotNull()
        {
            Assert.IsNotNull(_diagramViewModel.Model);
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_AddsNodeViewModelToViewModelList()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            Assert.AreEqual(1, _diagramViewModel.Nodes.Count);
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_LoadsNodeViewModel()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _nodeProviderMoq.Verify(p => p.LoadNodeViewModelFromNode(It.Is<NodeModel>(n => n == _nodeMoq.Object)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Diagram view model requires a node provider")]
        public void TestConstructor_NullNodeProvider_ThrowsArgumentNullException()
        {
            new Diagram(_diagramMoq.Object, null, new List<DiagramInteractor>());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _diagramMoq = new Mock<DiagramModel>();
            _diagramMoq.Setup(d => d.AddNode(It.IsAny<NodeModel>())).Verifiable();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _diagramViewModel = new Diagram(_diagramMoq.Object, _nodeProviderMoq.Object, new List<DiagramInteractor>());
        }

        private void ConstructDiagramViewModelWithDiagramThatAlreadyHasANode()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<Node>();
            _pluginNodeMoq.SetupGet(n => n.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> { _nodeMoq.Object });
            _nodeProviderMoq.Setup(n => n.LoadNodeViewModelFromNode(It.IsAny<NodeModel>())).Returns(_pluginNodeMoq.Object);
            _diagramViewModel = new Diagram(_diagramMoq.Object, _nodeProviderMoq.Object, new List<DiagramInteractor>());
        }
    }
}