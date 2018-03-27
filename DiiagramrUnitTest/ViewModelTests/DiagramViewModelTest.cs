using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram.CoreNode;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            _nodeSelectorViewModelMoq = new Mock<NodeSelectorViewModel>((Func<IProvideNodes>) (() => _nodeProviderMoq.Object));
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
        [ExpectedException(typeof(ArgumentNullException), "Diagram view model requires a diagram")]
        public void TestConstructor_NullDiagram_ThrowsArgumentNullException()
        {
            new DiagramViewModel(null, _nodeProviderMoq.Object, null, _nodeSelectorViewModelMoq.Object);
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
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
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

        [TestMethod]
        public void TestLeftMouseButtonDown_NodeSelected_UnselectsNode()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _pluginNodeMoq.SetupAllProperties();
            _pluginNodeMoq.Object.IsSelected = true;
            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count(node => node.IsSelected));

            _diagramViewModel.LeftMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(0, _diagramViewModel.NodeViewModels.Count(node => node.IsSelected));
        }

        [TestMethod]
        public void TestRemoveNodePressed_NodeNotSelected_DoesNotRemoveNode()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _diagramViewModel.RemoveNodePressed();
            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestRemoveNodePressed_NodeSelected_RemovesNode()
        {
            ConstructDiagramViewModelWithDiagramThatAlreadyHasANode();
            _pluginNodeMoq.SetupGet(n => n.IsSelected).Returns(true);
            _diagramViewModel.RemoveNodePressed();
            Assert.AreEqual(0, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestSetInsertingNodeViewModel_AddsNodeToDiagram()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;

            _diagramViewModel.PreviewLeftMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(1, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestPreviewRightMouseButtonDown_InsertingNode_InsertingNodeSetToNull()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _pluginNodeMoq.SetupGet(n => n.TerminalViewModels).Returns(new List<TerminalViewModel>());
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;

            _diagramViewModel.PreviewRightMouseButtonDown(new Point(0, 0));

            Assert.IsNull(_diagramViewModel.InsertingNodeViewModel);
        }

        [TestMethod]
        public void TestPreviewRightMouseButtonDown_InsertingNode_InsertingNodeRemovedFromDiagram()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _pluginNodeMoq.SetupGet(n => n.TerminalViewModels).Returns(new List<TerminalViewModel>());
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;

            _diagramViewModel.PreviewRightMouseButtonDown(new Point(0, 0));

            Assert.AreEqual(0, _diagramViewModel.NodeViewModels.Count);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDown_InsertingNode_InsertingNodeViewModelSetToNull()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;

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
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _pluginNodeMoq.SetupGet(m => m.X).Returns(10);
            _pluginNodeMoq.SetupGet(m => m.Y).Returns(10);
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;

            _diagramViewModel.MouseMoved(new Point(5, 5));

            _pluginNodeMoq.VerifySet(m => m.X = -10);
            _pluginNodeMoq.VerifySet(m => m.Y = -10);
        }

        [TestMethod]
        public void TestMouseMoved_InsertingNode_SetsInsertingNodePositionCenteredOnMouseRespectingZoom()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> {_nodeMoq.Object});
            _pluginNodeMoq.SetupGet(m => m.X).Returns(10);
            _pluginNodeMoq.SetupGet(m => m.Y).Returns(10);
            _pluginNodeMoq.SetupGet(m => m.Width).Returns(2);
            _pluginNodeMoq.SetupGet(m => m.Height).Returns(2);

            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;
            _diagramViewModel.Zoom = 2;
            _diagramViewModel.MouseMoved(new Point(5, 5));

            _pluginNodeMoq.VerifySet(m => m.X = -13.5);
            _pluginNodeMoq.VerifySet(m => m.Y = -13.5);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDownOnBorder_SelectsNode()
        {
            _pluginNodeMoq = new Mock<PluginNode>();
            _diagramViewModel.NodeViewModels.Add(_pluginNodeMoq.Object);

            _diagramViewModel.PreviewLeftMouseButtonDownOnBorder(_pluginNodeMoq.Object, false, false);

            _pluginNodeMoq.VerifySet(m => m.IsSelected = true);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDownOnBorder_ControlNotPressed_UnselectsPreviouslySelectedNodes()
        {
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.IsSelected).Returns(true);
            _diagramViewModel.NodeViewModels.Add(_pluginNodeMoq.Object);

            _diagramViewModel.PreviewLeftMouseButtonDownOnBorder(_pluginNodeMoq.Object, false, false);

            _pluginNodeMoq.VerifySet(m => m.IsSelected = false);
            _pluginNodeMoq.VerifySet(m => m.IsSelected = true);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDownOnBorder_ControlPressed_KeepsPreviouslySelectedNodes()
        {
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.IsSelected).Returns(true);
            _diagramViewModel.NodeViewModels.Add(_pluginNodeMoq.Object);

            _diagramViewModel.PreviewLeftMouseButtonDownOnBorder(_pluginNodeMoq.Object, true, false);

            _pluginNodeMoq.VerifySet(m => m.IsSelected = false, Times.Never);
            _pluginNodeMoq.VerifySet(m => m.IsSelected = true);
        }

        [TestMethod]
        public void TestPreviewLeftMouseButtonDownOnBorder_AltPressed_SetsInsertingNodeViewModel()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramViewModel.NodeViewModels.Add(_pluginNodeMoq.Object);

            _diagramViewModel.PreviewLeftMouseButtonDownOnBorder(_pluginNodeMoq.Object, true, true);

            _nodeProviderMoq.Verify(p => p.CreateNodeViewModelFromName(It.IsAny<string>()));
        }

        [TestMethod]
        public void TestIsDraggingDiagramCallNode_DraggingDiagramCallNodeIsNull_ReturnsFalse()
        {
            Assert.IsFalse(_diagramViewModel.IsDraggingDiagramCallNode);
        }

        [TestMethod]
        public void TestIsDraggingDiagramCallNode_DiagramDraggedIn_ReturnsTrue()
        {
            DiagramCallNodeViewModel.ProjectManager = new Mock<IProjectManager>().Object;
            _diagramViewModel.DiagramDragEnter(_diagramMoq.Object);
            Assert.IsTrue(_diagramViewModel.IsDraggingDiagramCallNode);
        }

        [TestMethod]
        public void TestDropDiagramCallText_DiagramDraggedIn_UsesDroppedDiagramNameInText()
        {
            DiagramCallNodeViewModel.ProjectManager = new Mock<IProjectManager>().Object;
            var otherDiagramMoq = new Mock<DiagramModel>();
            otherDiagramMoq.SetupGet(d => d.Name).Returns("ddd");
            _diagramViewModel.DiagramDragEnter(otherDiagramMoq.Object);
            Assert.IsTrue(_diagramViewModel.DropDiagramCallText.Contains("ddd"));
        }

        [TestMethod]
        public void TestIsSnapGridVisible_InsertingNodeViewModelNotNull_ReturnsTrue()
        {
            _nodeMoq = new Mock<NodeModel>("node");
            _pluginNodeMoq = new Mock<PluginNode>();
            _pluginNodeMoq.SetupGet(m => m.NodeModel).Returns(_nodeMoq.Object);
            _diagramMoq.SetupGet(d => d.Nodes).Returns(new List<NodeModel> { _nodeMoq.Object });

            Assert.IsFalse(_diagramViewModel.IsSnapGridVisible);
            _diagramViewModel.InsertingNodeViewModel = _pluginNodeMoq.Object;
            Assert.IsTrue(_diagramViewModel.IsSnapGridVisible);
        }

        [TestMethod]
        public void TestIsSnapGridVisible_NodeBeingDraggedTrue_ReturnsTrue()
        {
            Assert.IsFalse(_diagramViewModel.IsSnapGridVisible);
            _diagramViewModel.NodeBeingDragged = true;
            Assert.IsTrue(_diagramViewModel.IsSnapGridVisible);
        }

        [TestMethod]
        public void TestDroppedDiagramCallNode_NoDraggingDiagramCallNode_InsertingNodeViewModelNull()
        {
            _diagramViewModel.DroppedDiagramCallNode(null, null);
            Assert.IsNull(_diagramViewModel.InsertingNodeViewModel);
        }
    }
}