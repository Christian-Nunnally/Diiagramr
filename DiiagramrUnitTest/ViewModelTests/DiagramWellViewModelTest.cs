﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class DiagramWellViewModelTest
    {
        private DiagramWellViewModel _diagramWellViewModel;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodeSelectorViewModel> _nodeSelectorMoq;
        private Mock<IProjectManager> _projectManagerMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _nodeSelectorMoq = new Mock<NodeSelectorViewModel>((Func<IProvideNodes>) (() => _nodeProviderMoq.Object));

            _diagramWellViewModel = new DiagramWellViewModel(() => _projectManagerMoq.Object);
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

        private DiagramModel SetupProjectWithSingleDiagram()
        {
            var diagram = new DiagramModel();
            var project = new ProjectModel();
            var nodeProviderMoq = new Mock<IProvideNodes>();
            var diagramViewModelMoq = new Mock<DiagramViewModel>(diagram, nodeProviderMoq.Object, _nodeSelectorMoq.Object);
            var diagramViewModelList = new List<DiagramViewModel> {diagramViewModelMoq.Object};

            _projectManagerMoq.SetupAllProperties();
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(project.Diagrams);
            _projectManagerMoq.SetupGet(m => m.DiagramViewModels).Returns(diagramViewModelList);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            project.Diagrams.Add(diagram);
            return diagram;
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