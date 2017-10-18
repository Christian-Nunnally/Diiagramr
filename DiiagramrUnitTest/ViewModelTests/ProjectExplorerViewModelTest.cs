using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stylet;
using System;
using DiiagramrAPI.Model;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace DiiagramrUnitTests.ViewModelTests
{
    [TestClass]
    public class ProjectExplorerViewModelTest
    {
        private Mock<IProjectManager> _projectManagerMoq;
        private ProjectExplorerViewModel _projectExplorerViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            MockedViewModelFactories.CreateSingletonMoqs();
            _projectManagerMoq = MockedViewModelFactories.CreateMoqProjectManager();

            Func<IProjectManager> projectManagerFactory = () => _projectManagerMoq.Object;

            _projectExplorerViewModel = new ProjectExplorerViewModel(projectManagerFactory);
        }

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNull_ReturnsFalse()
        {
            Assert.IsFalse(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCanDeleteDiagram_SelectedDiagramNotNull_ReturnsTrue()
        {
            _projectExplorerViewModel.SelectedDiagram = new DiagramModel();
            Assert.IsTrue(_projectExplorerViewModel.CanDeleteDiagram);
        }

        [TestMethod]
        public void TestCreateDiagram_CallsCreateDiagram()
        {
            _projectExplorerViewModel.CreateDiagram();
            _projectManagerMoq.Verify(m => m.CreateDiagram(), Times.Once);
        }

        [TestMethod]
        public void TestDeleteDiagram_CallsDeleteDiagramWithSelectedDiagram()
        {
            var diagram = new DiagramModel();
            _projectManagerMoq.SetupGet(m => m.CurrentDiagrams).Returns(new BindableCollection<DiagramModel> { diagram });
            _projectExplorerViewModel.SelectedDiagram = diagram;
            _projectExplorerViewModel.DeleteDiagram();
            _projectManagerMoq.Verify(m => m.DeleteDiagram(It.Is<DiagramModel>(d => d == diagram)), Times.Once);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNull_IsAddDiagramButtonIsFalse()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = null;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsFalse(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_IsAddDiagramButtonIsTrue()
        {
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = new ProjectModel();
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.IsTrue(_projectExplorerViewModel.IsAddDiagramButtonVisible);
        }

        [TestMethod]
        public void TestProjectChanged_ProjectNotNull_ProjectSet()
        {
            var project = new ProjectModel();
            _projectManagerMoq.SetupProperty(m => m.CurrentProject);
            _projectManagerMoq.Object.CurrentProject = project;
            _projectManagerMoq.Raise(m => m.CurrentProjectChanged += null);

            Assert.AreEqual(project, _projectExplorerViewModel.Project);
        }

        [TestMethod]
        public void TestDiagramProjectItemMouseDown_SingleClick_SelectedDiagramNotOpen()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectExplorerViewModel.SelectedDiagram = diagramMoq.Object;

            _projectExplorerViewModel.DiagramProjectItemMouseDown(1);

            diagramMoq.VerifySet(d => d.IsOpen = true, Times.Never);
        }

        [TestMethod]
        public void TestDiagramProjectItemMouseDown_DoubleClick_SelectedDiagramOpen()
        {
            var diagramMoq = new Mock<DiagramModel>();
            _projectExplorerViewModel.SelectedDiagram = diagramMoq.Object;

            _projectExplorerViewModel.DiagramProjectItemMouseDown(2);

            diagramMoq.VerifySet(d => d.IsOpen = true);
        }

        [TestMethod]
        public void TestCopyDiagram_DiagramSelected_NewDiagramAdded()
        {
            _projectExplorerViewModel.SelectedDiagram = new DiagramModel();

            _projectExplorerViewModel.CopyDiagram();

            _projectManagerMoq.Verify(p => p.CreateDiagram(It.IsAny<DiagramModel>()));
        }
    }
}
