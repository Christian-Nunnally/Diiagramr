using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Diiagramr.Service.Interfaces;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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

            IProjectManager ProjectManagerFactory() => _projectManagerMoq.Object;
            IProvideNodes NodeProviderFactory() => _nodeProviderMoq.Object;
            NodeSelectorViewModel NodeSelectorFactory() => _nodeSelectorMoq.Object;

            _diagramWellViewModel = new DiagramWellViewModel(ProjectManagerFactory, NodeProviderFactory, NodeSelectorFactory);
        }

        [TestMethod]
        public void TestCloseActiveDiagram_NoOpenDiagrams_DoesNotThrowException()
        {
            _diagramWellViewModel.CloseActiveDiagram();
        }

        [TestMethod]
        public void TestCloseActiveDiagram_OneOpenDiagram_SetsCurrentDiagramIsOpenToFalse()
        {
            var diagramMoq = new Mock<DiagramModel>();
            diagramMoq.SetupAllProperties();
            diagramMoq.Object.IsOpen = true;
            _diagramWellViewModel.ActiveItem = new Mock<DiagramViewModel>(diagramMoq.Object, _nodeProviderMoq.Object).Object;

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

        private DiagramModel SetupProjectWithSingleDiagram()
        {
            var diagram = new DiagramModel();
            var project = new ProjectModel();
            var nodeProviderMoq = new Mock<IProvideNodes>();
            var diagramViewModelMoq = new Mock<DiagramViewModel>(diagram, nodeProviderMoq.Object);
            var diagramViewModelList = new List<DiagramViewModel> { diagramViewModelMoq.Object };

            _projectManagerMoq.SetupAllProperties();
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(project.Diagrams);
            _projectManagerMoq.SetupGet(m => m.DiagramViewModels).Returns(diagramViewModelList);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            project.Diagrams.Add(diagram);
            return diagram;
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

        [TestMethod]
        public void TestProjectChanged_ProjectSetToNull_OldDiagramsClosed()
        {
            var projectMoq = new Mock<ProjectModel>();
            var diagramMoq = new Mock<DiagramModel>();
            var diagramsList = new ObservableCollection<DiagramModel>();
            diagramsList.Add(diagramMoq.Object);

            _projectManagerMoq.SetupGet(m => m.CurrentProject).Returns(projectMoq.Object);
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(diagramsList);

            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            diagramMoq.VerifySet(m => m.IsOpen = false);
        }

        [TestMethod]
        public void TestProjectChanged_NewProjectHas2Diagrams_CurrentDiagramSetWithNewDiagrams()
        {
            var projectMoq = new Mock<ProjectModel>();
            var diagramMoq = new Mock<DiagramModel>();
            var diagramsList = new ObservableCollection<DiagramModel>();
            diagramsList.Add(diagramMoq.Object);

            var newProjectMoq = new Mock<ProjectModel>();
            var newDiagramMoq1 = new Mock<DiagramModel>();
            var newDiagramMoq2 = new Mock<DiagramModel>();
            var newDiagramsList = new ObservableCollection<DiagramModel>();
            newDiagramsList.Add(newDiagramMoq1.Object);
            newDiagramsList.Add(newDiagramMoq2.Object);

            _projectManagerMoq.SetupGet(m => m.CurrentProject).Returns(projectMoq.Object);
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(diagramsList);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            _projectManagerMoq.SetupGet(m => m.CurrentProject).Returns(newProjectMoq.Object);
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(newDiagramsList);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.AreEqual(newDiagramsList, _diagramWellViewModel.CurrentDiagrams);
        }
    }
}
