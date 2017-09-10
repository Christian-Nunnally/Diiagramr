﻿using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.ComponentModel;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class DiagramWellViewModelTest
    {
        private DiagramWellViewModel _diagramWellViewModel;
        private Mock<IProjectManager> _projectManagerMoq;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            MockedViewModelFactories.CreateSingletonMoqs();
            _projectManagerMoq = MockedViewModelFactories.CreateMoqProjectManager();
            _nodeProviderMoq = MockedViewModelFactories.CreateMoqNodeProvider();
            _nodeSelectorMoq = MockedViewModelFactories.CreateMoqNodeSelectorViewModel();

            Func<IProjectManager> projectManagerFactory = () => _projectManagerMoq.Object;
            Func<IProvideNodes> nodeProviderFactory = () => _nodeProviderMoq.Object;
            Func<NodeSelectorViewModel> nodeSelectorFactory = () => _nodeSelectorMoq.Object;

            _diagramWellViewModel = new DiagramWellViewModel(projectManagerFactory, nodeProviderFactory, nodeSelectorFactory);
        }

        [TestMethod]
        public void TestCloseActiveDiagram_NoOpenDiagrams_DoesNotThrowException()
        {
            _diagramWellViewModel.CloseActiveDiagram();
        }

        [TestMethod]
        public void TestCloseActiveDiagram_OneOpenDiagram_SetsCurrentDiagramIsOpenToFalse()
        {
            var diagramMoq = new Mock<EDiagram>();
            diagramMoq.SetupAllProperties();
            diagramMoq.Object.IsOpen = true;
            _diagramWellViewModel.ActiveItem = new Mock<DiagramViewModel>(diagramMoq.Object, _nodeProviderMoq.Object, _nodeSelectorMoq.Object).Object;

            _diagramWellViewModel.CloseActiveDiagram();

            Assert.IsFalse(diagramMoq.Object.IsOpen);
        }

        [TestMethod]
        public void TestOpenDiagram_DiagramIsOpenSetToTrue_DiagramBecomesActiveItem()
        {
            var diagram = SetupProjectWithSingleDiagram();

            Assert.IsNull(_diagramWellViewModel.ActiveItem);
            diagram.IsOpen = true;
            Assert.AreEqual(diagram, _diagramWellViewModel.ActiveItem.Diagram);
        }

        private EDiagram SetupProjectWithSingleDiagram()
        {
            var diagram = new EDiagram();
            var project = new Project("TestProject");

            _projectManagerMoq.SetupAllProperties();
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(project.Diagrams);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            project.Diagrams.Add(diagram);
            return diagram;
        }

        [TestMethod]
        public void TestOpenDiagram_NodeSelectedWithDiagramOpen_CreatesNewNodeAndSetsDiagramInsertingNode()
        {
            var abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _nodeProviderMoq.Setup(n => n.CreateNodeViewModelFromName(It.IsAny<string>())).Returns(abstractNodeViewModelMoq.Object);
            _nodeSelectorMoq.SetupAllProperties();

            var diagram = SetupProjectWithSingleDiagram();
            diagram.IsOpen = true;

            Assert.IsNull(_diagramWellViewModel.ActiveItem.InsertingNodeViewModel);

            _nodeSelectorMoq.Object.SelectedNode = abstractNodeViewModelMoq.Object;
            _nodeSelectorMoq.Raise(m => m.PropertyChanged += null, null, new PropertyChangedEventArgs(nameof(NodeSelectorViewModel.SelectedNode)));

            Assert.AreEqual(abstractNodeViewModelMoq.Object, _diagramWellViewModel.ActiveItem.InsertingNodeViewModel);
        }

        [TestMethod]
        public void TestRightMouseDown_NoDiagramOpen_SetsNodeSelectorsNodeToNull()
        {
            _diagramWellViewModel.RightMouseDown();
            _nodeSelectorMoq.VerifySet(m => m.SelectedNode = null);
        }

        [TestMethod]
        public void TestRightMouseDown_DiagramOpenAndNodeNotSelected_SetsNodeSelectorVisibleToTrue()
        {
            var diagram = SetupProjectWithSingleDiagram();
            diagram.IsOpen = true;

            Assert.IsFalse(_diagramWellViewModel.NodeSelectorVisible);
            _diagramWellViewModel.RightMouseDown();
            Assert.IsTrue(_diagramWellViewModel.NodeSelectorVisible);
        }

        [TestMethod]
        public void TestRightMouseDown_DiagramOpenAndNodeSelected_SetsNodeSelectorSelectedNodeToNull()
        {
            var abstractNodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            _nodeSelectorMoq.SetupGet(m => m.SelectedNode).Returns(abstractNodeViewModelMoq.Object);
            _diagramWellViewModel.RightMouseDown();
            var diagram = SetupProjectWithSingleDiagram();
            diagram.IsOpen = true;

            _diagramWellViewModel.RightMouseDown();
            _nodeSelectorMoq.VerifySet(m => m.SelectedNode = null);
        }

        [TestMethod]
        public void TestLeftMouseDown_NodeSelectorVisibleTrue_SetsNodeSelectorVisibleToFalse()
        {
            _diagramWellViewModel.NodeSelectorVisible = true;
            _diagramWellViewModel.LeftMouseDown();
            Assert.IsFalse(_diagramWellViewModel.NodeSelectorVisible);
        }
    }
}
