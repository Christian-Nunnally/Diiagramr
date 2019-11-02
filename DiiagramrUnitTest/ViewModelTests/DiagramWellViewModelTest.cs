using DiiagramrAPI.Editor;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class DiagramWellViewModelTest
    {
        private DiagramWell _diagramWellViewModel;
        private Mock<IProvideNodes> _nodeProviderMoq;
        private Mock<NodePalette> _nodeSelectorMoq;
        private Mock<IProjectManager> _projectManagerMoq;

        [TestInitialize]
        public void TestInitialize()
        {
            _projectManagerMoq = new Mock<IProjectManager>();
            _nodeProviderMoq = new Mock<IProvideNodes>();
            _nodeSelectorMoq = new Mock<NodePalette>((Func<IProvideNodes>)(() => _nodeProviderMoq.Object));

            _diagramWellViewModel = new DiagramWell(() => _projectManagerMoq.Object);
        }

        [TestMethod]
        public void TestCloseActiveDiagram_NoOpenDiagrams_DoesNotThrowException()
        {
            _diagramWellViewModel.CloseActiveDiagram();
        }

        private DiagramModel SetupProjectWithSingleDiagram()
        {
            var diagram = new DiagramModel();
            var project = new ProjectModel();
            var nodeProviderMoq = new Mock<IProvideNodes>();
            var diagramViewModelMoq = new Mock<Diagram>(diagram, nodeProviderMoq.Object, null, _nodeSelectorMoq.Object);
            var diagramViewModelList = new List<Diagram> { diagramViewModelMoq.Object };

            _projectManagerMoq.SetupAllProperties();
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(project.Diagrams);
            _projectManagerMoq.SetupGet(m => m.Diagrams).Returns(diagramViewModelList);
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);
            project.Diagrams.Add(diagram);
            return diagram;
        }

        [TestMethod]
        public void TestProjectChanged_ProjectSetToNull_OldDiagramsClosed()
        {
            var projectMoq = new Mock<ProjectModel>();
            var diagramMoq = new Mock<DiagramModel>();
            var diagramsList = new ObservableCollection<DiagramModel>
            {
                diagramMoq.Object
            };

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
            var diagramsList = new ObservableCollection<DiagramModel>
            {
                diagramMoq.Object
            };

            var newProjectMoq = new Mock<ProjectModel>();
            var newDiagramMoq1 = new Mock<DiagramModel>();
            var newDiagramMoq2 = new Mock<DiagramModel>();
            var newDiagramsList = new ObservableCollection<DiagramModel>
            {
                newDiagramMoq1.Object,
                newDiagramMoq2.Object
            };

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
