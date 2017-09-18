﻿using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
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
        private DiagramViewModel _diagramViewModel;
        private Mock<EDiagram> _diagramMoq;
        private Mock<DiagramNode> _nodeMoq;
        private Mock<AbstractNodeViewModel> _abstractNodeViewModelMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _diagramMoq = new Mock<EDiagram>();
            _diagramMoq.Setup(d => d.AddNode(It.IsAny<DiagramNode>())).Verifiable();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _diagramViewModel = new DiagramViewModel(_diagramMoq.Object, _nodeProviderMoq.Object);
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
        [ExpectedException(typeof(ArgumentNullException), "Diagram view model requires a diagram")]
        public void TestConstructor_NullDiagram_ThrowsArgumentNullException()
        {
            new DiagramViewModel(null, _nodeProviderMoq.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Diagram view model requires a node provider")]
        public void TestConstructor_NullNodeProvider_ThrowsArgumentNullException()
        {
            new DiagramViewModel(_diagramMoq.Object, null);
        }

        private void ConstructDiagramViewModelWithDiagramThatAlreadyHasANode()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(n => n.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _nodeProviderMoq.Setup(n => n.LoadNodeViewModelFromNode(It.IsAny<DiagramNode>())).Returns(_abstractNodeViewModelMoq.Object);
            _diagramViewModel = new DiagramViewModel(_diagramMoq.Object, _nodeProviderMoq.Object);
        }

        [TestMethod]
        public void TestConstructor_NodeAlreadyOnDiagram_LoadsNodeViewModel()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _nodeProviderMoq.Verify(p => p.LoadNodeViewModelFromNode(It.Is<DiagramNode>(n => n == _nodeMoq.Object)));
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

        [TestMethod]
        public void TestLeftMouseButtonDown_NodeSelected_UnselectsNode()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _abstractNodeViewModelMoq.SetupAllProperties();
            _abstractNodeViewModelMoq.Object.IsSelected = true;
            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count(node => node.IsSelected));

            _diagramViewModel.LeftMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(0, _diagramViewModel.NodeViewModels.Count(node => node.IsSelected));
        }

        [TestMethod]
        public void TestSetInsertingNodeViewModel_AddsNodeToDiagram()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;

            _diagramViewModel.PreviewLeftMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestPreviewRightMouseButtonDown_InsertingNode_InsertingNodeSetToNull()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;

            _diagramViewModel.PreviewRightMouseButtonDown(new Point(0, 0));

            Assert.IsNull(_diagramViewModel.InsertingNodeViewModel);
        }

        [TestMethod]
        public void TestPreviewRightMouseButtonDown_InsertingNode_InsertingNodeRemovedFromDiagram()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;

            _diagramViewModel.PreviewRightMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(0, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDown_InsertingNode_InsertingNodeViewModelSetToNull()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;

            _diagramViewModel.PreviewLeftMouseButtonDown(new Point(0, 0));

            Assert.IsNull(_diagramViewModel.InsertingNodeViewModel);
        }

        [TestMethod]
        public void TestMouseMoved_NotInsertingNode_DoesNothing()
        {
            _diagramViewModel.MouseMoved(new Point(0, 0));
        }

        [TestMethod]
        public void TestMouseMoved_InsertingNode_SetsInsertingNodePosition()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _abstractNodeViewModelMoq.SetupGet(m => m.X).Returns(10);
            _abstractNodeViewModelMoq.SetupGet(m => m.Y).Returns(10);
            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;

            _diagramViewModel.MouseMoved(new Point(5, 5));

            _abstractNodeViewModelMoq.VerifySet(m => m.X = -5);
            _abstractNodeViewModelMoq.VerifySet(m => m.Y = -5);
        }

        [TestMethod]
        public void TestMouseMoved_InsertingNode_SetsInsertingNodePositionCenteredOnMouseRespectingZoom()
        {
            _nodeMoq = new Mock<DiagramNode>("node");
            _abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _abstractNodeViewModelMoq.SetupGet(m => m.DiagramNode).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<DiagramNode> { _nodeMoq.Object });
            _abstractNodeViewModelMoq.SetupGet(m => m.X).Returns(10);
            _abstractNodeViewModelMoq.SetupGet(m => m.Y).Returns(10);
            _abstractNodeViewModelMoq.SetupGet(m => m.Width).Returns(2);
            _abstractNodeViewModelMoq.SetupGet(m => m.Height).Returns(2);

            _diagramViewModel.InsertingNodeViewModel = _abstractNodeViewModelMoq.Object;
            _diagramViewModel.Zoom = 2;
            _diagramViewModel.MouseMoved(new Point(5, 5));

            _abstractNodeViewModelMoq.VerifySet(m => m.X = -8.5);
            _abstractNodeViewModelMoq.VerifySet(m => m.Y = -8.5);
        }
    }
}